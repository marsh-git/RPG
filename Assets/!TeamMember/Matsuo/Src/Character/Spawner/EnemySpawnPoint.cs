using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("敵プレハブ")]
    [SerializeField]
    private EnemyBase enemyPrefab;

    [Header("生成範囲")]
    [SerializeField]
    private int spawnRange = 2;

    // この前哨基地のあるタイル
    public int TileID { get; private set; }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(int tileID)
    {
        TileID = tileID;
    }

    /// <summary>
    /// 敵を1体生成
    /// </summary>
    public void SpawnEnemy()
    {
        HexTileData myTile = HexTileManager.instance.GetTileData(TileID);

        if (myTile == null) return;


        // 有効な生成候補取得
        List<HexTileData> candidates = TileRangeExpansion.GetValidTiles(myTile, spawnRange);

        if (candidates.Count == 0) return;

        // ランダム選択
        HexTileData spawnTile = candidates[Random.Range(0, candidates.Count)];
        spawnTile.SetTileState(eTileState.CharacterIn);
        // 敵生成
        EnemyBase enemy = Instantiate(enemyPrefab, spawnTile.GetTilePos(), Quaternion.identity);
        enemy.SetTile(spawnTile.ID);

        CharacterManager.Instance.Register(enemy);

        Debug.Log($"敵生成 : {spawnTile.ID}");
    }
}