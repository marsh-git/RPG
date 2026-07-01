using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexPathfinding {
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

    // ★★ 引数に HexUnit movingUnit を追加 ★★
    public static List<HexTile> FindPath(HexTile start, HexTile goal, HexUnit movingUnit) {
        if(!start || !goal || !movingUnit) return null;
        if(goal.terrainType == TerrainType.Mountain) return null;

        List<HexTile> openSet = new List<HexTile> { start };
        HashSet<HexTile> closedSet = new HashSet<HexTile>();

        Dictionary<HexTile, HexTile> cameFrom = new Dictionary<HexTile, HexTile>();
        Dictionary<HexTile, int> gScore = new Dictionary<HexTile, int> { [start] = 0 };
        Dictionary<HexTile, int> fScore = new Dictionary<HexTile, int> { [start] = HeuristicDistance(start, goal) };

        while(openSet.Count > 0) {
            HexTile current = openSet[0];
            for(int i = 1; i < openSet.Count; i++) {
                if(fScore.ContainsKey(openSet[i]) && fScore[openSet[i]] < fScore[current]) {
                    current = openSet[i];
                }
            }

            if(current == goal) return RetracePath(start, goal, cameFrom);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach(Vector2Int dir in HexDirections) {
                Vector2Int neighborCoord = current.axialCoordinate + dir;
                HexTile neighbor = HexGridGenerator.Instance.GetTileAt(neighborCoord);

                if(!neighbor || closedSet.Contains(neighbor)) continue;
                if(neighbor.terrainType == TerrainType.Mountain) continue;

                // ★★ 移動経路計算でも被りルールを完全に同期 ★★
                if(neighbor != start) {
                    if(movingUnit.isEnemy) {
                        if(neighbor.HasPlayer || neighbor.HasEnemy) continue;
                    } else {
                        if(neighbor.HasEnemy) continue;
                    }
                }

                int tentativeGScore = gScore[current] + neighbor.MovementCost;

                if(!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor]) {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + HeuristicDistance(neighbor, goal);

                    if(!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    // ★★ 引数に HexUnit movingUnit を追加してエラーを解消 ★★
    public static HashSet<HexTile> CalculateMovementRange(HexTile start, int maxCost, HexUnit movingUnit) {
        HashSet<HexTile> reachableTiles = new HashSet<HexTile>();
        Dictionary<HexTile, int> costSoFar = new Dictionary<HexTile, int>();
        Queue<HexTile> frontier = new Queue<HexTile>();

        if(!start || !movingUnit) return reachableTiles;

        frontier.Enqueue(start);
        costSoFar[start] = 0;

        while(frontier.Count > 0) {
            HexTile current = frontier.Dequeue();
            int currentCost = costSoFar[current];

            foreach(Vector2Int dir in HexDirections) {
                Vector2Int neighborCoord = current.axialCoordinate + dir;
                HexTile neighbor = HexGridGenerator.Instance.GetTileAt(neighborCoord);

                if(neighbor == null || neighbor.terrainType == TerrainType.Mountain) continue;

                // 核心部分：movingUnit を参照可能に修正
                if(neighbor != start) {
                    if(movingUnit.isEnemy) {
                        // 敵：プレイヤーがいても、他の敵がいても進入不可
                        if(neighbor.HasPlayer || neighbor.HasEnemy) continue;
                    } else {
                        // プレイヤー：敵がいるマスだけは進入不可（味方プレイヤーとの重複はOK）
                        if(neighbor.HasEnemy) continue;
                    }
                }

                int newCost = currentCost + neighbor.MovementCost;

                if(newCost > maxCost) continue;

                if(!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) {
                    costSoFar[neighbor] = newCost;
                    frontier.Enqueue(neighbor);
                    reachableTiles.Add(neighbor);
                }
            }
        }

        return reachableTiles;
    }

    private static int HeuristicDistance(HexTile a, HexTile b) {
        Vector2Int coordA = a.axialCoordinate;
        Vector2Int coordB = b.axialCoordinate;
        int az = -coordA.x - coordA.y;
        int bz = -coordB.x - coordB.y;
        return (Mathf.Abs(coordA.x - coordB.x) + Mathf.Abs(coordA.y - coordB.y) + Mathf.Abs(az - bz)) / 2;
    }

    private static List<HexTile> RetracePath(HexTile start, HexTile goal, Dictionary<HexTile, HexTile> cameFrom) {
        List<HexTile> path = new List<HexTile>();
        HexTile current = goal;
        while(current != start) {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }
}