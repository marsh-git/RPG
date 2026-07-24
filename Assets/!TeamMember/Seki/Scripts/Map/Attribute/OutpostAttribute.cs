using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpostAttribute : IAttributeTile {
    private int _turnSpawnCounter = -1;
    private int _maxSpawnCounter = -1;
    private int _spawnRange = 2;
    private EnemyBase _enemyPrefab = null;

    /// <summary>
    /// 自身の属性取得
    /// </summary>
    public eAttribute AttributeType => eAttribute.Outpost;

    /// <summary>
    /// キャラクターがこのマスを踏んだ瞬間（前哨基地の破壊処理）
    /// </summary>
    public void OnEnterTile(HexTileData tile, CharacterBase character = null) {
        // 自身の属性を解除して破壊された扱いにする
        tile.SetAttributeTile(null);
    }

    /// <summary>
    /// キャラクターがこのマスでターンを終了時処理
    /// </summary>
    public void OnTurnEndOnTile(HexTileData tile, CharacterBase character = null) {
    }

    /// <summary>
    /// ターン経過によるタイル処理
    /// </summary>
    public void OnTickTile(HexTileData tile) {
        _turnSpawnCounter--;

        if(_turnSpawnCounter <= 0) {
            SpawnEnemy(tile);
            // カウンターをリセットして次の生成サイクルへ
            _turnSpawnCounter = _maxSpawnCounter;
        }
    }
    /// <summary>
    /// プレイヤーに見つかった時の処理
    /// </summary>
    public void OnFoundThisFirst(HexTileData tile) {
    }
    /// <summary>
    /// 準備処理
    /// </summary>
    /// <param name="setCounter">スポーンまでのターンカウント</param>
    /// <param name="enemyPrefab">スポーンさせる敵のプレハブ</param>
    /// <param name="spawnRange">生成範囲（デフォルト: 2）</param>
    public void Setup(int setCounter, EnemyBase enemyPrefab = null, int spawnRange = 2) {
        _turnSpawnCounter = setCounter;
        _maxSpawnCounter = setCounter;
        _enemyPrefab = enemyPrefab;
        _spawnRange = spawnRange;
    }
    /// <summary>
    /// シード値での生成
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="spawnRand"></param>
    public void FirstSpawnBySeed(HexTileData tile, System.Random spawnRand) {
        if(tile == null || _enemyPrefab == null) return;
        // 有効な生成候補を取得
        List<HexTileData> candidates = TileRangeExpansion.GetValidTiles(tile, 1);
        if(candidates == null || candidates.Count == 0) return;
        // 候補からランダムにタイルを選択
        int randIndex = spawnRand.Next(0, candidates.Count);
        HexTileData spawnTile = candidates[randIndex];
        // タイル状態をキャラクターがいる状態にする
        spawnTile.SetTileState(eTileState.CharacterIn);
        // 敵の生成
        EnemyBase enemy = Object.Instantiate(_enemyPrefab, spawnTile.GetTilePos(), Quaternion.identity);
        enemy.SetTile(spawnTile.ID);
        // 管理クラスへの登録
        CharacterManager.Instance.Register(enemy);

        Debug.Log($"[OutpostAttribute] 敵生成完了 : TileID {spawnTile.ID}");
    }
    /// <summary>
    /// 敵の生成処理
    /// </summary>
    private void SpawnEnemy(HexTileData tile) {
        if(tile == null || _enemyPrefab == null) return;
        // 有効な生成候補を取得
        List<HexTileData> candidates = TileRangeExpansion.GetValidTiles(tile, _spawnRange);
        if(candidates == null || candidates.Count == 0) return;
        // 候補からランダムにタイルを選択
        int randIndex = Random.Range(0, candidates.Count);
        HexTileData spawnTile = candidates[randIndex];
        // タイル状態をキャラクターがいる状態にする
        spawnTile.SetTileState(eTileState.CharacterIn);
        // 敵の生成
        EnemyBase enemy = Object.Instantiate(_enemyPrefab, spawnTile.GetTilePos(), Quaternion.identity);
        enemy.SetTile(spawnTile.ID);
        // 管理クラスへの登録
        CharacterManager.Instance.Register(enemy);

        Debug.Log($"[OutpostAttribute] 敵生成完了 : TileID {spawnTile.ID}");
    }
}
