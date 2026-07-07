using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 敵プレハブ
    [SerializeField]
    private EnemyBase enemyPrefab;

    // キャラクター管理クラス
    [SerializeField]
    private CharacterManager characterManager;

    /// <summary>
    /// 指定したタイルに敵を生成する
    /// </summary>
    /// <param name="spawnTile">生成先のタイル</param>
    /// <returns>生成した敵</returns>
    public EnemyBase Spawn(HexTile spawnTile)
    {
        // 敵プレハブが設定されているか確認する
        if (enemyPrefab == null)
        {
            return null;
        }

        // CharacterManagerが設定されているか確認する
        if (characterManager == null)
        {
            return null;
        }

        // タイルが存在しない場合は生成しない
        if (spawnTile == null)
        {
            return null;
        }

        // 敵を生成する
        EnemyBase enemy = Instantiate(enemyPrefab);

        // タイルへ配置する
        enemy.SetTile(spawnTile);

        // 管理対象へ登録する
        characterManager.Register(enemy);

        return enemy;
    }
}