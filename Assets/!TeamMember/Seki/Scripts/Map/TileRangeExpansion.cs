using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileRangeExpansion{
    /// <summary>
    /// 指定回数分の指定方向範囲内のタイル取得
    /// startTile自体は含めない
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="dir"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<HexTileData> GetNumDirTile(this HexTileData startTile, eDirectionHex dir, int count) {
        if(startTile == null || dir == eDirectionHex.Invalid || count < 0) return new List<HexTileData>();
        List<HexTileData> rangeTileList = new List<HexTileData>(count);
        for(int i = 0; i < count; i++) {
            HexTileData tileData = HexTileManager.instance.GetToDirTile(startTile.gridPosX, startTile.gridPosY, dir);
            if(tileData == null) continue;

            rangeTileList.Add(tileData);
            startTile = tileData;
        }
        return rangeTileList;
    }
    /// <summary>
    /// 指定方向の扇形範囲内のタイル取得
    /// ※startTile自体は含めない
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static List<HexTileData> GetFanShapedTile(this HexTileData startTile, eDirectionHex dir) {
        if(startTile == null || dir == eDirectionHex.Invalid) return new List<HexTileData>();
        List<HexTileData> rangeTileList = new List<HexTileData>(3);
        // 方向の取得
        eDirectionHex forward = dir;
        eDirectionHex left = dir.GetLeftDir();
        eDirectionHex right = dir.GetRightDir();

        // 前方タイルの取得
        HexTileData forwardTile = HexTileManager.instance.GetToDirTile(startTile.gridPosX, startTile.gridPosY, forward);
        if(forwardTile != null) rangeTileList.Add(forwardTile);
        // 左タイルの取得
        HexTileData leftTile = HexTileManager.instance.GetToDirTile(startTile.gridPosX, startTile.gridPosY, left);
        if(leftTile != null) rangeTileList.Add(leftTile);
        // 右タイルの取得
        HexTileData rightTile = HexTileManager.instance.GetToDirTile(startTile.gridPosX, startTile.gridPosY, right);
        if(rightTile != null) rangeTileList.Add(rightTile);

        return rangeTileList;
    }
}
