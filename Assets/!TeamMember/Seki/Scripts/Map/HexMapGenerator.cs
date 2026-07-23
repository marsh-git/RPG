using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour {
    /// <summary>
    /// 難易度やカスタムルールから算出される、マップ生成の具体的なパラメータ設定構造体。
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
    [SerializeField] private BiomeVisualDataSO mapConfig = null;
    [SerializeField] private CropsVisualDataSO cropsConfig = null;
    [SerializeField] private BiomeTerrainDataSO biomeTerrainConfig = null;

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
    /// 指定された設定に基づき、決定論的（再現性100%）にマップの構築プロセスを実行する。
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

        // 各エリアの中心座標を決定
        List<Vector2Int> areaCenters = DecideDirArea(mapRand);

        // 各エリアにおいて、端から1マス内側（周囲6マスが収まる位置）に街の中心を抽選
        List<Vector2Int> townCenters = DecideTownCenters(areaCenters, mapRand);

        // フェーズ1: 基礎地形の生成
        // 街の中心座標リスト(townCenters)を渡し、街予定地の山脈排除（平滑化）を適用する
        GenerateBaseTerrain(areaCenters, townCenters, config, mapRand, ref allTileList, ref areaTileMap, ref playerSpawnCandidates);

        // フェーズ2: 街マスの属性付与
        // Viewの破壊・再生成を行わず、純粋なデータ（Model）の属性設定のみを安全に行う
        GenerateTowns(townCenters, areaTileMap);

        // フェーズ3: 特殊属性（前哨基地・イベント・作物）の配置
        // 街や山脈ではない「プレーンな空きマス」に対して確率抽選で配置する
        GenerateSpecialAttributes(allTileList, config, mapRand);

        // フェーズ4:　属性確定後、スポーン条件を満たすタイルを摘出
        playerSpawnCandidates = GetValidPlayerSpawnCandidates(areaTileMap);

        SetAllTileObjectView(allTileList);

        // フェーズ5: キャラクターのスポーン
        // 構築が完了したマップデータ上にプレイヤーと敵を配置
        SpawnCharacters(playerSpawnCandidates);

        Debug.Log($"【マップ生成完了】シード値: {config.Seed} / 総エリア数: {areaCenters.Count} / 総タイル数: {allTileList.Count}");
    }

    /// <summary>
    /// 各エリアの「街の中心座標」を決定する。
    /// エリア中心から (MAP_RADIUS - 1) マス以内の範囲に含まれるすべてのマス（中心を含む）からランダムに1点を選択する。
    /// </summary>
    /// <param name="areaCenters">エリアの中心座標リスト</param>
    /// <param name="mapRand">マップ生成用乱数インスタンス</param>
    /// <returns>各エリアの街の中心座標リスト</returns>
    private List<Vector2Int> DecideTownCenters(List<Vector2Int> areaCenters, System.Random mapRand) {
        List<Vector2Int> townCenters = new List<Vector2Int>();
        int maxDistance = MAP_RADIUS - 1;

        foreach(var areaCenter in areaCenters) {
            // 安全弁：距離が0以下の場合はエリア中心をそのまま採用
            if(maxDistance <= 0) {
                townCenters.Add(areaCenter);
                continue;
            }
            List<Vector2Int> internalCandidates = new List<Vector2Int>();
            // エリア中心からマンハッタン距離（ヘックスにおけるaxial座標の距離）が maxDistance 以内の全タイルを走査
            for(int q = -maxDistance; q <= maxDistance; q++) {
                int rStart = Mathf.Max(-maxDistance, -q - maxDistance);
                int rEnd = Mathf.Min(maxDistance, -q + maxDistance);

                for(int r = rStart; r <= rEnd; r++) {
                    Vector2Int candidateCoord = new Vector2Int(areaCenter.x + q, areaCenter.y + r);
                    internalCandidates.Add(candidateCoord);
                }
            }
            // 抽出した範囲内の全マスからランダムに1点を街の中心として確定
            if(internalCandidates.Count > 0) {
                Vector2Int selectedTownCenter = internalCandidates[mapRand.Next(0, internalCandidates.Count)];
                townCenters.Add(selectedTownCenter);
            } else {
                townCenters.Add(areaCenter);
            }
        }
        return townCenters;
    }
    /// <summary>
    /// すべてのエリアの基礎地形データをループ生成し、物理オブジェクトをインスタンス化する。
    /// </summary>
    private void GenerateBaseTerrain(
        List<Vector2Int> areaCenters,
        List<Vector2Int> townCenters,
        MapGenerationConfig config,
        System.Random mapRand,
        ref List<HexTileData> allGeneratedTiles,
        ref Dictionary<int, List<HexTileData>> areaTileMap,
        ref List<HexTileObject> playerSpawnCandidates) {
        int currentTileID = 0;

        HashSet<Vector2Int> townCoordinates = PrecomputeTownCoordinates(townCenters);

        List<eBiome> availableBiomes = new List<eBiome>();
        for(int i = 1; i < (int)eBiome.Max; i++) {
            eBiome b = (eBiome)i;
            if(b != eBiome.Grassland) availableBiomes.Add(b);
        }

        for(int areaID = 0; areaID < areaCenters.Count; areaID++) {
            Vector2Int areaCenter = areaCenters[areaID];
            List<int> registeredTileIDs = new List<int>();
            List<HexTileData> areaTiles = new List<HexTileData>();

            // 1. 先に当エリアのバイオームを決定
            eBiome areaBiome;
            if(areaID == 0) {
                areaBiome = eBiome.Grassland;
            } else {
                if(availableBiomes.Count > 0) {
                    int randBiomeIdx = mapRand.Next(0, availableBiomes.Count);
                    areaBiome = availableBiomes[randBiomeIdx];
                    availableBiomes.RemoveAt(randBiomeIdx);
                } else {
                    areaBiome = (eBiome)((areaID % (int)eBiome.Max) + 1);
                }
            }

            // 2. エリア内のタイルの座標リストを作成＆オブジェクト生成
            List<Vector2Int> assignableCoords = new List<Vector2Int>();
            Dictionary<Vector2Int, HexTileData> coordToTileMap = new Dictionary<Vector2Int, HexTileData>();

            for(int q = -MAP_RADIUS; q <= MAP_RADIUS; q++) {
                int rStart = Mathf.Max(-MAP_RADIUS, -q - MAP_RADIUS);
                int rEnd = Mathf.Min(MAP_RADIUS, -q + MAP_RADIUS);

                for(int r = rStart; r <= rEnd; r++) {
                    int globalQ = areaCenter.x + q;
                    int globalR = areaCenter.y + r;
                    Vector2Int currentCoord = new Vector2Int(globalQ, globalR);

                    Vector3 spawnPosition = CalculateHex3DPosition(globalQ, globalR);

                    HexTileObject newTileObject = Instantiate(_spawnTile, Vector3.zero, Quaternion.Euler(0, 30, 0), transform);
                    newTileObject.Setup(currentTileID, spawnPosition);
                    newTileObject.name = $"Tile_[ID:{currentTileID}]_Area:{areaID}_G({globalQ},{globalR})";

                    HexTileData newTileData = new HexTileData();
                    newTileData.Setup(currentTileID, globalQ, globalR);

                    HexTileManager.instance.AddTile(newTileData, newTileObject);
                    allGeneratedTiles.Add(newTileData);
                    areaTiles.Add(newTileData);
                    coordToTileMap[currentCoord] = newTileData;

                    // 街マス以外のマスを、地形ランダム割り当ての候補プールに追加
                    if(!townCoordinates.Contains(currentCoord)) {
                        assignableCoords.Add(currentCoord);
                    } else {
                        // 街マスは平原固定
                        newTileData.SetTerrain(eTerrain.Plain);
                    }

                    registeredTileIDs.Add(currentTileID);
                    currentTileID++;
                }
            }

            // 3. SOに基づいてバイオームごとの地形配置目標数を決定
            int mountainTarget = biomeTerrainConfig.EvaluateTerrainCount(areaBiome, eTerrain.Mountain, mapRand);
            int forestTarget = biomeTerrainConfig.EvaluateTerrainCount(areaBiome, eTerrain.Forest, mapRand);
            int hillTarget = biomeTerrainConfig.EvaluateTerrainCount(areaBiome, eTerrain.Hill, mapRand);

            // 4. シャッフル候補地に対して優先度順（例：山脈 -> 森林 -> 丘陵 -> 残り平原）で割り振る
            ShuffleList(assignableCoords, mapRand);

            int assignedIndex = 0;
            System.Action<eTerrain, int> assignTerrainBatch = (terrain, count) => {
                for(int i = 0; i < count && assignedIndex < assignableCoords.Count; i++) {
                    Vector2Int coord = assignableCoords[assignedIndex];
                    HexTileData tile = coordToTileMap[coord];
                    tile.SetTerrain(terrain);

                    if(terrain == eTerrain.Mountain) {
                        tile.SetAttributeTile(AttributeFactory.Create(eAttribute.CannotMove));
                    }
                    assignedIndex++;
                }
            };

            assignTerrainBatch(eTerrain.Mountain, mountainTarget);
            assignTerrainBatch(eTerrain.Forest, forestTarget);
            assignTerrainBatch(eTerrain.Hill, hillTarget);

            // 残ったマスはすべて Plain（平原）を割り振る
            while(assignedIndex < assignableCoords.Count) {
                Vector2Int coord = assignableCoords[assignedIndex];
                coordToTileMap[coord].SetTerrain(eTerrain.Plain);
                assignedIndex++;
            }

            // エリアデータの生成と登録
            HexAreaData newAreaData = new HexAreaData();
            newAreaData.Setup(areaID, areaCenter.x, areaCenter.y, areaBiome, registeredTileIDs);
            HexTileManager.instance.AddArea(newAreaData);

            areaTileMap[areaID] = areaTiles;
        }
    }
    /// <summary>
    /// 決定済みの街マスの中心座標を基に、各エリアの7マス（中心+周囲6方向）へ「街属性」データを付与する。
    /// </summary>
    private void GenerateTowns(List<Vector2Int> townCenters, Dictionary<int, List<HexTileData>> areaTileMap) {
        for(int areaId = 0; areaId < townCenters.Count; areaId++) {
            if(!areaTileMap.ContainsKey(areaId)) continue;

            List<HexTileData> areaTiles = areaTileMap[areaId];
            Vector2Int centerCoord = townCenters[areaId];

            // エリア内の中心タイルデータを検索
            HexTileData townCenter = areaTiles.Find(t => t.gridPosX == centerCoord.x && t.gridPosY == centerCoord.y);
            if(townCenter == null) continue;

            // 中心タイルに街属性を設定
            townCenter.SetAttributeTile(AttributeFactory.Create(eAttribute.Town));

            // 周囲6方向の隣接マスに対しても街属性を設定
            foreach(eDirectionHex dir in Directions) {
                HexTileData neighbor = HexTileManager.instance.GetToDirTile(townCenter.gridPosX, townCenter.gridPosY, dir);

                // 隣接マスが存在し、かつ自エリア内に収まっている場合のみ属性を付与
                if(neighbor != null && areaTiles.Contains(neighbor)) {
                    neighbor.SetAttributeTile(AttributeFactory.Create(eAttribute.Town));
                }
            }
        }
    }
    /// <summary>
    /// 特殊属性（前哨基地・イベント・作物）を、残された空きマスに対して確率・条件に基づき配置する。
    /// </summary>
    private void GenerateSpecialAttributes(List<HexTileData> allTileList, MapGenerationConfig config, System.Random mapRand) {
        // まだ何の属性も付与されていない完全な空きマスを抽出
        List<HexTileData> emptyTiles = allTileList.FindAll(t => t.Attribute == eAttribute.None);

        // 敵の前哨基地を、設定された個数に達するまでランダムに配置
        int outpostsPlaced = 0;
        while(outpostsPlaced < config.EnemyOutpostCount && emptyTiles.Count > 0) {
            int randIdx = mapRand.Next(0, emptyTiles.Count);
            HexTileData targetTile = emptyTiles[randIdx];

            targetTile.SetAttributeTile(AttributeFactory.Create(eAttribute.Outpost));
            if(targetTile.attributeTile is OutpostAttribute outpostTile) {
                outpostTile.Setup(3);
            }

            emptyTiles.RemoveAt(randIdx);
            outpostsPlaced++;
        }

        // イベントマスの配置
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
                eventDataList.RemoveAt(eventID); // 重複防止
            }

            emptyTiles.RemoveAt(tileIdx);
            targetEventCount--;
        }

        // 作物マスの配置（平原または丘陵の空きマスを対象とする）
        List<HexTileData> cropsCandidates = emptyTiles.FindAll(t => t.terrain == eTerrain.Plain || t.terrain == eTerrain.Hill);
        int targetCropsCount = Mathf.RoundToInt(cropsCandidates.Count * config.CropsTileChance);

        while(targetCropsCount > 0 && cropsCandidates.Count > 0) {
            int tileIdx = mapRand.Next(0, cropsCandidates.Count);
            HexTileData targetTile = cropsCandidates[tileIdx];

            targetTile.SetAttributeTile(AttributeFactory.Create(eAttribute.Crops));
            if(targetTile.attributeTile is CropsAttribute cropsTile) {
                cropsTile.Setup(0); // 初期状態のセットアップ
            }

            emptyTiles.Remove(targetTile);
            cropsCandidates.RemoveAt(tileIdx);
            targetCropsCount--;
        }

        // 残ったタイルは明確に None（何もない平坦なマス）として確定
        foreach(var tile in emptyTiles) {
            tile.SetAttributeTile(AttributeFactory.Create(eAttribute.None));
        }
    }
    /// <summary>
    /// 全属性決定後、Area0の「属性なし」かつ「山脈以外」の有効なスポーン候補タイルオブジェクトを収集する。
    /// </summary>
    private List<HexTileObject> GetValidPlayerSpawnCandidates(Dictionary<int, List<HexTileData>> areaTileMap) {
        List<HexTileObject> candidates = new List<HexTileObject>();

        if(!areaTileMap.TryGetValue(0, out List<HexTileData> area0Tiles)) {
            Debug.LogError("【エラー】Area 0 のタイルデータが存在しません。");
            return candidates;
        }

        foreach(var tileData in area0Tiles) {
            // 条件: 属性がNone (街/前哨基地/イベント/作物/移動不可のいずれでもない) かつ 山脈でない
            if(tileData.Attribute == eAttribute.None && tileData.terrain != eTerrain.Mountain) {
                HexTileObject tileObj = HexTileManager.instance.GetTileObject(tileData.ID);
                if(tileObj != null) {
                    candidates.Add(tileObj);
                }
            }
        }

        // 万が一候補がゼロの場合のフォールバック (エリア0の非山脈マスを最小保障)
        if(candidates.Count == 0) {
            Debug.LogWarning("【警告】完全な空きマスが存在しないため、街・特殊属性マスを除く非山脈マスをフォールバック検索します。");
            foreach(var tileData in area0Tiles) {
                if(tileData.terrain != eTerrain.Mountain && tileData.Attribute != eAttribute.Town) {
                    HexTileObject tileObj = HexTileManager.instance.GetTileObject(tileData.ID);
                    if(tileObj != null) candidates.Add(tileObj);
                }
            }
        }

        return candidates;
    }
    /// <summary>
    /// 全てのタイルデータに対応する3Dビジュアル（見た目）を割り当てる。
    /// </summary>
    /// <param name="allTileList">全タイルデータのリスト</param>
    public void SetAllTileObjectView(List<HexTileData> allTileList) {
        foreach(var tileData in allTileList) {
            TileVisualAssignor.SetTileObjectView(tileData);
        }
    }
    /// <summary>
    /// 構築が完了したマップデータに基づき、プレイヤーや敵ユニットのスポーン処理を実行する。
    /// </summary>
    /// <param name="playerSpawnCandidates">プレイヤースポーン候補地のリスト</param>
    private void SpawnCharacters(List<HexTileObject> playerSpawnCandidates) {
        playerSpawner.Spawn(playerSpawnCandidates);
        enemySpawner.SpawnOutpost(playerSpawnCandidates, 3);
    }
    /// <summary>
    /// 決定済みの街の中心座標を基に、中心と隣接する6方向すべての「街予定地」のグローバル座標を事前に計算して収集する。
    /// </summary>
    /// <param name="townCenters">街の中心座標リスト</param>
    /// <returns>街が占有する全座標のハッシュセット</returns>
    private HashSet<Vector2Int> PrecomputeTownCoordinates(List<Vector2Int> townCenters) {
        HashSet<Vector2Int> coords = new HashSet<Vector2Int>();

        Vector2Int[] hexOffsets = new Vector2Int[] {
            new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(0, 1),
            new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1)
        };

        foreach(Vector2Int center in townCenters) {
            coords.Add(center); // 街の中心座標を追加

            foreach(Vector2Int offset in hexOffsets) {
                coords.Add(new Vector2Int(center.x + offset.x, center.y + offset.y));
            }
        }
        return coords;
    }
    /// <summary>
    /// アキシアル座標(q, r)から3D空間のワールド座標(X, Z)へ幾何学的な変換を行う。
    /// </summary>
    /// <param name="q">q軸座標</param>
    /// <param name="r">r軸座標</param>
    /// <returns>対応する3D空間のベクトル</returns>
    private Vector3 CalculateHex3DPosition(int q, int r) {
        float x = 2f * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f) / 2f * r);
        float z = 2f * (3f / 2f * r);
        return new Vector3(x, 0f, z);
    }
    /// <summary>
    /// 設定されたウェイト（比率）に基づき、シード乱数から地形を決定論的に抽選・決定する。
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
    /// 選択された難易度プリセットとシード値から、マップ生成の設定オブジェクト（Config）をビルドして返す。
    /// </summary>
    /// <param name="level">ゲーム難易度</param>
    /// <param name="seed">乱数シード値</param>
    /// <returns>構築されたマップ生成設定</returns>
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
    /// デバッグ用のランダムマップ生成エントリポイント。
    /// </summary>
    public void CreateDebugMap() {
        int seed;
        if(useStaticSeed) {
            seed = debugSeed; // 固定シードが有効なら指定値を使用
        } else {
            seed = Random.Range(0, 99999);
        }
        MapGenerationConfig debugConfig = DecideConfigByLevel(eGameLevel.Normal, seed);
        CreateMap(debugConfig);
    }
    /// <summary>
    /// 隣接エリアの中心方向・座標を、数珠繋ぎに(エリア0 -> エリア1 -> エリア2)重複を回避して決定論的に算出する。
    /// </summary>
    /// <param name="rand">乱数インスタンス</param>
    /// <returns>各エリアの中心座標リスト</returns>
    private static List<Vector2Int> DecideDirArea(System.Random rand) {
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

            // エリア0（原点）の位置に重複して重ならないかつ、すでに登録されている位置と被らない候補のみ許可
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
    /// <summary>
    /// シード乱数を使用してリスト要素をシャッフル（Fisher-Yates アルゴリズム）する。
    /// </summary>
    private void ShuffleList<T>(List<T> list, System.Random rand) {
        int n = list.Count;
        while(n > 1) {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}