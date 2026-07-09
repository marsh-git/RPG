using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 敵プレハブ
    [SerializeField]
    private EnemyBase enemyPrefab = null;

    // キャラクター管理クラス
    [SerializeField]
    private CharacterManager characterManager = null;

    /// <summary>
    /// 候補タイルからランダムな位置へ敵を生成する
    /// </summary>
    /// <param name="candidateTiles">生成候補タイル</param>
    /// <param name="spawnCount">生成する敵の数</param>
    public void SpawnEnemy(List<HexTileObject> candidateTiles, int spawnCount)
    {
        // 候補タイルまたはプレハブが存在しない場合は処理しない
        if (candidateTiles == null || candidateTiles.Count == 0 || enemyPrefab == null)
        {
            Debug.LogError("敵の生成に失敗しました（候補タイルがない、またはPrefabが未設定です）");
            return;
        }

        // 同じタイルへ生成しないよう候補リストをコピーする
        List<HexTileObject> spawnTiles = new List<HexTileObject>(candidateTiles);

        // 指定数だけ敵を生成する
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

            // 敵を生成する
            EnemyBase enemy = Instantiate(enemyPrefab, targetTile.transform.position, Quaternion.identity);

            // デバッグ用の名前を設定する
            enemy.name = $"Enemy_{i}";

            // 敵の現在タイルを設定する
            enemy.SetTile(targetTile.ID);

            Debug.Log($"【敵生成】{enemy.name} を {targetTile.name} に配置しました。");

            // キャラクター管理へ登録する
            characterManager.Register(enemy);
        }
    }
}