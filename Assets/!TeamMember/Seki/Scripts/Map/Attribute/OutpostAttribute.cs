using Mirror.BouncyCastle.Security;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpostAttribute : IAttributeTile {
    // 前哨基地の耐久値（破壊されると敵の生成が止まる）
    public int baseHP { get; private set; } = 0;
    // ターンごとに敵を生成するカウンター
    private int _turnSpawnCounter = -1;
    // 敵を生成するまでのターンの最大数
    private int _maxSpawnCounter = -1;
    // 敵を生成する範囲
    private int _spawnRange = -1;
    // 生成する敵のプレハブ（そのうち種類が増えて配列型になりそう）
    private EnemyBase _enemyPrefab = null;
    // 生成した敵のカウンター
    private int _enemySpawnCounter = -1;
    // 生成する敵の最大数
    private int _maxEnemySpawn = -1;

    private const int _TURN_SPAWN_COUNT = 3; // 敵を生成するターン数
    private const int _DEFAULT_SPAWN_RANGE = 2; // 敵を生成する範囲

    /// <summary>
    /// 自身の属性取得
    /// </summary>
    public eAttribute AttributeType => eAttribute.Outpost;
    /// <summary>
    /// 自身の建物タイプ取得
    /// </summary>
    public eBuildingType BuildingType => eBuildingType.Attackable;

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
    /// <param name="setData"></param>
    /// <param name="enemyPrefab"></param>
    public void Setup(OutpostResultData setData, EnemyBase enemyPrefab = null) {
        baseHP = setData.HP;
        _maxEnemySpawn = setData.maxSpawnNum;
        _enemyPrefab = enemyPrefab;
        _turnSpawnCounter = _TURN_SPAWN_COUNT;
        _maxSpawnCounter = _TURN_SPAWN_COUNT;
        _spawnRange = _DEFAULT_SPAWN_RANGE;
        Debug.Log($" 前哨地生成完了 : HP {baseHP}");
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
        spawnTile.SetTileState(eTileMoveState.CharacterIn);
        // 敵の生成
        EnemyBase enemy = Object.Instantiate(_enemyPrefab, spawnTile.GetTilePos(), Quaternion.identity);
        enemy.SetTile(spawnTile.ID);
        // 管理クラスへの登録
        CharacterManager.Instance.Register(enemy);
        // カウンターを増やす
        _enemySpawnCounter++;

        Debug.Log($"[OutpostAttribute] 敵生成完了 : TileID {spawnTile.ID}");
    }
    /// <summary>
    /// 敵の生成処理
    /// </summary>
    /// <param name="tile"></param>
    private void SpawnEnemy(HexTileData tile) {
        if(tile == null || _enemyPrefab == null) return;
        // 一定以上生成した場合、新たに生成しない
        if(_enemySpawnCounter < _maxEnemySpawn) return;
        // 有効な生成候補を取得
        List<HexTileData> candidates = TileRangeExpansion.GetValidTiles(tile, _spawnRange);
        if(candidates == null || candidates.Count == 0) return;
        // 候補からランダムにタイルを選択
        int randIndex = Random.Range(0, candidates.Count);
        HexTileData spawnTile = candidates[randIndex];
        // タイル状態をキャラクターがいる状態にする
        spawnTile.SetTileState(eTileMoveState.CharacterIn);
        // 敵の生成
        EnemyBase enemy = Object.Instantiate(_enemyPrefab, spawnTile.GetTilePos(), Quaternion.identity);
        enemy.SetTile(spawnTile.ID);
        // 管理クラスへの登録
        CharacterManager.Instance.Register(enemy);
        // カウンターを増やす
        _enemySpawnCounter++;

        Debug.Log($"[OutpostAttribute] 敵生成完了 : TileID {spawnTile.ID}");
    }
    /// <summary>
    /// 前哨地のHPを計算
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int CalcOutpostHP(OutpostData data) {

        return 0;
    }
}
