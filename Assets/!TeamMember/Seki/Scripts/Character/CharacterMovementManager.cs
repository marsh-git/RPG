using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour {
    public static CharacterMovementManager instance { get; private set; } = null;

    // 動かす対象のキャラクターリスト（Nullによるバグを防ぐため初期化しておく）
    private List<CharacterBase> _moveCharacterList = new List<CharacterBase>();

    // 現在マップ上に表示されている「移動可能マス」のリスト（選択解除や移動後に色を戻す用）
    private List<HexTileData> _movableTileList = new List<HexTileData>();

    private void Awake() {
        instance = this;
    }

    /// <summary>
    /// 動かす対象キャラの追加（プレイヤー・敵共通）
    /// </summary>
    public void AddMoveCharacter(CharacterBase character) {
        if(character == null) return;
        if(!_moveCharacterList.Contains(character)) {
            _moveCharacterList.Add(character);
        }
    }

    /// <summary>
    /// 移動可能なタイルの設定（主にプレイヤーのUI表示用）
    /// </summary>
    public void SetMovableTileList(List<HexTileData> _setMovableList) {
        _movableTileList = _setMovableList;
    }
    /// <summary>
    /// リストに登録されたキャラクターを順番に（非同期で）移動させる
    /// </summary>
    public async UniTask MoveCharacter() {
        if(CommonModule.IsEmpty(_moveCharacterList)) return;

        // 登録されているキャラを1体ずつ順番に動かす（敵のターンなどで順に動かしたい場合に最適）
        for(int i = 0, max = _moveCharacterList.Count; i < max; i++) {
            CharacterBase chara = _moveCharacterList[i];
            if(chara == null || CommonModule.IsEmpty(chara.currentMoveRoute)) continue;

            // キャラ自身が持っているルートを渡して、移動完了を待つ
            await chara.MoveAsync(chara.currentMoveRoute);
        }

        // 全員の移動が完了したら後片付け
        ResetMoveCharacter();
        ResetMovableTile();
    }

    /// <summary>
    /// 移動キャラリストのリセットと終了処理
    /// </summary>
    public void ResetMoveCharacter() {
        if(CommonModule.IsEmpty(_moveCharacterList)) return;

        foreach(var chara in _moveCharacterList) {
            if(chara == null) continue;
            // キャラクター側の移動終了処理
            chara.EndMove();
        }
        // マネージャー側のリストをクリアして次のターン/アクションに備える
        _moveCharacterList.Clear();
    }
    /// <summary>
    /// 移動可能マスのリセット
    /// </summary>
    public void ResetMovableTile() {
        if(CommonModule.IsEmpty(_movableTileList)) return;

        foreach(var tile in _movableTileList) {
            if(tile == null) continue;
            tile.SetTileState(eTileState.Normal);
        }
        _movableTileList.Clear();
    }
    /// <summary>
    /// 現在選択されている（リストの先頭の）キャラクターを取得する
    /// </summary>
    public CharacterBase GetFirstMoveCharacter() {
        if(CommonModule.IsEmpty(_moveCharacterList)) return null;

        return _moveCharacterList[0];
    }
    /// <summary>
    /// 指定されたタイルが、現在提示されている移動可能範囲内かどうかを判定する
    /// </summary>
    public bool IsMovableTile(HexTileData tile) {
        if(tile == null) return false;
        return _movableTileList.Contains(tile);
    }
}