using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour {
    /// <summary>
    /// 難易度やカスタムルールから算出される、マップ生成の具体的なパラメータ設定構造体
    /// </summary>
    public struct MapGenerationConfig {
        public int Seed;                  // 再現性を担保するための乱数シード値
        public int EnemyOutpostCount;     // 敵の前哨基地の総生成数
        public float EventTileChance;      // イベントマスの生成確率 (0.0f ～ 1.0f)
        public float CropsTileChance;      // 作物マスの生成確率 (0.0f ～ 1.0f)

        // 地形分布のウェイト（合計値に対する割合で抽選）
        public int PlainWeight;
        public int HillWeight;
        public int ForestWeight;
        public int MountainWeight;
    }
    [Header("Tile")]
    [SerializeField] private HexTileObject _spawnTile = null;

    [Header("UnitPrefabs")]
    [SerializeField] private PlayerSpawner playerSpawner = null;
    [SerializeField] private OutpostSpawner enemySpawner = null;

    [Header("一括管理マスターデータベース参照")]
    [SerializeField] private BiomeDataSO mapConfig = null;
    [SerializeField] private CropsDataSO cropsConfig = null;

    [Header("Debug Settings")]
    [Tooltip("チェックを入れると、下の debugSeed で指定したシード値で固定されます")]
    [SerializeField] private bool useStaticSeed = false;
    [Tooltip("再現テストしたいシード値をここに入力します")]
    [SerializeField] private int debugSeed = 12345;

    // マップ全体の定数（半径10のヘックスエリア）
    private const int MAP_RADIUS = 10;

    // アキシアル座標系における隣接6方向の定義
    private static readonly eDirectionHex[] Directions = new eDirectionHex[] {
        eDirectionHex.UpRight, eDirectionHex.Right, eDirectionHex.DownRight,
        eDirectionHex.DownLeft,  eDirectionHex.Left,  eDirectionHex.UpLeft
    };

    /// <summary>
    /// 他クラスに変更を加えず、決定論的（再現性100%）にマップを自動生成する
    /// </summary>
    /// <param name="config">生成ルールパラメータ</param>
    public void CreateMap(MapGenerationConfig config) {
        // マスターデータ管理クラスの初期化
        TileVisualAssignor.Initialize(mapConfig, cropsConfig);
        // マップ生成専用に独立した乱数インスタンスをシード値固定で生成（非決定的な挙動の排除）
        System.Random mapRand = new System.Random(config.Seed);

        // 生成プロセス中の一時管理用プール
        List<HexTileObject> playerSpawnCandidates = new List<HexTileObject>();
        List<HexTileData> allTileList = new List<HexTileData>();
        Dictionary<int, List<HexTileData>> areaTileMap = new Dictionary<int, List<HexTileData>>();

        // 街の配置予定地（中心座標）を先んじて決定する（エリア0 -> エリア1 -> エリア2 と数珠繋ぎに決定）
        List<Vector2Int> areaCenters = DecideDirArea(mapRand);

        // フェーズ1: 基礎地形の生成 ---
        // この内部で「街マス予定地」の事前判定と山脈の排除（平滑化）を同時に完結させる
        GenerateBaseTerrain(areaCenters, config, mapRand, ref allTileList, ref areaTileMap, ref playerSpawnCandidates);

        // フェーズ2: 街マスの属性付与 ---
        // Viewの破壊・再生成を行わず、純粋なデータ（Model）の属性設定のみを安全に行う
        GenerateTowns(areaCenters, areaTileMap);

        // フェーズ3: 特殊属性（前哨基地・イベント・作物）の配置 ---
        // 街や山脈ではない「プレーンな空きマス」に対して確率抽選で配置する
        GenerateSpecialAttributes(allTileList, config, mapRand);

        // フェーズ4: タイルの見た目の変更
        SetAllTileObjectView(allTileList);

        // フェーズ5: キャラクターのスポーン ---
        // 構築が完了したマップデータ上にプレイヤーと敵を配置
        SpawnCharacters(playerSpawnCandidates);

        // デバッグログに現在のシード値を明記（バグ遭遇時にこの数値をインスペクターにコピペできるようにする）
        Debug.Log($"【マップ生成完了】シード値: {config.Seed} / 総エリア数: {areaCenters.Count} / 総タイル数: {allTileList.Count}");
    }

    /// <summary>
    /// すべてのエリアの基礎地形とViewをループ生成する。
    /// </summary>
    private void GenerateBaseTerrain(
        List<Vector2Int> areaCenters,
        MapGenerationConfig config,
        System.Random mapRand,
        ref List<HexTileData> allGeneratedTiles,
        ref Dictionary<int, List<HexTileData>> areaTileMap,
        ref List<HexTileObject> playerSpawnCandidates) {
        int currentTileID = 0;

        // 街マス（全エリアの中心＋周囲6マス）の座標を事前に計算し、検索が高速なHashSetにプールする
        HashSet<Vector2Int> townCoordinates = PrecomputeTownCoordinates(areaCenters);

        // バイオーム決定用のプールを作成（エリア0は固定で除外、エリア1, 2にランダム割り当て用）
        List<eBiome> availableBiomes = new List<eBiome>();
        for(int i = 1; i < (int)eBiome.Max; i++) {
            eBiome b = (eBiome)i;
            if(b != eBiome.Grassland) { // エリア0が草原(Plain)と仮定
                availableBiomes.Add(b);
            }
        }

        for(int areaID = 0; areaID < areaCenters.Count; areaID++) {
            Vector2Int areaCenter = areaCenters[areaID];
            List<int> registeredTileIDs = new List<int>();
            List<HexTileData> areaTiles = new List<HexTileData>();

            for(int q = -MAP_RADIUS; q <= MAP_RADIUS; q++) {
                int rStart = Mathf.Max(-MAP_RADIUS, -q - MAP_RADIUS);
                int rEnd = Mathf.Min(MAP_RADIUS, -q + MAP_RADIUS);

                for(int r = rStart; r <= rEnd; r++) {
                    int globalQ = areaCenter.x + q;
                    int globalR = areaCenter.y + r;
                    Vector2Int currentCoord = new Vector2Int(globalQ, globalR);

                    // 3D物理空間への座標変換
                    Vector3 spawnPosition = CalculateHex3DPosition(globalQ, globalR);

                    // 地形の決定（街マス予定地なら山脈を生成させない安全弁）
                    eTerrain chosenTerrain;
                    if(townCoordinates.Contains(currentCoord)) {
                        chosenTerrain = eTerrain.Plain; // 強制平滑化
                    } else {
                        chosenTerrain = ChooseTerrainDeterministic(config, mapRand); // 通常抽選
                    }

                    // View（3Dオブジェクト）のインスタンス化
                    HexTileObject newTileObject = Instantiate(_spawnTile, Vector3.zero, Quaternion.Euler(0, 30, 0), transform);
                    newTileObject.Setup(currentTileID, spawnPosition);
                    newTileObject.name = $"Tile_[ID:{currentTileID}]_Area:{areaID}_G({globalQ},{globalR})";

                    // Model（データ）の生成と初期設定
                    HexTileData newTileData = new HexTileData();
                    newTileData.Setup(currentTileID, globalQ, globalR);
                    newTileData.SetTerrain(chosenTerrain);

                    // 山脈マスの場合は、ゲームロジック用に進行不可属性を付与
                    if(chosenTerrain == eTerrain.Mountain) {
                        newTileData.SetAttributeTile(AttributeFactory.Create(eAttribute.CannotMove));
                    }

                    // マネージャーへ登録（他クラスの既存関数のみを使用）
                    HexTileManager.instance.AddTile(newTileData, newTileObject);
                    allGeneratedTiles.Add(newTileData);
                    areaTiles.Add(newTileData);

                    // 初期エリア（Area 0）の歩けるマスをプレイヤーの初期スポーン候補とする
                    if(chosenTerrain != eTerrain.Mountain && areaID == 0) {
                        playerSpawnCandidates.Add(newTileObject);
                    }

                    registeredTileIDs.Add(currentTileID);
                    currentTileID++;
                }
            }
            // エリアデータの生成と登録
            eBiome areaBiome;
            if(areaID == 0) {
                areaBiome = eBiome.Grassland; // エリア0は「草原」で確定
            } else {
                // エリア1以降は重複しないよう、プールからランダムに取得して消費する
                if(availableBiomes.Count > 0) {
                    int randBiomeIdx = mapRand.Next(0, availableBiomes.Count);
                    areaBiome = availableBiomes[randBiomeIdx];
                    availableBiomes.RemoveAt(randBiomeIdx); // 重複回避のため削除
                } else {
                    // 万が一プールが枯渇した場合はセーフティとしてフォールバック
                    areaBiome = (eBiome)((areaID % (int)eBiome.Max) + 1);
                }
            }

            HexAreaData newAreaData = new HexAreaData();
            newAreaData.Setup(areaID, areaCenter.x, areaCenter.y, areaBiome, registeredTileIDs);
            HexTileManager.instance.AddArea(newAreaData);

            areaTileMap[areaID] = areaTiles;
        }
    }

    /// <summary>
    /// 各エリアの中心に街マス（1+6マス）の「属性（データ）」を付与する。
    /// </summary>
    private void GenerateTowns(List<Vector2Int> areaCenters, Dictionary<int, List<HexTileData>> areaTileMap) {
        foreach(var areaKeyValuePair in areaTileMap) {
            int areaId = areaKeyValuePair.Key;
            List<HexTileData> areaTiles = areaKeyValuePair.Value;
            Vector2Int centerCoord = areaCenters[areaId];

            // エリア内の中心タイルデータを検索
            HexTileData townCenter = areaTiles.Find(t => t.gridPosX == centerCoord.x && t.gridPosY == centerCoord.y);
            if(townCenter == null) continue;

            // 中心を街マス属性に設定
            townCenter.SetAttributeTile(AttributeFactory.Create(eAttribute.Town));

            // 周囲6方向の隣接マスに対しても街マス属性を設定
            foreach(eDirectionHex dir in Directions) {
                // ※HexTileManagerに存在する既存のGetToDirTileを使用
                HexTileData neighbor = HexTileManager.instance.GetToDirTile(townCenter.gridPosX, townCenter.gridPosY, dir);

                // 隣接マスが存在し、かつ他エリアにはみ出していない場合のみ属性を付与
                if(neighbor != null && areaTiles.Contains(neighbor)) {
                    neighbor.SetAttributeTile(AttributeFactory.Create(eAttribute.Town));
                }
            }
        }
    }

    /// <summary>
    /// 特殊属性（前哨基地・イベント・作物）を、開いたマスに対して確率配置する
    /// </summary>
    private void GenerateSpecialAttributes(List<HexTileData> allTileList, MapGenerationConfig config, System.Random mapRand) {
        // まだ何の属性も付与されていない（山脈でも街でもない）完全な空きマスを抽出
        List<HexTileData> emptyTiles = allTileList.FindAll(t => t.Attribute == eAttribute.None);

        // 敵の前哨基地を、設定された個数に達するまでランダムに配置
        int outpostsPlaced = 0;
        while(outpostsPlaced < config.EnemyOutpostCount && emptyTiles.Count > 0) {
            int randIdx = mapRand.Next(0, emptyTiles.Count);
            HexTileData targetTile = emptyTiles[randIdx];

            targetTile.SetAttributeTile(AttributeFactory.Create(eAttribute.Outpost));
            // クラスを直接取得して処理する
            if(targetTile.attributeTile is OutpostAttribute outpostTile) {
                // TODO : そのうち難易度等に応じて外部での設定を行う
                outpostTile.Setup(3);
            }
            emptyTiles.RemoveAt(randIdx); // 配置済みのマスは候補から除外
            outpostsPlaced++;
        }

        // 残りの空きマスに対して、イベントマス・作物マスを設定された確率に基づいて配置
        // イベントマスの配置
        // ※ 確率から算出した目標配置数を決め、使用したマスは emptyTiles から除外する
        List<EventDataBase> eventDataList = new List<EventDataBase>(EventManager.instance.eventDatas);
        int targetEventCount = Mathf.RoundToInt(emptyTiles.Count * config.EventTileChance);

        while(targetEventCount > 0 && eventDataList.Count > 0 && emptyTiles.Count > 0) {
            int tileIdx = mapRand.Next(0, emptyTiles.Count);
            HexTileData targetTile = emptyTiles[tileIdx];

            targetTile.SetAttributeTile(AttributeFactory.Create(eAttribute.Event));
            if(targetTile.attributeTile is EventAttribute eventTile) {
                int eventID = mapRand.Next(0, eventDataList.Count);
                EventDataBase eventData = eventDataList[eventID];

                eventTile.Setup(eventData.EventID, eventData.endEventFlag);
                eventDataList.RemoveAt(eventID); // イベントデータの重複を防止
            }

            emptyTiles.RemoveAt(tileIdx); // 配置したタイルをリストから除外して重複を防止
            targetEventCount--;
        }

        // 作物マスの配置
        // ※ イベントマスを除外した後の「残りの空きマス」に対して個数を算出して配置する
        List<HexTileData> cropsCandidates = emptyTiles.FindAll(t => t.terrain == eTerrain.Plain || t.terrain == eTerrain.Hill);
        int targetCropsCount = Mathf.RoundToInt(cropsCandidates.Count * config.CropsTileChance);

        while(targetCropsCount > 0 && cropsCandidates.Count > 0) {
            int tileIdx = mapRand.Next(0, cropsCandidates.Count);
            HexTileData targetTile = cropsCandidates[tileIdx];

            targetTile.SetAttributeTile(AttributeFactory.Create(eAttribute.Crops));

            // 初期状態の作物データのセットアップ（TODO : 現在の作物IDは0（じゃがいも）固定）
            if(targetTile.attributeTile is CropsAttribute cropsTile) cropsTile.Setup(0);

            emptyTiles.Remove(targetTile);
            cropsCandidates.RemoveAt(tileIdx);
            targetCropsCount--;
        }

        // 残ったタイルは明確に None（何もないマス）として確定
        foreach(var tile in emptyTiles) {
            tile.SetAttributeTile(AttributeFactory.Create(eAttribute.None));
        }
    }
    /// <summary>
    /// 全てのタイルの見た目の変更
    /// </summary>
    /// <param name="allTileList"></param>
    public void SetAllTileObjectView(List<HexTileData> allTileList) {
        foreach(var tileData in allTileList) {
            TileVisualAssignor.SetTileObjectView(tileData);
        }
    }
    /// <summary>
    /// 確定したマップオブジェクト群に対してユニットのスポーン命令を出す
    /// </summary>
    private void SpawnCharacters(List<HexTileObject> playerSpawnCandidates) {
        playerSpawner.Spawn(playerSpawnCandidates);
        enemySpawner.SpawnOutpost(playerSpawnCandidates, 3);
    }

    /// <summary>
    /// 各エリアの中心と、そこから隣接する6方向のすべての「街予定地」のグローバル座標を事前に計算する
    /// </summary>
    private HashSet<Vector2Int> PrecomputeTownCoordinates(List<Vector2Int> areaCenters) {
        HashSet<Vector2Int> coords = new HashSet<Vector2Int>();

        // Pointy-Toppedにおけるアキシアル座標の6方向相対ベクトル
        Vector2Int[] hexOffsets = new Vector2Int[] {
            new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(0, 1),
            new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1)
        };

        foreach(Vector2Int center in areaCenters) {
            coords.Add(center); // 街の中心座標を追加

            // 周囲6マスの座標を計算して追加
            foreach(Vector2Int offset in hexOffsets) {
                coords.Add(new Vector2Int(center.x + offset.x, center.y + offset.y));
            }
        }
        return coords;
    }

    /// <summary>
    /// アキシアル座標(q, r)から3D空間(X, Z)への正しい幾何学的な変換を行う
    /// </summary>
    private Vector3 CalculateHex3DPosition(int q, int r) {
        float x = 2f * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f) / 2f * r);
        float z = 2f * (3f / 2f * r);
        return new Vector3(x, 0f, z);
    }

    /// <summary>
    /// 設定されたウェイト（比率）に基づき、シード乱数から地形を決定論的に決定する
    /// </summary>
    private eTerrain ChooseTerrainDeterministic(MapGenerationConfig config, System.Random rand) {
        int totalWeight = config.PlainWeight + config.HillWeight + config.ForestWeight + config.MountainWeight;
        int roll = rand.Next(0, totalWeight);

        if(roll < config.PlainWeight) return eTerrain.Plain;
        roll -= config.PlainWeight;
        if(roll < config.HillWeight) return eTerrain.Hill;
        roll -= config.HillWeight;
        if(roll < config.ForestWeight) return eTerrain.Forest;

        return eTerrain.Mountain;
    }

    /// <summary>
    /// 選択された難易度プリセットから、マップ生成の設定オブジェクト（Config）をビルドして返す
    /// </summary>
    public static MapGenerationConfig DecideConfigByLevel(eGameLevel level, int seed) {
        MapGenerationConfig config = new MapGenerationConfig {
            Seed = seed,
            PlainWeight = 50,
            HillWeight = 20,
            ForestWeight = 20,
            MountainWeight = 10
        };

        switch(level) {
            case eGameLevel.Easy:
            config.EnemyOutpostCount = 2; config.EventTileChance = 0.08f; config.CropsTileChance = 0.12f;
            break;
            case eGameLevel.Normal:
            config.EnemyOutpostCount = 4; config.EventTileChance = 0.05f; config.CropsTileChance = 0.08f;
            break;
            case eGameLevel.Hard:
            config.EnemyOutpostCount = 7; config.EventTileChance = 0.03f; config.CropsTileChance = 0.05f; config.MountainWeight = 18;
            break;
        }
        return config;
    }

    /// <summary>
    /// デバッグ用のランダム生成エントリ（HexTileManagerのAwakeから呼ばれる）
    /// </summary>
    public void CreateDebugMap() {
        int seed;

        if(useStaticSeed) {
            // インスペクターで固定シードが有効なら、指定された値をそのまま使用
            seed = debugSeed;
        } else {
            // 無効なら、Unity側のランダムから毎回異なるシードを生成
            seed = UnityEngine.Random.Range(0, 99999);
        }

        MapGenerationConfig debugConfig = DecideConfigByLevel(eGameLevel.Normal, seed);
        CreateMap(debugConfig);
    }

    /// <summary>
    /// 決定論的に隣接エリアの方向・座標を、数珠繋ぎに(エリア0 -> エリア1 -> エリア2)重複を回避して算出する
    /// </summary>
    private static List<Vector2Int> DecideDirArea(System.Random rand) {
        // スタート: エリア0は原点(0,0)から
        List<Vector2Int> areaCentersToCreate = new List<Vector2Int> { new Vector2Int(0, 0) };

        // 1ステップあたりの移動オフセット（隣り合う大きなヘックスマップの中心間距離）
        Vector2Int[] bigHexOffsets = new Vector2Int[] {
            new Vector2Int(21, -10), new Vector2Int(10, 11), new Vector2Int(-11, 21),
            new Vector2Int(-21, 10), new Vector2Int(-10, -11), new Vector2Int(11, -21)
        };

        // エリア1の方向を決定（エリア0を基準）
        int dirIndexArea1 = rand.Next(0, bigHexOffsets.Length);
        Vector2Int area1Center = bigHexOffsets[dirIndexArea1];
        areaCentersToCreate.Add(area1Center);

        // エリア2の方向を決定（エリア1を基準。エリア0へ戻らないようにする）
        Vector2Int area2Center = Vector2Int.zero;
        List<Vector2Int> possibleOffsetsForArea2 = new List<Vector2Int>();

        foreach(var offset in bigHexOffsets) {
            Vector2Int candidatePos = area1Center + offset;

            // エリア0（原点）の位置に重複して重ならない（=逆流しない）かつ、
            // すでに登録されている位置と被らない安全な移動候補のみを許可
            if(candidatePos != Vector2Int.zero && !areaCentersToCreate.Contains(candidatePos)) {
                possibleOffsetsForArea2.Add(offset);
            }
        }

        // 安全に進行方向を抽選
        if(possibleOffsetsForArea2.Count > 0) {
            int randIdx = rand.Next(0, possibleOffsetsForArea2.Count);
            area2Center = area1Center + possibleOffsetsForArea2[randIdx];
        } else {
            // 万が一のデッドロック回避のフォールバック
            area2Center = area1Center + bigHexOffsets[(dirIndexArea1 + 2) % bigHexOffsets.Length];
        }

        areaCentersToCreate.Add(area2Center);

        return areaCentersToCreate;
    }
}