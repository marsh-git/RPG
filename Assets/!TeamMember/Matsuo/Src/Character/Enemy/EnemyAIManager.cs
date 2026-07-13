using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 敵ターン開始
    /// </summary>
    public async UniTask StartEnemyTurn()
    {
        // 生存している敵一覧を取得
        List<EnemyBase> enemyList = CharacterManager.Instance.GetEnemies();

        // 敵ごとの移動ルートを保持する
        Dictionary<EnemyBase, List<HexTileData>> routes = new();

        // 各敵の移動ルートを決定する
        foreach (EnemyBase enemy in enemyList)
        {
            if (enemy == null || enemy.IsDead)
            {
                continue;
            }

            // 敵AIに移動ルートを決めてもらう
            List<HexTileData> route = enemy.DecideRoute();

            if (route != null && route.Count > 0)
            {
                routes.Add(enemy, route);
            }
        }

        // ゴール地点の重複を防ぐ
        HashSet<int> reservedTile = new();

        // プレイヤーが立っているマスは予約済みにする
        foreach (PlayerBase player in CharacterManager.Instance.GetPlayers())
        {
            reservedTile.Add(player.GetTileID());
        }

        // 敵同士のゴールが重ならないよう調整
        foreach (var pair in routes)
        {
            List<HexTileData> route = pair.Value;

            while (route.Count > 0)
            {
                // 現在のゴール地点
                int goalID = route[^1].ID;

                // 空いているなら予約して終了
                if (!reservedTile.Contains(goalID))
                {
                    reservedTile.Add(goalID);
                    break;
                }

                // 被っていたらゴールを1マス手前に変更
                route.RemoveAt(route.Count - 1);
            }
        }

        // 全ての敵を同時に移動させる
        List<UniTask> tasks = new();

        foreach (var pair in routes)
        {
            // ルートが無くなった敵は移動しない
            if (pair.Value.Count == 0)
            {
                continue;
            }

            // 敵へ移動ルートを渡す
            pair.Key.SetMoveRoute(pair.Value);

            // 移動タスクを追加
            tasks.Add(pair.Key.MoveAsync(pair.Value));
        }

        // 全員の移動終了まで待機
        await UniTask.WhenAll(tasks);

        // 敵ターン終了
        TurnManager.Instance.EndEnemyTurn();
    }

    /// <summary>
    /// 一番移動コストが少ないプレイヤーを取得する
    /// </summary>
    public PlayerBase FindTargetPlayer(EnemyBase enemy)
    {
        if (enemy == null)
        {
            return null;
        }

        PlayerBase targetPlayer = null;

        // 現時点で一番小さいコスト
        float lowestCost = float.MaxValue;

        // 敵の現在位置
        HexTileData enemyTile =
            HexTileManager.instance.GetTileData(enemy.GetTileID());

        if (enemyTile == null)
        {
            return null;
        }

        // 生存プレイヤー一覧
        List<PlayerBase> playerList = CharacterManager.Instance.GetPlayers();

        foreach (PlayerBase player in playerList)
        {
            if (player == null || player.IsDead)
            {
                continue;
            }

            // プレイヤーの現在位置
            HexTileData playerTile =
                HexTileManager.instance.GetTileData(player.GetTileID());

            if (playerTile == null)
            {
                continue;
            }

            // 敵からプレイヤーまでの最短ルートを取得
            List<HexTileData> route =
                HexRouteSearcher.FindPath(enemyTile, playerTile, true);

            if (route == null || route.Count == 0)
            {
                continue;
            }

            // ルート全体の移動コストを計算
            float totalCost = 0f;

            foreach (HexTileData tile in route)
            {
                totalCost += tile.GetMovementCost();
            }

            // 一番コストが低いプレイヤーをターゲットにする
            if (totalCost < lowestCost)
            {
                lowestCost = totalCost;
                targetPlayer = player;
            }
        }

        return targetPlayer;
    }
}