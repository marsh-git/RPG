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
        // 最終的に戻す「移動可能なタイル」を格納するハッシュセット
        HashSet<HexTileData> reachableTiles = new HashSet<HexTileData>();

        // 各タイルに到達するまでにかかった「最小累積コスト」を記録する辞書
        Dictionary<HexTileData, int> costSoFar = new Dictionary<HexTileData, int>();

        // 次に探索するべきタイルを入れておくキュー（探索のフロントライン）
        Queue<HexTileData> frontier = new Queue<HexTileData>();

        // スタート地点がヌルなら空のリストを返す（防衛策）
        if(start == null) return reachableTiles;

        // 初期化：スタート地点を探索キューに入れ、そのコストを0に設定
        frontier.Enqueue(start);
        costSoFar[start] = 0;

        // 探索すべきタイルがある限りループを回す
        while(frontier.Count > 0) {
            // キューから現在調査するタイルを1つ取り出す
            HexTileData current = frontier.Dequeue();
            int currentCost = costSoFar[current];

            // 現在のタイルから周囲6方向（Hexの隣接方向）を順に調べる
            foreach(eDirectionHex dir in Directions) {
                // 隣接するタイルのデータをマネージャーから取得
                HexTileData neighbor = HexTileManager.instance.GetToDirTile(current.gridPosX, current.gridPosY, dir);

                // 隣が存在しない、または「山」、または「移動不可属性」ならスキップ
                if(neighbor == null || neighbor.terrain == eTerrain.Mountain || neighbor.attribute == eAttribute.CannotMove) continue;

                // 隣マスの地形に応じた移動コストを取得
                int movementCost = (int)neighbor.GetMovementCost();
                // コストがマイナス（通行不能設定など）ならスキップ
                if(movementCost < 0) continue;

                // スタート地点からその隣マスへ行くための「累積コスト」を計算
                int newCost = currentCost + movementCost;

                // 累積コストが最大移動力を超えてしまうならスキップ
                if(newCost > maxCost) continue;

                // 移動可能判定されたマスの処理
                // まだその隣マスに到達したことがない、または、以前調べたルートより少ないコストで到達できる場合
                if(!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) {
                    // そのマスの最小移動コストを更新・記録する
                    costSoFar[neighbor] = newCost;
                    // そのマスの先にも移動できる可能性があるので、次の探索キューに追加する
                    frontier.Enqueue(neighbor);
                    // 移動可能範囲のリスト（戻り値）にこのタイルを登録する
                    reachableTiles.Add(neighbor);
                    // タイル状態を選択可能にする
                    neighbor.SetTileState(eTileState.Movable);
                }
            }
        }
        // 最終的に溜まった移動可能なタイル群を返す
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