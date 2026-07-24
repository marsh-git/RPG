using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("敵プレハブ")]
    [SerializeField]
    private EnemyBase enemyPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 前哨基地から敵を生成
    /// </summary>
    /// <param name="tile">生成元のタイル</param>
    public void SpawnEnemy(HexTileData tile)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("敵プレハブが設定されていません");
            return;
        }

        // 仮で生成位置を取得
        HexTileObject tileObject = HexTileManager.instance.GetTileObject(tile.ID);

        if (tileObject == null)
        {
            return;
        }

        EnemyBase enemy = Instantiate(enemyPrefab,tileObject.transform.position,Quaternion.identity);

        // タイル設定
        enemy.SetTile(tile.ID);

        // キャラクター管理へ登録
        CharacterManager.Instance.Register(enemy);
    }
}