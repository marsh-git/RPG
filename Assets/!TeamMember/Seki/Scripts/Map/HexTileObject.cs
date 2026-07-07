using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HexTileObject : MonoBehaviour, IClickable {
    public int ID { get; private set; } = -1;

    [Header("Visuals")]
    [SerializeField] private GameObject highlightEffect;

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
    public void SetHighlight(bool isActive) {
        if(highlightEffect != null) highlightEffect.SetActive(isActive);
    }
    /// <summary>
    /// クリックされたときの処理
    /// </summary>
    public void OnClick() {
        // タイルデータを取得
        HexTileData data = HexTileManager.instance.GetTileData(ID);
        // クリック管理クラスに伝える
        ClickableSelectionManager.instance.OnTileClicked(data);
    }
}
