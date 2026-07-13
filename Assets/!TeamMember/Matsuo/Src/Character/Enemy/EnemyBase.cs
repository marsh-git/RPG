using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
public class EnemyBase : CharacterBase
{
    // 敵の種類
    [SerializeField]
    protected EnemyType enemyType;

    // 移動力
    [SerializeField]
    protected int moveRange = 3;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 敵ターン開始
    /// </summary>
    public virtual async UniTask StartTurn()
    {
        await Think();
    }

    /// <summary>
    /// 敵AI
    /// </summary>
    protected virtual async UniTask Think()
    {
        // 一番近いプレイヤーを取得
        PlayerBase targetPlayer = EnemyAIManager.Instance.FindTargetPlayer(this);

        if (targetPlayer == null)
        {
            return;
        }

        // 自分とプレイヤーのタイル取得
        HexTileData myTile =
            HexTileManager.instance.GetTileData(GetTileID());

        HexTileData targetTile =
            HexTileManager.instance.GetTileData(targetPlayer.GetTileID());

        // 最短ルート取得
        List<HexTileData> route =
            HexRouteSearcher.FindPath(myTile, targetTile, true);

        if (route == null || route.Count == 0)
        {
            return;
        }

        // 移動力分だけ切り出す
        if (route.Count > moveRange)
        {
            route = route.GetRange(0, moveRange);
        }

        // ルート設定
        SetMoveRoute(route);

        // 移動
        await MoveAsync(currentMoveRoute);
    }

    /// <summary>
    /// 移動先決定
    /// </summary>
    protected virtual HexTileData DecideMoveTile()
    {
        return null;
    }

    /// <summary>
    /// プレイヤーに攻撃
    /// </summary>
    protected virtual void AttackPlayer(PlayerBase player)
    {

    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    protected override void Die()
    {
        base.Die();

        // ここらへんにドロップ処理や経験値付与などを実装する
        Destroy(gameObject);
    }
}