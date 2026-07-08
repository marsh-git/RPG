using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HexTileObject : MonoBehaviour, IClickable {
    public int ID { get; private set; } = -1;

    [Header("Visuals")]
    [SerializeField] private GameObject[] _highlightEffectList = null;

    /// <summary>
    /// 座標のセットアップ
    /// </summary>
    /// <param name="setPosition"></param>
    public void Setup(int setID, Vector3 setPosition) {
        ID = setID;
        Vector3 position = transform.position;
        position.x = setPosition.x;
        position.y = 0;
        position.z = setPosition.z;
        transform.position = position;
    }
    /// <summary>
    /// ハイライトの設定
    /// </summary>
    /// <param name="isActive"></param>
    public void SetHighlight(bool isActive, eTileHighlight setHighlight) {
        int highlightIndex = (int)setHighlight;
        if(!CommonModule.IsEnableIndex(_highlightEffectList, highlightIndex)) return;

        if(_highlightEffectList != null) _highlightEffectList[highlightIndex].SetActive(isActive);
    }
    /// <summary>
    /// クリックされたときの処理
    /// </summary>
    public void OnClick() {
        var ClickableHighlight = ClickableSelectionManager.instance;
        var MovementManager = CharacterMovementManager.instance;
        // タイルデータを取得
        HexTileData targetTile = HexTileManager.instance.GetTileData(ID);
        switch(targetTile.tileState) {
            case eTileState.Normal:
            // クリック管理クラスに伝える
            ClickableHighlight.OnTileHighlight(targetTile);
            break;
            case eTileState.Movable:
            // ハイライトの解除
            ClickableHighlight.ClearHighlights();
            // 移動ルートの決定
            MovementManager.DecideMoveRoute(targetTile);
            // 移動処理
            UniTask task = MovementManager.MoveCharacter();
            break;
            case eTileState.Selected:
            break;
        }
    }
}
