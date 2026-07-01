using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileManager : MonoBehaviour {
    public static HexTileManager instance { get; private set; } = null;

    private List<HexTileData> _tileDataList = null;
    private List<HexTileObject> _tileObjectList = null;

    private List<HexAreaData> _areaDataList = null;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize() {
        instance = this;
        // マスの生成

        // 部屋の生成
    }
    /// <summary>
    /// IDから2次元座標に変換
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void GetTilePosition(int ID, out int x, out int y) {
        // ここでの2次元座標に関しては3次元を疑似的な2次元整数座標で置き換えている
        x = -1;
        y = -1;
    }
    /// <summary>
    /// 2次元座標からIDに変換
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int GetTileID(int x, int y) {
        // ここでの2次元座標に関しては3次元を疑似的な2次元整数座標で置き換えている
        return -1;
    }
    /// <summary>
    /// ID指定のタイル情報取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public HexTileData GetHexTileData(int ID) {
        // TODO: OutofIndex対策をする
        return _tileDataList[ID];
    }
    /// <summary>
    /// ID指定のタイルオブジェクト取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public HexTileObject GetHexTileObject(int ID) {
        // TODO: OutofIndex対策をする
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
        return GetHexTileData(tileID);
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
            case eDirectionHex.UpLeft:      // 左下
                x--;
                y++;
            break;
        }
    }
}
