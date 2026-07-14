using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileManager : MonoBehaviour {
    public static HexTileManager instance { get; private set; } = null;

    private List<HexTileData> _tileDataList = new List<HexTileData>();
    private List<HexTileObject> _tileObjectList = new List<HexTileObject>();
    private List<HexAreaData> _areaDataList = new List<HexAreaData>();

    // TODO:そのうち、ゲームシーンステートクラスが持つようになる
    [SerializeField] private HexMapGenerator mapGenerator;

    private void Start()
    {
        // デバッグ用
        mapGenerator.CreateDebugMap();
    }

    public void Awake() {
        instance = this;
        // TODO : そのうち呼び出し場所を変える
        AttributeFactory.Initialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize() {
        // マスの生成

        // 部屋の生成

    }
    /// <summary>
    /// データとオブジェクトを一元管理に登録
    /// </summary>
    /// <param name="data"></param>
    /// <param name="tileObject"></param>
    public void AddTile(HexTileData data, HexTileObject tileObject) {
        if(data == null || tileObject == null) return;

        _tileDataList.Add(data);
        _tileObjectList.Add(tileObject);
    }
    /// <summary>
    /// エリアの追加
    /// </summary>
    /// <param name="area"></param>
    public void AddArea(HexAreaData area) { 
        _areaDataList.Add(area);
    }
    /// <summary>
    /// 座標指定のタイルID取得
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int GetTileID(int x, int y) {
        // リスト全体をループして、座標が一致するタイルを探す
        for(int i = 0, max = _tileDataList.Count; i < max; i++) {
            if(_tileDataList[i].gridPosX != x || _tileDataList[i].gridPosY != y) continue;
            // 一致したIDを返す
            return _tileDataList[i].ID;
        }
        return -1; // マップ外
    }
    /// <summary>
    /// ID指定のタイル情報取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public HexTileData GetTileData(int ID) {
        if(!CommonModule.IsEnableIndex(_tileDataList, ID)) return null;
        return _tileDataList[ID];
    }
    /// <summary>
    /// ID指定のタイルオブジェクト取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public HexTileObject GetTileObject(int ID) {
        if(!CommonModule.IsEnableIndex(_tileDataList, ID)) return null;
        return _tileObjectList[ID];
    }
    /// <summary>
    /// 座標指定のタイル情報取得
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public HexTileData GetTileData(int x, int y) {
        int tileID = GetTileID(x, y);
        return GetTileData(tileID);
    }
    /// <summary>
    /// 指定方向に隣接した座標のタイル取得
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public HexTileData GetToDirTile(int x, int y, eDirectionHex dir) {
        // 隣接タイルの取得
        ToDirPos(ref x, ref y, dir);
        return GetTileData(x, y);
    }
    /// <summary>
    /// 指定方向に隣接した座標取得
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="dir"></param>
    private void ToDirPos(ref int x, ref int y, eDirectionHex dir) {
        switch(dir) {
            case eDirectionHex.UpRight:     // 右上
                y++;
            break;
            case eDirectionHex.Right:       // 右
                x++;
            break;
            case eDirectionHex.DownRight:   // 右下
                x++;
                y--;
            break;
            case eDirectionHex.DownLeft:    // 左下
                y--;
            break;
            case eDirectionHex.Left:        // 左
                x--;
            break;
            case eDirectionHex.UpLeft:      // 左上
                x--;
                y++;
            break;
        }
    }
}
