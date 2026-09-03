using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HexTileObject : MonoBehaviour, IClickable {
    public int ID { get; private set; } = -1;

    [Header("ハイライトの見た目")]
    [SerializeField] private GameObject[] _highlightEffectList = null;

    [Header("コンポーネント参照")]
    [SerializeField] private MeshRenderer _tileMeshRenderer = null;
    [SerializeField] private Transform _terrainRoot = null;
    [SerializeField] private Transform _attributeRoot = null;

    private GameObject _terrainObject = null;
    private GameObject _attributeObject = null;

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
    /// タイルデータの取得
    /// </summary>
    /// <returns></returns>
    public HexTileData GetTileData() {
        HexTileData tileData = HexTileManager.instance.GetTileData(ID);
        if(tileData == null) return null;

        return tileData;
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
    /// 見た目の設定
    /// </summary>
    /// <param name="data"></param>
    public void RefreshVisuals(GameObject terrainObject, GameObject attributeObject, Material terrainMaterial) {
        // 地形マテリアルの適応
        if(terrainMaterial != null) _tileMeshRenderer.material = terrainMaterial;
        // 既存オブジェクトのクリア
        ClearDecorations();
        // 地形オブジェクトの生成
        if(terrainObject != null) _terrainObject = Instantiate(terrainObject, _terrainRoot);
        // 属性オブジェクトの生成
        if(attributeObject != null)  _attributeObject = Instantiate(attributeObject, _attributeRoot);
    }
    /// <summary>
    /// 見た目オブジェクトの削除
    /// </summary>
    public void ClearDecorations() {
        if(_terrainObject != null) Destroy(_terrainObject);
        if(_attributeObject != null) Destroy(_attributeObject);
    }
    /// <summary>
    /// クリックされたときの処理
    /// </summary>
    public void OnClick() {
        // プレイヤーターン以外は操作禁止
        if (!TurnManager.Instance.IsPlayerTurn()) return;
        
        var ClickableHighlight = ClickableSelectionManager.instance;
        var MovementManager = CharacterMovementManager.instance;
        var HexManager = HexTileManager.instance;
        // タイルデータを取得
        HexTileData targetTile = HexManager.GetTileData(ID);
        switch(targetTile.tileState) {
            case eTileMoveState.Normal:
            // 移動の片付け処理
            MovementManager.TeardownMovement();
            // クリック管理クラスに伝える
            ClickableHighlight.OnTileHighlight(targetTile);
            Debug.Log(targetTile.Attribute);
            break;
            case eTileMoveState.Movable:
            // 移動対象キャラクターの取得（プレイヤーの取得）
            CharacterBase selectChara = MovementManager.GetFirstMoveCharacter();
            if(selectChara == null) return;
            // 選択状態なら移動開始
            if(selectChara.isSelect) {
                // 開始地点（キャラクター）のタイル取得
                HexTileData startTile = HexManager.GetTileData(selectChara.GetTileID());
                // 移動ルートの決定
                List<HexTileData> route = HexRouteSearcher.FindPath(startTile, targetTile, selectChara.IsEnemy());
                // 現在のマスを通常マスに戻す
                startTile.SetTileState(eTileMoveState.Normal);
                // 移動先マスをキャラクター存在マスに変更
                targetTile.SetTileState(eTileMoveState.CharacterIn);
                // 移動ルートの設定
                selectChara.SetMoveRoute(route);
                // ハイライトの解除
                ClickableHighlight.ClearHighlights();
                // 移動開始
                MovePlayer().Forget();
            } else {
                // ハイライトの解除
                ClickableHighlight.ClearHighlights();
                // 移動の片付け処理
                MovementManager.TeardownMovement();
            }
            break;
            case eTileMoveState.CharacterIn:
            // 移動の片付け処理
            MovementManager.TeardownMovement();
            // クリック管理クラスに伝える
            ClickableHighlight.OnTileHighlight(targetTile);
            break;
        }
    }
    /// <summary>
    /// プレイヤーを移動させ、移動終了後にターンを終了する
    /// </summary>
    private async UniTaskVoid MovePlayer(){
        await CharacterMovementManager.instance.MoveCharacter();

        TurnManager.Instance.EndPlayerTurn();
    }
}