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

    [Header("TilePrefabs")]
    [SerializeField] private HexTileObject tilePrefabPlain = null;
    [SerializeField] private HexTileObject tilePrefabHill = null;
    [SerializeField] private HexTileObject tilePrefabForest = null;
    [SerializeField] private HexTileObject tilePrefabMountain = null;

    [Header("UnitPrefabs")]
    [SerializeField] private PlayerSpawner playerSpawner = null;
    [SerializeField] private EnemySpawner enemySpawner = null;

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
    /// 【メインエントリ】他クラスに変更を加えず、決定論的（再現性100%）にマップを自動生成する
    /// </summary>
    /// <param name="config">生成ルールパラメータ</param>
    public void CreateMap(MapGenerationConfig config) {
        // マップ生成専用に独立した乱数インスタンスをシード値固定で生成（非決定的な挙動の排除）
        System.Random mapRand = new System.Random(config.Seed);

        // 生成プロセス中の一時管理用プール
        List<HexTileObject> playerSpawnCandidates = new List<HexTileObject>();
        List<HexTileData> allGeneratedTiles = new List<HexTileData>();
        Dictionary<int, List<HexTileData>> areaTileMap = new Dictionary<int, List<HexTileData>>();

        // 街の配置予定地（中心座標）を先んじて決定する（mapRandを伝播させて方角も固定）
        List<Vector2Int> areaCenters = DecideDirArea(mapRand);

        // --- フェーズ 1: 基礎地形の生成 ---
        // この内部で「街マス予定地」の事前判定と山脈の排除（平滑化）を同時に完結させる
        GenerateBaseTerrain(areaCenters, config, mapRand, ref allGeneratedTiles, ref areaTileMap, ref playerSpawnCandidates);

        // --- フェーズ 2: 街マスの属性付与 ---
        // Viewの破壊・再生成を行わず、純粋なデータ（Model）の属性設定のみを安全に行う
        GenerateTowns(areaCenters, areaTileMap);

        // --- フェーズ 3: 特殊属性（前哨基地・イベント・作物）の配置 ---
        // 街や山脈ではない「プレーンな空きマス」に対して確率抽選で配置する
        GenerateSpecialAttributes(allGeneratedTiles, config, mapRand);

        // --- フェーズ 4: キャラクターのスポーン ---
        // 構築が完了したマップデータ上にプレイヤーと敵を配置
        SpawnCharacters(playerSpawnCandidates);

        // デバッグログに現在のシード値を明記（バグ遭遇時にこの数値をインスペクターにコピペできるようにする）
        Debug.Log($"【マップ生成完了】シード値: {config.Seed} / 総エリア数: {areaCenters.Count} / 総タイル数: {allGeneratedTiles.Count}");
    }

    /// <summary>
    /// 【フェーズ 1】すべてのエリアの基礎地形とViewをループ生成する。
    /// 街マス予定地である場合は、山脈を弾いて最初から「平原」として生成する。
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
                    HexTileObject prefabToSpawn = GetTerrainPrefab(chosenTerrain);
                    HexTileObject newTileObject = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.Euler(0, 30, 0), this.transform);
                    newTileObject.Setup(currentTileID, spawnPosition);
                    newTileObject.name = $"Tile_[ID:{currentTileID}]_Area:{areaID}_G({globalQ},{globalR})";

                    // Model（データ）の生成と初期設定
                    HexTileData newTileData = new HexTileData();
                    newTileData.Setup(currentTileID, globalQ, globalR);
                    newTileData.SetTerrain(chosenTerrain);

                    // 山脈マスの場合は、ゲームロジック用に進行不可属性を付与
                    if(chosenTerrain == eTerrain.Mountain) {
                        newTileData.SetAttribute(eAttribute.CannotMove);
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
            HexAreaData newAreaData = new HexAreaData();
            eBiome areaBiome = (eBiome)((areaID % (int)eBiome.Max) + 1);
            newAreaData.Setup(areaID, areaCenter.x, areaCenter.y, areaBiome, registeredTileIDs);
            HexTileManager.instance.AddArea(newAreaData);

            areaTileMap[areaID] = areaTiles;
        }
    }

    /// <summary>
    /// 【フェーズ 2】各エリアの中心に街マス（1+6マス）の「属性（データ）」を付与する。
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
            townCenter.SetAttribute(eAttribute.Town);

            // 周囲6方向の隣接マスに対しても街マス属性を設定
            foreach(eDirectionHex dir in Directions) {
                // ※HexTileManagerに存在する既存のGetToDirTileを使用
                HexTileData neighbor = HexTileManager.instance.GetToDirTile(townCenter.gridPosX, townCenter.gridPosY, dir);

                // 隣接マスが存在し、かつ他エリアにはみ出していない場合のみ属性を付与
                if(neighbor != null && areaTiles.Contains(neighbor)) {
                    neighbor.SetAttribute(eAttribute.Town);
                }
            }
        }
    }

    /// <summary>
    /// 【フェーズ 3】特殊属性（前哨基地・イベント・作物）を、開いたマスに対して確率配置する
    /// </summary>
    private void GenerateSpecialAttributes(List<HexTileData> allGeneratedTiles, MapGenerationConfig config, System.Random mapRand) {
        // まだ何の属性も付与されていない（山脈でも街でもない）完全な空きマスを抽出
        List<HexTileData> emptyTiles = allGeneratedTiles.FindAll(t => t.attribute == eAttribute.None);

        // 3-A: 敵の前哨基地を、設定された個数に達するまでランダムに配置
        int outpostsPlaced = 0;
        while(outpostsPlaced < config.EnemyOutpostCount && emptyTiles.Count > 0) {
            int randIdx = mapRand.Next(0, emptyTiles.Count);
            HexTileData targetTile = emptyTiles[randIdx];

            targetTile.SetAttribute(eAttribute.Outpost);
            emptyTiles.RemoveAt(randIdx); // 配置済みのマスは候補から除外
            outpostsPlaced++;
        }

        // 3-B: 残りの空きマスに対して、イベントマス・作物マスを設定された確率に基づいて配置
        foreach(var tile in emptyTiles) {
            double roll = mapRand.NextDouble();

            if(roll < config.EventTileChance) {
                tile.SetAttribute(eAttribute.Event);
            } else if(roll < config.EventTileChance + config.CropsTileChance) {
                tile.SetAttribute(eAttribute.Crops);
            }
        }
    }

    /// <summary>
    /// 【フェーズ 4】確定したマップオブジェクト群に対してユニットのスポーン命令を出す
    /// </summary>
    private void SpawnCharacters(List<HexTileObject> playerSpawnCandidates) {
        playerSpawner.Spawn(playerSpawnCandidates);
        enemySpawner.SpawnEnemy(playerSpawnCandidates, 5);
    }

    // =========================================================================
    // 内部補助ヘルパー関数
    // =========================================================================

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

    private HexTileObject GetTerrainPrefab(eTerrain terrain) {
        switch(terrain) {
            case eTerrain.Plain: return tilePrefabPlain;
            case eTerrain.Hill: return tilePrefabHill;
            case eTerrain.Forest: return tilePrefabForest;
            case eTerrain.Mountain: return tilePrefabMountain;
        }
        return null;
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
    /// 決定論的に隣接エリアの方向・座標を算出する
    /// </summary>
    private static List<Vector2Int> DecideDirArea(System.Random rand) {
        List<Vector2Int> areaCentersToCreate = new List<Vector2Int> { new Vector2Int(0, 0) };
        Vector2Int[] bigHexOffsets = new Vector2Int[] {
            new Vector2Int(21, -10), new Vector2Int(10, 11), new Vector2Int(-11, 21),
            new Vector2Int(-21, 10), new Vector2Int(-10, -11), new Vector2Int(11, -21)
        };

        List<int> directionIndices = new List<int> { 0, 1, 2, 3, 4, 5 };
        for(int i = 0; i < 2; i++) {
            int randIdx = rand.Next(0, directionIndices.Count);
            int chosenDirIdx = directionIndices[randIdx];
            directionIndices.RemoveAt(randIdx);
            areaCentersToCreate.Add(bigHexOffsets[chosenDirIdx]);
        }
        return areaCentersToCreate;
    }
}