using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ヘックスグリッドマップにおける経路探索および移動範囲計算を行う静的クラス
/// </summary>
public static class HexRouteSearcher {

    // アキシアル座標系におけるPointy-Toppedの隣接6方向（右上、右、右下、左下、左、左上）
    private static readonly eDirectionHex[] Directions = new eDirectionHex[] {
        eDirectionHex.UpRight, eDirectionHex.Right, eDirectionHex.DownRight,
        eDirectionHex.DownLeft,  eDirectionHex.Left,  eDirectionHex.UpLeft
    };

    /// <summary>
    /// A*（エースター）アルゴリズム：スタートからゴールまでの最適な経路を計算し、タイルデータのリストとして返す
    /// </summary>
    /// <param name="start">移動開始地点のタイルデータ</param>
    /// <param name="goal">目的地のタイルデータ</param>
    /// <param name="isEnemy">実行者が敵ユニットかどうか（拡張用パラメータ）</param>
    /// <returns>スタートの次マスからゴールまでの順テーラードな経路リスト。経路が存在しない場合はnull</returns>
    public static List<HexTileData> FindPath(HexTileData start, HexTileData goal, bool isEnemy) {
        // 事前バリデーション（境界条件の防衛）
        if(start == null || goal == null) return null;

        // ゴール地点そのものが進入不可能な状態（山岳・移動不可属性・別ユニットの存在）であれば即座に終了
        if(goal.terrain == eTerrain.Mountain ||
           goal.attribute == eAttribute.CannotMove ||
           goal.tileState == eTileState.CharacterIn) return null;

        // 探索候補ノード（オープンリスト）と探索完了ノード（クローズドリスト）の初期化
        List<HexTileData> openSet = new List<HexTileData> { start };
        HashSet<HexTileData> closedSet = new HashSet<HexTileData>();

        // 各ノードの親関係（経路復元用）およびコスト管理用辞書
        Dictionary<HexTileData, HexTileData> cameFrom = new Dictionary<HexTileData, HexTileData>();
        Dictionary<HexTileData, int> gScore = new Dictionary<HexTileData, int> { [start] = 0 };
        Dictionary<HexTileData, int> fScore = new Dictionary<HexTileData, int> { [start] = HeuristicDistance(start, goal) };

        // オープンリストが空になるまで探索ループを継続
        while(openSet.Count > 0) {
            // オープンリストの中から総推定コスト（fScore）が最も低いノードを現在の調査対象に選択
            HexTileData current = openSet[0];
            for(int i = 1; i < openSet.Count; i++) {
                if(fScore.ContainsKey(openSet[i]) && fScore[openSet[i]] < fScore[current]) {
                    current = openSet[i];
                }
            }

            // ゴールに到達した場合、親ノードのリンクを逆引きして経路リストを構築・返却
            if(current == goal) return RetracePath(start, goal, cameFrom);

            // 現在のノードをオープンリストから除外し、探索完了としてクローズドリストへ追加
            openSet.Remove(current);
            closedSet.Add(current);

            // 隣接する6方向のマスを調査
            foreach(eDirectionHex dir in Directions) {
                HexTileData neighbor = HexTileManager.instance.GetToDirTile(current.gridPosX, current.gridPosY, dir);

                // 基本的な進入不可条件のフィルタリング（マップ外、探索済、山岳、移動不可属性）
                if(neighbor == null || closedSet.Contains(neighbor)) continue;
                if(neighbor.terrain == eTerrain.Mountain || neighbor.attribute == eAttribute.CannotMove) continue;

                // ユニット衝突判定：道中に別のキャラクターが配置されている、または移動予約がある場合は通行不可（壁）として処理
                if(neighbor.tileState == eTileState.CharacterIn
                    || neighbor.tileState == eTileState.Reserved) continue;

                // 地形に応じた移動コストの取得と安全弁チェック
                int movementCost = (int)neighbor.GetMovementCost();
                if(movementCost < 0) continue;

                // スタート地点から当該隣接マスまでの仮の実績コスト（gScore）を計算
                int tentativeGScore = gScore[current] + movementCost;

                // 過去に計算したルートより低コスト、または未探索のノードである場合、スコア情報を更新
                if(!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor]) {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    // 総推定コスト f(n) = g(n) + h(n) の算出
                    fScore[neighbor] = tentativeGScore + HeuristicDistance(neighbor, goal);

                    // 未登録であればオープンリストに追加し、次以降の探索候補とする
                    if(!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        // すべての探索候補を消費してもゴールに到達できなかった場合は経路なしとしてnullを返す
        return null;
    }

    /// <summary>
    /// ダイクストラ法：最大移動コストの範囲内で、到達可能なすべてのタイル群を計算して返す
    /// </summary>
    /// <param name="start">探索の起点（現在地）となるタイルデータ</param>
    /// <param name="maxCost">消費可能な最大移動力（総コスト限界値）</param>
    /// <param name="isEnemy">実行者が敵ユニットかどうか（拡張用パラメータ）</param>
    /// <returns>到達可能なタイルデータのハッシュセット</returns>
    public static HashSet<HexTileData> CalculateMovementRange(HexTileData start, int maxCost, bool isEnemy) {
        HashSet<HexTileData> reachableTiles = new HashSet<HexTileData>();
        Dictionary<HexTileData, int> costSoFar = new Dictionary<HexTileData, int>();
        Queue<HexTileData> frontier = new Queue<HexTileData>();

        if(start == null) return reachableTiles;

        // 起点ノードの初期化と探索キューへの蓄積
        frontier.Enqueue(start);
        costSoFar[start] = 0;

        // 幅優先探索ベースで周囲へのコスト伝播ループを実行
        while(frontier.Count > 0) {
            HexTileData current = frontier.Dequeue();
            int currentCost = costSoFar[current];

            // 現在のノードから6方向への探索を展開
            foreach(eDirectionHex dir in Directions) {
                HexTileData neighbor = HexTileManager.instance.GetToDirTile(current.gridPosX, current.gridPosY, dir);

                // 基本的な進入不可判定（マップ外、山岳、移動不可属性）
                if(neighbor == null || neighbor.terrain == eTerrain.Mountain || neighbor.attribute == eAttribute.CannotMove) continue;

                // 自身の足元（startノード）を除き、すでに他のキャラクターが立っているマスは通行・停止ともに不可能なためスキップ
                if(neighbor != start && neighbor.tileState == eTileState.CharacterIn) continue;

                // 地形の通行コスト取得
                int movementCost = (int)neighbor.GetMovementCost();
                if(movementCost < 0) continue;

                // 当該マスへ到達するための総累積コストを算出
                int newCost = currentCost + movementCost;

                // 累積コストが最大移動力を超える場合は、その先の探索を打ち切り
                if(newCost > maxCost) continue;

                // 未到達、または従来の計算経路より低コストで到達可能な場合に情報を更新
                if(!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) {
                    costSoFar[neighbor] = newCost;
                    frontier.Enqueue(neighbor);
                    reachableTiles.Add(neighbor);

                    // プレイヤーの視覚的フィードバック（UI表示）のためにタイル状態を移動可能に変更
                    neighbor.SetTileState(eTileState.Movable);
                }
            }
        }
        return reachableTiles;
    }

    /// <summary>
    /// アキシアル座標系における2点間のハリスティック（直線ステップ数）距離を算出する
    /// </summary>
    private static int HeuristicDistance(HexTileData start, HexTileData goal) {
        // アキシアル座標（q, r）から、3軸の整合性を担保する第3の成分 z (s) 座標を逆算（q + r + z = 0）
        int az = -start.gridPosX - start.gridPosY;
        int bz = -goal.gridPosX - goal.gridPosY;

        // 3軸それぞれの差分の絶対値のうち、最大のものを距離とする（ヘックス空間におけるマンハッタン距離に相当）
        return (Mathf.Abs(start.gridPosX - goal.gridPosX) + Mathf.Abs(start.gridPosY - goal.gridPosY) + Mathf.Abs(az - bz)) / 2;
    }

    /// <summary>
    /// 確定した cameFrom の家系図マップをゴールからスタートへ向かって逆引きし、移動順の経路リストに再構築して反転する
    /// </summary>
    private static List<HexTileData> RetracePath(HexTileData start, HexTileData goal, Dictionary<HexTileData, HexTileData> cameFrom) {
        List<HexTileData> path = new List<HexTileData>();
        HexTileData current = goal;

        // ゴールから親ノードを遡ってリストに追加
        while(current != start) {
            path.Add(current);
            current = cameFrom[current];
        }

        // ゴールからスタートへの順序になっているリストを、スタートからゴールへの正しい順序に反転
        path.Reverse();
        return path;
    }
}