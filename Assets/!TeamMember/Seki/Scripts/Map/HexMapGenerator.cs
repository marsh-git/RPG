using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HexMapGenerator : MonoBehaviour{
    /// <summary>
    /// デバッグ用のマップ生成
    /// </summary>
    [Header("Tile Prefabs (View)")]
    [SerializeField] private HexTileObject tilePrefabPlain;
    [SerializeField] private HexTileObject tilePrefabHill;
    [SerializeField] private HexTileObject tilePrefabForest;
    [SerializeField] private HexTileObject tilePrefabMountain;

    /// <summary>
    /// デバッグ用のマップ生成 (データとオブジェクトを同時に生成してManagerへ登録)
    /// </summary>
    public void CreateDebugMap() {
        int mapRadius = 10;
        int currentID = 0; // 通し番号としてのユニークID

        // 中心 (0,0) から半径 mapRadius の範囲を巡回するループ
        for(int q = -mapRadius; q <= mapRadius; q++) {
            int rStart = Mathf.Max(-mapRadius, -q - mapRadius);
            int rEnd = Mathf.Min(mapRadius, -q + mapRadius);

            for(int r = rStart; r <= rEnd; r++) {
                // 1. 3D空間への変換計算
                float x = 2f * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f) / 2f * r);
                float z = 2f * (3f / 2f * r);
                Vector3 spawnPosition = new Vector3(x, 0f, z);

                // 2. ランダムな地形の決定 (Invalid, Maxを除外した有効レンジ)
                eTerrain randomTerrain = (eTerrain)Random.Range((int)eTerrain.Plain, (int)eTerrain.Mountain + 1);

                // 3. 地形に応じた適切な3Dプレハブを選択
                HexTileObject prefabToSpawn = randomTerrain switch {
                    eTerrain.Plain => tilePrefabPlain,
                    eTerrain.Hill => tilePrefabHill,
                    eTerrain.Forest => tilePrefabForest,
                    eTerrain.Mountain => tilePrefabMountain,
                    _ => tilePrefabPlain
                };

                if(prefabToSpawn == null) {
                    Debug.LogError($"【生成エラー】{randomTerrain} に対応するプレハブがインスペクターで未設定です。");
                    continue;
                }

                // 4. View（3Dオブジェクト）の生成とセットアップ
                HexTileObject newTileObject = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.Euler(0, 30, 0), this.transform);
                newTileObject.Setup(spawnPosition);
                newTileObject.name = $"HexTile_[ID:{currentID}]_({q},{r})";

                // 5. Model（純粋データクラス）の生成とセットアップ
                HexTileData newTileData = new HexTileData();
                newTileData.Setup(currentID, q, r);
                newTileData.SetTerrain(randomTerrain);
                // デバッグ用としてバイオームや属性の初期値を必要に応じて設定
                newTileData.SetBiome(eBiome.Grassland);

                // 6. 司令塔である Manager へデータとViewのペアを登録
                HexTileManager.instance.AddTile(newTileData, newTileObject);

                currentID++;
            }
        }

        Debug.Log($"【マップ生成完了】総タイル数: {currentID} マスが正常にデータ同期されました。");
    }

    public static int DecideSeedByLevel() {
        // 難易度選択のみでゲームが開始されるときは、難易度に応じたシード値を決定する。
        return -1;
    }
    public static int DecideSeedByCustom() {
        // カスタムルールでゲームが開始されるときは、カスタム内容に応じたシード値を決定する。
        return -1;
    }
    public static void CreateMap() {
        // シード値に応じたマップ生成を行う。
    }
    /// <summary>
    /// 街マスの取得
    /// </summary>
    private static void CreateTown() {
        // 中心からタイルを決定する

        // 中心タイルから周囲6マスも街マスとする

        // ※そのため、街マスの中心は端マスより1マス内側でなければいけない
    }

}