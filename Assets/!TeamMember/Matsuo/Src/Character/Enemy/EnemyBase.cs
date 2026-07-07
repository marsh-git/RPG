using UnityEngine;

public class EnemyBase : CharacterBase
{
    // 敵の種類
    [SerializeField]
    protected EnemyType enemyType;

    /// <summary>
    /// 敵の初期化を行う
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 死亡時の処理
    /// </summary>
    protected override void Die()
    {
        base.Die();

        // ここらへんにドロップ処理や経験値付与などを実装する
        Destroy(gameObject);
    }
}