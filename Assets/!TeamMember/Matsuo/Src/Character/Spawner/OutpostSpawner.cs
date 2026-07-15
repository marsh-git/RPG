using System.Collections.Generic;
using UnityEngine;

public class OutpostSpawner : MonoBehaviour
{
    // 前哨基地プレハブ
    [SerializeField]
    private EnemySpawnPoint outpostPrefab = null;

    /// <summary>
    /// 候補タイルからランダムな位置へ前哨基地を生成する
    /// </summary>
    /// <param name="candidateTiles">生成候補タイル</param>
    /// <param name="spawnCount">生成する前哨基地の数</param>
    public void SpawnOutpost(List<HexTileObject> candidateTiles, int spawnCount)
    {
        // 候補タイルまたはプレハブが存在しない場合は処理しない
        if (candidateTiles == null || candidateTiles.Count == 0 || outpostPrefab == null)
        {
            Debug.LogError("前哨基地の生成に失敗しました（候補タイルがない、またはPrefabが未設定です）");
            return;
        }

        // 同じタイルへ生成しないよう候補リストをコピーする
        List<HexTileObject> spawnTiles = new List<HexTileObject>(candidateTiles);

        // 指定数だけ前哨基地を生成する
        for (int i = 0; i < spawnCount; i++)
        {
            // 候補タイルがなくなったら終了する
            if (spawnTiles.Count == 0)
            {
                break;
            }

            // ランダムなタイルを選択する
            int randomIndex = Random.Range(0, spawnTiles.Count);
            HexTileObject targetTile = spawnTiles[randomIndex];

            // 選択したタイルを候補から除外する
            spawnTiles.RemoveAt(randomIndex);

            // タイル情報を取得
            HexTileData tileData = targetTile.GetTileData();

            // タイルの状態を更新
            tileData.SetTileState(eTileState.Building);

            // 前哨基地を生成する
            EnemySpawnPoint outpost = Instantiate(outpostPrefab, targetTile.transform.position, Quaternion.identity);

            // デバッグ用の名前を設定する
            outpost.name = $"Outpost_{i}";

            // 前哨基地の現在タイルを設定する
            outpost.Initialize(targetTile.ID);

            // 動作確認用に敵を1体生成する
            outpost.SpawnEnemy();

            Debug.Log($"【前哨基地生成】{outpost.name} を {targetTile.name} に配置しました。");
        }
    }
}