using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileManager : MonoBehaviour {
    public static HexTileManager instance { get; private set; } = null;

    private List<HexTileData> _tileDataList = new List<HexTileData>();
    private List<HexTileObject> _tileObjectList = new List<HexTileObject>();
    private List<HexAreaData> _areaDataList = new List<HexAreaData>();
    private Dictionary<Vector2Int, int> _coordToIdMap = new Dictionary<Vector2Int, int>();

    // TODO:そのうち、ゲームシーンステートクラスが持つようになる
    [SerializeField] private HexMapGenerator mapGenerator;

    public void Awake() {
        instance = this;
        // デバッグ用
        mapGenerator.CreateDebugMap();
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

        // 座標からIDを引けるように逆引き辞書に登録
        Vector2Int coord = new Vector2Int(data.gridPosX, data.gridPosY);
        _coordToIdMap[coord] = data.ID;
    }
    /// <summary>
    /// エリアの追加
    /// </summary>
    /// <param name="area"></param>
    public void AddArea(HexAreaData area) { 
        _areaDataList.Add(area);
    }
    /// <summary>
    /// IDから2次元座標に変換
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void GetTilePos(int ID, out int x, out int y) {
        HexTileData data = GetTileData(ID);
        if(data != null) {
            x = data.gridPosX;
            y = data.gridPosY;
        } else {
            x = -1;
            y = -1;
        }
    }
    /// <summary>
    /// 2次元座標からIDに変換
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int GetTileID(int x, int y) {
        Vector2Int coord = new Vector2Int(x, y);
        if(_coordToIdMap.TryGetValue(coord, out int id)) return id;

        return -1; // 存在しないマップ外の座標
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
    public HexTileData GetHexTileData(int x, int y) {
        // タイルIDを取得
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
        return GetHexTileData(x, y);
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
