using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileRangeExpansion {
    // 時計回りの方向リスト
    private static readonly eDirectionHex[] Directions = new eDirectionHex[] {
        eDirectionHex.UpRight, eDirectionHex.Right, eDirectionHex.DownRight,
        eDirectionHex.DownLeft,  eDirectionHex.Left,  eDirectionHex.UpLeft
    };
    /// <summary>
    /// 指定回数分の指定方向範囲内のタイル取得
    /// ※startTile自体は含めない
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
    /// 1マス目：1マス
    /// 2マス目：3マス
    /// 3マス目：5マス
    /// </summary>
    public static List<HexTileData> GetFanShapedTile(
        HexTileData startTile,
        eDirectionHex dir,
        int range) {
        List<HexTileData> rangeTileList = new List<HexTileData>();

        if (startTile == null ||
            dir == eDirectionHex.Invalid ||
            range <= 0) {
            return rangeTileList;
        }

        eDirectionHex left = dir.GetLeftDir();
        eDirectionHex right = dir.GetRightDir();

        for (int distance = 1; distance <= range; distance++) {
            // 自分から見て正面にあるタイル
            List<HexTileData> forwardTiles =
                GetNumDirTile(startTile, dir, distance);

            if (forwardTiles.Count == 0)
                continue;

            HexTileData centerTile =
                forwardTiles[forwardTiles.Count - 1];

            AddTile(rangeTileList, centerTile);

            // 正面タイルから左右に広げる
            HexTileData leftTile = centerTile;
            HexTileData rightTile = centerTile;

            for (int width = 1; width < distance; width++) {
                leftTile = GetToDirection(leftTile, left);
                rightTile = GetToDirection(rightTile, right);

                AddTile(rangeTileList, leftTile);
                AddTile(rangeTileList, rightTile);
            }
        }

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
                neighbor.Attribute == eAttribute.CannotMove ||
                neighbor.tileState == eTileMoveState.CharacterIn ||
                neighbor.tileState == eTileMoveState.Reserved
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
    /// <param name="centerTile">基準となるタイル</param>
    /// <param name="radius">半径</param>
    /// <returns></returns>
    public static List<HexTileData> GetTilesAtRadius(HexTileData centerTile, int radius) {
        List<HexTileData> ringTileList = new List<HexTileData>();
        if(centerTile == null || radius <= 0) return ringTileList;

        // 数学的にスタート地点のアキシアル座標(q, r)を計算
        // GetTilesWithinRadius のループ上限・下限計算ロジックを応用し、DownLeft方向の最下端座標を算出
        int currentQ = centerTile.gridPosX;
        int currentR = centerTile.gridPosY + radius;

        // スタート地点から、6つの方向を順番に変えながら「半径と同じ歩数」ずつ進むことで綺麗な正六角形の環を一周する
        // 走査順：UpRight -> Right -> DownRight -> DownLeft -> Left -> UpLeft
        foreach(eDirectionHex dir in Directions) {
            for(int i = 0; i < radius; i++) {
                // 座標から直接タイルデータを逆引きすることで、マップ外(null)を跨いだ正しいリング走査を保証
                HexTileData currentTile = HexTileManager.instance.GetTileData(currentQ, currentR);
                if(currentTile != null) {
                    ringTileList.Add(currentTile);
                }

                // マップの形状に関わらず正しく次の座標を追跡するため、
                // 隣接マスのデータ有無にかかわらず、次の方向の相対座標オフセット（位置関係）を計算に適用
                HexTileData peekTile = HexTileManager.instance.GetToDirTile(currentQ, currentR, dir);
                if(peekTile != null) {
                    currentQ = peekTile.gridPosX;
                    currentR = peekTile.gridPosY;
                } else {
                    // 万が一マップ外(null)に進む場合は、これまでの方向性（進行ベクトル）を維持して座標のみを進める
                    // ※アキシアル座標系における各方向の増分(dq, dr)を適用
                    switch(dir) {
                        case eDirectionHex.UpRight: currentQ += 1; currentR -= 1; break;
                        case eDirectionHex.Right: currentQ += 1; break;
                        case eDirectionHex.DownRight: currentR += 1; break;
                        case eDirectionHex.DownLeft: currentQ -= 1; currentR += 1; break;
                        case eDirectionHex.Left: currentQ -= 1; break;
                        case eDirectionHex.UpLeft: currentR -= 1; break;
                    }
                }
            }
        }

        return ringTileList;
    }
    /// <summary>
    /// 中心タイルから指定された半径「以内」にあるすべてのマスのリストを取得する
    /// ※中心タイル自体は含めない
    /// </summary>
    /// <param name="centerTile">基準となる中心のタイル</param>
    /// <param name="radius">探索半径（1以上を指定、0以下の場合は空のリストを返す）</param>
    /// <returns>半径内に存在する全タイルのリスト（中心除く）</returns>
    public static List<HexTileData> GetTilesWithinRadius(HexTileData centerTile, int radius) {
        if(centerTile == null || radius <= 0) return new List<HexTileData>();

        // 中心を除外するため、初期容量から1を引く
        int estimatedSize = 3 * radius * (radius + 1);
        List<HexTileData> allTilesInRadius = new List<HexTileData>(estimatedSize);

        // 中心座標を起点とした相対的なアキシアル座標の範囲ループ
        for(int q = -radius; q <= radius; q++) {
            int rStart = Mathf.Max(-radius, -q - radius);
            int rEnd = Mathf.Min(radius, -q + radius);

            for(int r = rStart; r <= rEnd; r++) {
                // 中心マス場合はリストに加えずスキップ
                if(q == 0 && r == 0) continue;

                // 中心タイルの絶対座標に相対オフセットを加算して対象座標を特定
                int targetQ = centerTile.gridPosX + q;
                int targetR = centerTile.gridPosY + r;

                // マネージャーから該当する座標のタイルデータを取得
                HexTileData tileData = HexTileManager.instance.GetTileData(targetQ, targetR);
                if(tileData != null) {
                    allTilesInRadius.Add(tileData);
                }
            }
        }
        return allTilesInRadius;
    }
    /// <summary>
    /// 範囲内のタイルの中から有効なタイルのみを返す
    /// </summary>
    /// <param name="cneterTile"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static List<HexTileData> GetValidTiles(HexTileData cneterTile, int radius) {
        // 範囲内のタイルリストの取得
        List<HexTileData> radiusTileList = GetTilesWithinRadius(cneterTile, radius);
        // 有効なマス以外を除外
        radiusTileList.RemoveAll(neighbor =>
            neighbor == null ||
            neighbor.Attribute == eAttribute.CannotMove ||
            neighbor.BuildingType != eBuildingType.Invalid ||
            neighbor.tileState == eTileMoveState.CharacterIn ||
            neighbor.tileState == eTileMoveState.Reserved
        );
        return radiusTileList;
    }
    /// <summary>
    /// アキシアル座標系における2点間の幾何学的なヘックス距離を算出するヘルパー関数
    /// </summary>
    private static int CalculateHexDistance(int q1, int r1, int q2, int r2) {
        int dq = q1 - q2;
        int dr = r1 - r2;
        return (Mathf.Abs(dq) + Mathf.Abs(dr) + Mathf.Abs(dq + dr)) / 2;
    }


    //古谷追加物

    private static HexTileData GetToDirection(
     HexTileData tile,
     eDirectionHex dir) {
        if (tile == null)
            return null;

        return HexTileManager.instance.GetToDirTile(
            tile.gridPosX,
            tile.gridPosY,
            dir
        );
    }

    private static void AddTile(
        List<HexTileData> tileList,
        HexTileData tile) {
        if (tile != null && !tileList.Contains(tile)) {
            tileList.Add(tile);
        }
    }
}