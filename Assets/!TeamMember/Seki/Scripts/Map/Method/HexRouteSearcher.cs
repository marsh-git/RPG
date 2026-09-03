using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ヘックスグリッドマップにおける経路探索および移動範囲計算を行う静的クラス
/// </summary>
public static class HexRouteSearcher {
    /// <summary>
    /// 移動範囲および攻撃可能マスの計算結果を格納する構造体
    /// </summary>
    public struct MovementRangeResult {
        // プレイヤーが移動して立ち止まれるマスの集合
        public HashSet<HexTileData> MovableTiles;
        // 移動範囲内に存在し、攻撃対象となる（敵がいる）マスの集合
        public HashSet<HexTileData> AttackableTiles;

        public MovementRangeResult(HashSet<HexTileData> movable, HashSet<HexTileData> attackable) {
            MovableTiles = movable;
            AttackableTiles = attackable;
        }
    }
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
           goal.Attribute == eAttribute.CannotMove ||
           goal.tileState == eTileMoveState.CharacterIn) return null;

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
                if(neighbor.terrain == eTerrain.Mountain || neighbor.Attribute == eAttribute.CannotMove) continue;

                // ユニット衝突判定：道中に別のキャラクターが配置されている、または移動予約がある場合は通行不可（壁）として処理
                if(neighbor.tileState == eTileMoveState.CharacterIn
                    || neighbor.tileState == eTileMoveState.Reserved) continue;

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
    /// ハイブリッドダイクストラ法：
    /// 最大移動コストの範囲内で、到達可能なマス（移動用）と、手が届く範囲の全候補マス（攻撃検索用）を計算して返す
    /// </summary>
    public static MovementRangeResult CalculateMovementRange(HexTileData start, int maxCost) {
        HashSet<HexTileData> movableTileList = new HashSet<HexTileData>();
        // コストの範囲内ですべてのマスの集合（敵がいるマスも含む）
        HashSet<HexTileData> scannedAllTiles = new HashSet<HexTileData>();

        Dictionary<HexTileData, int> costSoFar = new Dictionary<HexTileData, int>();
        Queue<HexTileData> frontier = new Queue<HexTileData>();

        if(start == null) return new MovementRangeResult(movableTileList, scannedAllTiles);

        frontier.Enqueue(start);
        costSoFar[start] = 0;

        while(frontier.Count > 0) {
            HexTileData current = frontier.Dequeue();
            int currentCost = costSoFar[current];

            foreach(eDirectionHex dir in Directions) {
                HexTileData neighbor = HexTileManager.instance.GetToDirTile(current.gridPosX, current.gridPosY, dir);

                // 基本的な進入不可判定（マップ外、山岳地形、移動不可属性）
                if(neighbor == null || neighbor.terrain == eTerrain.Mountain || neighbor.Attribute == eAttribute.CannotMove) continue;

                int movementCost = (int)neighbor.GetMovementCost();
                if(movementCost < 0) continue;

                int newCost = currentCost + movementCost;

                // 累積コストが最大移動力を超える場合はその先へ進めない
                if(newCost > maxCost) continue;

                // コスト的に届くマスであれば、敵の有無に関わらず「スキャン済（攻撃候補）」に登録
                scannedAllTiles.Add(neighbor);

                // ユニット衝突判定：敵や味方がいるマスは、探索の末端とする（frontierに入れないことで奥へのすり抜けを防ぐ）
                if(neighbor.tileState == eTileMoveState.CharacterIn || neighbor.tileState == eTileMoveState.Reserved) {
                    continue;
                }

                // 通常の移動可能マスの更新処理
                if(!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) {
                    costSoFar[neighbor] = newCost;
                    frontier.Enqueue(neighbor);
                    movableTileList.Add(neighbor);

                    neighbor.SetTileState(eTileMoveState.Movable);
                }
            }
        }
        // 移動できるマスと、手が届いた全マス（敵含む）のペアを返す
        return new MovementRangeResult(movableTileList, scannedAllTiles);
    }
    /// <summary>
    /// 純粋ダイクストラ法：攻撃対象のスキャンを行わず、自ユニットが通行・着地可能なマスの集合を計算する
    /// </summary>
    /// <param name="start">探索の起点（現在地）</param>
    /// <param name="maxCost">最大移動力</param>
    /// <returns>着地可能なタイルのハッシュセット</returns>
    public static HashSet<HexTileData> CalculatePureMovementRange(HexTileData start, int maxCost) {
        HashSet<HexTileData> movableTileList = new HashSet<HexTileData>();
        Dictionary<HexTileData, int> costSoFar = new Dictionary<HexTileData, int>();
        Queue<HexTileData> frontier = new Queue<HexTileData>();

        if(start == null) return movableTileList;

        frontier.Enqueue(start);
        costSoFar[start] = 0;

        while(frontier.Count > 0) {
            HexTileData current = frontier.Dequeue();
            int currentCost = costSoFar[current];

            foreach(eDirectionHex dir in Directions) {
                HexTileData neighbor = HexTileManager.instance.GetToDirTile(current.gridPosX, current.gridPosY, dir);

                // 基本的な進入不可判定（マップ外・山岳・移動不可属性）
                if(neighbor == null || neighbor.terrain == eTerrain.Mountain || neighbor.Attribute == eAttribute.CannotMove) continue;

                // ユニット衝突判定：他キャラや移動予約があるマスは、侵入もすり抜けも不可（壁扱い）
                if(neighbor.tileState == eTileMoveState.CharacterIn || neighbor.tileState == eTileMoveState.Reserved) continue;

                int movementCost = (int)neighbor.GetMovementCost();
                if(movementCost < 0) continue;

                int newCost = currentCost + movementCost;
                if(newCost > maxCost) continue;

                // 未到達、またはより低コストな最適経路を発見した場合に更新
                if(!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) {
                    costSoFar[neighbor] = newCost;
                    frontier.Enqueue(neighbor);
                    movableTileList.Add(neighbor);

                    // UI表示などのためにタイルのステートを更新
                    neighbor.SetTileState(eTileMoveState.Movable);
                }
            }
        }
        return movableTileList;
    }
    /// <summary>
    /// 指定された候補マスリストの中から、攻撃対象（敵ユニット）が存在するマスを抽出して返す
    /// </summary>
    /// <param name="start">起点となるマス（自身を除外するため）</param>
    /// <param name="searchTargetTiles">検索対象となるマスのコレクション（移動範囲、スキル範囲など）</param>
    /// <param name="isAttackerEnemy">攻撃を実行する側が敵ユニットかどうか</param>
    /// <returns>攻撃対象が存在するタイルの集合</returns>
    public static HashSet<HexTileData> FindAttackableTilesInCandidates(
        HexTileData start,
        IEnumerable<HexTileData> searchTargetTiles,
        bool isAttackerEnemy) {
        HashSet<HexTileData> attackableTiles = new HashSet<HexTileData>();

        if(start == null || searchTargetTiles == null) return attackableTiles;

        foreach(var tile in searchTargetTiles) {
            if(tile == null) continue;

            // 起点マス（自分自身の足元）は攻撃対象から除外
            if(tile == start) continue;

            // マスにキャラクターが存在する場合
            if(tile.tileState == eTileMoveState.CharacterIn) {

                // TODO: キャラクターマネージャー等からタイル上のキャラクターデータを取得するロジック
                // CharacterBase targetChara = CharacterManager.instance.GetCharacterOnTile(tile.ID);
                // if (targetChara != null && targetChara.IsEnemy() != isAttackerEnemy) { ... }

                // 現状は暫定的に「CharacterIn」であれば一律で対象とみなす（必要に応じて陣営チェックを有効化）
                attackableTiles.Add(tile);
            }
        }

        return attackableTiles;
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