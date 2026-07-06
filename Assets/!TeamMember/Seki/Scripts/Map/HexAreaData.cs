using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexAreaData {
    // エリアID
    public int areaID { get; private set; } = -1;
    // エリアのX座標
    public int areaGridX { get; private set; } = -1;
    // エリアのY座標
    public int areaGridY { get; private set; } = -1;
    // バイオーム
    public eBiome biome { get; private set; } = eBiome.None;
    // エリア制圧フラグ
    public bool isSubjugationArea { get; private set; } = false;
    // エリア内のマスIDリスト
    public List<int> tileIDList { get; private set; } = null;

    /// <summary>
    /// セットアップ処理
    /// </summary>
    /// <param name="setAreaID"></param>
    /// <param name="setBiome"></param>
    /// <param name="setTileIDList"></param>
    public void Setup(int setAreaID, int setAreaX, int setAreaY, eBiome setBiome, List<int> setTileIDList) {
        areaID = setAreaID;
        areaGridX = setAreaX; 
        areaGridY = setAreaY;
        biome = setBiome;
        tileIDList = setTileIDList;
        // タイルのエリアID, バイオームの設定
        for(int i = 0, max = tileIDList.Count; i < max; i++) {
            HexTileData hexTile = HexTileManager.instance.GetTileData(tileIDList[i]);
            if(hexTile == null) continue;

            hexTile.SetAreaID(areaID);
            hexTile.SetBiome(biome);
        }
    }
}
