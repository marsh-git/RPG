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
        List<EnemyBase> enemyList = CharacterManager.Instance.GetEnemies();

        foreach (EnemyBase enemy in enemyList)
        {
            if (enemy == null || enemy.IsDead)
            {
                continue;
            }

            await enemy.StartTurn();
        }

        TurnManager.Instance.EndEnemyTurn();
    }

    /// <summary>
    /// 一番移動コストが少ないプレイヤーを取得
    /// </summary>
    public PlayerBase FindTargetPlayer(EnemyBase enemy)
    {
        if (enemy == null)
        {
            return null;
        }

        PlayerBase targetPlayer = null;
        float lowestCost = float.MaxValue;

        HexTileData enemyTile =
            HexTileManager.instance.GetTileData(enemy.GetTileID());

        if (enemyTile == null)
        {
            return null;
        }

        List<PlayerBase> playerList = CharacterManager.Instance.GetPlayers();

        foreach (PlayerBase player in playerList)
        {
            if (player == null || player.IsDead)
            {
                continue;
            }

            HexTileData playerTile =
                HexTileManager.instance.GetTileData(player.GetTileID());

            if (playerTile == null)
            {
                continue;
            }

            List<HexTileData> route =
                HexRouteSearcher.FindPath(enemyTile, playerTile, true);

            if (route == null || route.Count == 0)
            {
                continue;
            }

            float totalCost = 0f;

            foreach (HexTileData tile in route)
            {
                totalCost += tile.GetMovementCost();
            }

            if (totalCost < lowestCost)
            {
                lowestCost = totalCost;
                targetPlayer = player;
            }
        }

        return targetPlayer;
    }
}