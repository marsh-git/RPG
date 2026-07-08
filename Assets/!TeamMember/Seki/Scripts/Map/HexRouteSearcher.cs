using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexRouteSearcher {

    // 方向列挙型（Invalid, Maxを除く有効な6方向）
    private static readonly eDirectionHex[] Directions = new eDirectionHex[] {
        eDirectionHex.UpRight,
        eDirectionHex.Right,
        eDirectionHex.DownRight,
        eDirectionHex.DownLeft,
        eDirectionHex.Left,
        eDirectionHex.UpLeft
    };

    /// <summary>
    /// A*アルゴリズム：スタートからゴールまでのタイルデータリストを返す
    /// </summary>
    public static List<HexTileData> FindPath(HexTileData start, HexTileData goal, bool isEnemy) {
        if(start == null || goal == null) return null;
        if(goal.terrain == eTerrain.Mountain || goal.attribute == eAttribute.CannotMove) return null;

        List<HexTileData> openSet = new List<HexTileData> { start };
        HashSet<HexTileData> closedSet = new HashSet<HexTileData>();

        Dictionary<HexTileData, HexTileData> cameFrom = new Dictionary<HexTileData, HexTileData>();
        Dictionary<HexTileData, int> gScore = new Dictionary<HexTileData, int> { [start] = 0 };
        Dictionary<HexTileData, int> fScore = new Dictionary<HexTileData, int> { [start] = HeuristicDistance(start, goal) };

        while(openSet.Count > 0) {
            HexTileData current = openSet[0];
            for(int i = 1; i < openSet.Count; i++) {
                if(fScore.ContainsKey(openSet[i]) && fScore[openSet[i]] < fScore[current]) {
                    current = openSet[i];
                }
            }

            if(current == goal) return RetracePath(start, goal, cameFrom);

            openSet.Remove(current);
            closedSet.Add(current);

            // 6方向の隣接マスを調査
            foreach(eDirectionHex dir in Directions) {
                HexTileData neighbor = HexTileManager.instance.GetToDirTile(current.gridPosX, current.gridPosY, dir);

                // マップ外、判定済、山岳、進行不可属性はスキップ
                if(neighbor == null || closedSet.Contains(neighbor)) continue;
                if(neighbor.terrain == eTerrain.Mountain || neighbor.attribute == eAttribute.CannotMove) continue;

                int movementCost = (int)neighbor.GetMovementCost();
                if(movementCost < 0) continue; // Invalid等の安全弁

                int tentativeGScore = gScore[current] + movementCost;

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

    /// <summary>
    /// ダイクストラ法：移動可能範囲のタイルデータ群を返す
    /// </summary>
    /// <param name="start"></param>
    /// <param name="maxCost"></param>
    /// <param name="isEnemy"></param>
    /// <returns></returns>
    public static HashSet<HexTileData> CalculateMovementRange(HexTileData start, int maxCost, bool isEnemy) {
        HashSet<HexTileData> reachableTiles = new HashSet<HexTileData>();
        Dictionary<HexTileData, int> costSoFar = new Dictionary<HexTileData, int>();
        Queue<HexTileData> frontier = new Queue<HexTileData>();

        if(start == null) return reachableTiles;

        frontier.Enqueue(start);
        costSoFar[start] = 0;

        while(frontier.Count > 0) {
            HexTileData current = frontier.Dequeue();
            int currentCost = costSoFar[current];

            foreach(eDirectionHex dir in Directions) {
                HexTileData neighbor = HexTileManager.instance.GetToDirTile(current.gridPosX, current.gridPosY, dir);

                if(neighbor == null || neighbor.terrain == eTerrain.Mountain || neighbor.attribute == eAttribute.CannotMove) continue;

                int movementCost = (int)neighbor.GetMovementCost();
                if(movementCost < 0) continue;

                int newCost = currentCost + movementCost;
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
    /// <summary>
    /// アキシアル座標系における直線ステップ数計算
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    private static int HeuristicDistance(HexTileData start, HexTileData goal) {
        int az = -start.gridPosX - start.gridPosY;
        int bz = -goal.gridPosX - goal.gridPosY;
        return (Mathf.Abs(start.gridPosX - goal.gridPosX) + Mathf.Abs(start.gridPosY - goal.gridPosY) + Mathf.Abs(az - bz)) / 2;
    }
    /// <summary>
    /// A* 探索の結果から、ゴールからスタートへ向かう親ノードのリンクを逆引きし、
    /// 正しい移動順（スタートの隣接マス ～ ゴール）の経路リストに復元・反転
    /// /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <param name="cameFrom"></param>
    /// <returns></returns>
    private static List<HexTileData> RetracePath(HexTileData start, HexTileData goal, Dictionary<HexTileData, HexTileData> cameFrom) {
        List<HexTileData> path = new List<HexTileData>();
        HexTileData current = goal;
        while(current != start) {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }
}