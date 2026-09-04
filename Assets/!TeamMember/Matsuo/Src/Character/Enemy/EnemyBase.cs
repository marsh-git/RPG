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
    /// 敵ターン
    /// </summary>
    /// <returns></returns>
    public virtual List<HexTileData> DecideRoute()
    {
        PlayerBase targetPlayer = EnemyAIManager.Instance.FindTargetPlayer(this);

        if (targetPlayer == null)
        {
            return null;
        }

        HexTileData myTile =　HexTileManager.instance.GetTileData(GetTileID());

        HexTileData targetTile =　HexTileManager.instance.GetTileData(targetPlayer.GetTileID());

        List<HexTileData> route =　HexRouteSearcher.FindPath(myTile, targetTile, true);

        // 移動力分だけ切り出す
        if (route.Count > moveRange)
        {
            route = route.GetRange(0, moveRange);
        }

        // 最後はプレイヤーのマスなので削除する
        if (route.Count > 0)
        {
            route.RemoveAt(route.Count - 1);
        }

        return route;
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
    protected override void Die(){
        base.Die();

        // ここらへんにドロップ処理や経験値付与などを実装する
        Destroy(gameObject);

    }
}