using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    // ネットワークID
    protected int id;

    // ステータス
    protected int maxHp = 100;
    protected int hp;
    protected int attack = 10;
    protected int defense = 5;
    protected int luck = 0;

    public bool IsDead => hp <= 0;

    protected virtual void Start()
    {
        hp = maxHp;
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage - defense);

        hp -= finalDamage;

        if (hp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 回復処理
    /// </summary>
    /// <param name="amount"></param>
    public virtual void Heal(int amount)
    {
        hp = Mathf.Min(maxHp, hp + amount);
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    protected virtual void Die()
    {
        // 死亡処理
    }
}