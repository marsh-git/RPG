using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexPathfinding
{
    // 軸座標における隣接6方向のオフセット
    private static readonly Vector2Int[] HexDirections = new Vector2Int[]
    {
        new Vector2Int(1, 0),   // 右
        new Vector2Int(0, 1),   // 右斜め上
        new Vector2Int(-1, 1),  // 左斜め上
        new Vector2Int(-1, 0),  // 左
        new Vector2Int(0, -1),  // 左斜め下
        new Vector2Int(1, -1)   // 右斜め下
    };

    // A*アルゴリズム本体：スタートからゴールまでのタイルのリストを返す
    public static List<HexTile> FindPath(HexTile start, HexTile goal)
    {
        if (start == null || goal == null) return null;
        if (goal.terrainType == TerrainType.Mountain) return null; // 山岳は目的地にできない

        // 探索用データ構造
        List<HexTile> openSet = new List<HexTile> { start };
        HashSet<HexTile> closedSet = new HashSet<HexTile>();

        Dictionary<HexTile, HexTile> cameFrom = new Dictionary<HexTile, HexTile>();
        Dictionary<HexTile, int> gScore = new Dictionary<HexTile, int> { [start] = 0 };
        Dictionary<HexTile, int> fScore = new Dictionary<HexTile, int> { [start] = HeuristicDistance(start, goal) };

        while (openSet.Count > 0)
        {
            // openSetの中で最もfScore（トータル予測コスト）が低いタイルを選ぶ
            HexTile current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (fScore.ContainsKey(openSet[i]) && fScore[openSet[i]] < fScore[current])
                {
                    current = openSet[i];
                }
            }

            // ゴールに到達した場合、経路を復元して返す
            if (current == goal)
            {
                return RetracePath(start, goal, cameFrom);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            // 隣接する6マスを調査
            foreach (Vector2Int dir in HexDirections)
            {
                Vector2Int neighborCoord = current.axialCoordinate + dir;
                HexTile neighbor = HexGridGenerator.Instance.GetTileAt(neighborCoord);

                if (neighbor == null || closedSet.Contains(neighbor)) continue;
                if (neighbor.terrainType == TerrainType.Mountain) continue; // 山は通れない

                // 移動コストの計算 (現在のコスト + 移動先のタイルの固有コスト)
                int tentativeGScore = gScore[current] + neighbor.MovementCost;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + HeuristicDistance(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // 経路が見つからなかった場合
    }

    // キューブ/軸座標系における2点間の直線距離（マンハッタン距離のHex版）
    private static int HeuristicDistance(HexTile a, HexTile b)
    {
        Vector2Int coordA = a.axialCoordinate;
        Vector2Int coordB = b.axialCoordinate;

        int az = -coordA.x - coordA.y;
        int bz = -coordB.x - coordB.y;

        return (Mathf.Abs(coordA.x - coordB.x) + Mathf.Abs(coordA.y - coordB.y) + Mathf.Abs(az - bz)) / 2;
    }

    // ゴールから親を辿ってスタートまでの経路を逆転させてリスト化する
    private static List<HexTile> RetracePath(HexTile start, HexTile goal, Dictionary<HexTile, HexTile> cameFrom)
    {
        List<HexTile> path = new List<HexTile>();
        HexTile current = goal;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse(); // スタート→ゴールの順にする
        return path;
    }
    public static HashSet<HexTile> CalculateMovementRange(HexTile start, int maxCost)
    {
        HashSet<HexTile> reachableTiles = new HashSet<HexTile>();
        Dictionary<HexTile, int> costSoFar = new Dictionary<HexTile, int>();
        Queue<HexTile> frontier = new Queue<HexTile>();

        // スタート地点を初期化
        frontier.Enqueue(start);
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            HexTile current = frontier.Dequeue();
            int currentCost = costSoFar[current];

            foreach (Vector2Int dir in HexDirections)
            {
                Vector2Int neighborCoord = current.axialCoordinate + dir;
                HexTile neighbor = HexGridGenerator.Instance.GetTileAt(neighborCoord);

                // マップ外、または山岳なら進入不可
                if (neighbor == null || neighbor.terrainType == TerrainType.Mountain) continue;

                // 移動先タイルのコストを加算
                int newCost = currentCost + neighbor.MovementCost;

                // ★★【絶対のブレーキ】計算されたコストが出目（maxCost）を超えるなら、絶対にそれ以上進ませない★★
                if (newCost > maxCost)
                {
                    continue; // Queueへの追加も、候補リストへの追加も即座に完全遮断
                }

                // 出目の範囲内に収まる場合のみ、最短ルートを記録して登録
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    frontier.Enqueue(neighbor);
                    reachableTiles.Add(neighbor);
                }
            }
        }

        return reachableTiles;
    }
}