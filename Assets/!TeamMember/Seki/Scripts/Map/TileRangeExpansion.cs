using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileRangeExpansion{
    // 時計回りの方向リスト
    private static readonly eDirectionHex[] Directions = new eDirectionHex[] {
        eDirectionHex.UpRight, eDirectionHex.Right, eDirectionHex.DownRight,
        eDirectionHex.DownLeft,  eDirectionHex.Left,  eDirectionHex.UpLeft
    };
    /// <summary>
    /// 指定回数分の指定方向範囲内のタイル取得
    /// startTile自体は含めない
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="dir"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<HexTileData> GetNumDirTile(HexTileData startTile, eDirectionHex dir, int count) {
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
    public static List<HexTileData> GetFanShapedTile(HexTileData startTile, eDirectionHex dir) {
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
    /// <summary>
    /// 中心タイルの周囲から、有効なマスが見つかるまで外側へ探索半径を広げ続け、
    /// 条件を満たすマスが初めて見つかった距離（リング）内において、基準タイルに最も幾何学的距離が近いタイルを取得する。
    /// </summary>
    /// <param name="centerTile">中心となるタイル（例：ターゲットやプレイヤーがいるマス）</param>
    /// <param name="referenceTile">基準となるタイル（例：移動前の自身のマス）</param>
    /// <param name="maxRadius">無限探索を防ぐための最大半径（安全弁）</param>
    /// <returns>条件を満たす最も基準に近いタイル。最大半径まで見つからない場合はnull</returns>
    public static HexTileData GetClosestNeighborToReference(HexTileData centerTile, HexTileData referenceTile, int maxRadius = 10) {
        if(centerTile == null || referenceTile == null) return null;

        // 距離（半径）d = 1 から順に外側へ向かって探索を広げる
        for(int d = 1; d <= maxRadius; d++) {
            List<HexTileData> validTilesInRing = GetTilesAtRadius(centerTile, d);

            // このリング内に歩行可能かつキャラがいないマスがあるかフィルタリング
            validTilesInRing.RemoveAll(neighbor =>
                neighbor == null ||
                neighbor.attribute == eAttribute.CannotMove ||
                neighbor.tileState == eTileState.CharacterIn || 
                neighbor.tileState == eTileState.Reserved
            );

            // この距離のリング内に1つでも有効なマスが見つかった場合
            if(validTilesInRing.Count > 0) {
                HexTileData closestNeighbor = null;
                int minDistance = int.MaxValue;

                // 見つかった有効なマスの中から、基準タイルに最も近いものを決定論的に選択
                foreach(HexTileData neighbor in validTilesInRing) {
                    int distance = CalculateHexDistance(
                        neighbor.gridPosX, neighbor.gridPosY,
                        referenceTile.gridPosX, referenceTile.gridPosY
                    );

                    if(distance < minDistance) {
                        minDistance = distance;
                        closestNeighbor = neighbor;
                    }
                }

                // このリング内で最も基準に近いタイルを返す
                return closestNeighbor;
            }
        }

        // マップ全域が埋まっているなど、見つからなかった場合
        return null;
    }

    /// <summary>
    /// 中心タイルから指定距離の位置にあるタイルのリストを取得
    /// </summary>
    private static List<HexTileData> GetTilesAtRadius(HexTileData center, int radius) {
        List<HexTileData> ringTiles = new List<HexTileData>();

        // 中心から指定方向に「半径分の歩数」だけ進んだ地点（スタート地点）を算出
        HexTileData current = center;
        for(int i = 0; i < radius; i++) {
            current = HexTileManager.instance.GetToDirTile(current.gridPosX, current.gridPosY, eDirectionHex.DownLeft);
            if(current == null) break;
        }

        // スタート地点から、6つの方向を順番に変えながら「半径と同じ歩数」ずつ進むことで綺麗な正六角形の環を一周する
        // 走査順：UpRight -> Right -> DownRight -> DownLeft -> Left -> UpLeft
        foreach(eDirectionHex dir in Directions) {
            for(int i = 0; i < radius; i++) {
                if(current != null) {
                    ringTiles.Add(current);
                }
                current = HexTileManager.instance.GetToDirTile(current.gridPosX, current.gridPosY, dir);
            }
        }

        return ringTiles;
    }
    /// <summary>
    /// アキシアル座標系における2点間の幾何学的なヘックス距離を算出するヘルパー関数
    /// </summary>
    private static int CalculateHexDistance(int q1, int r1, int q2, int r2) {
        int dq = q1 - q2;
        int dr = r1 - r2;
        return (Mathf.Abs(dq) + Mathf.Abs(dr) + Mathf.Abs(dq + dr)) / 2;
    }
}
