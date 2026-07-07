using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    // ネットワークID
    protected int id;
    // 現在いるタイル
    protected HexTile currentTile;
    // キャラクターの移動処理
    protected CharacterMovement movement = new();
    // 移動キャンセル用
    protected CancellationTokenSource moveCancellation;
    // 最大HP
    protected int maxHp = 100;
    // 現在HP
    protected int hp;
    // 攻撃力
    protected int attack = 0;
    // 防御力
    protected int defense = 0;
    // 運
    protected int luck = 0;
    // 死亡しているか
    public bool IsDead => hp <= 0;
    // 移動中か
    public bool IsMoving { get; private set; }
    // 現在いるタイル
    public HexTile CurrentTile => currentTile;

    /// <summary>
    /// キャラクターの初期化
    /// </summary>
    protected virtual void Awake()
    {
        hp = maxHp;
    }

    /// <summary>
    /// キャラクターを指定したタイルへ配置
    /// </summary>
    /// <param name="tile">配置先のタイル</param>
    public virtual void SetTile(HexTile tile)
    {
        currentTile = tile;

        if (tile != null)
        {
            transform.position = tile.transform.position + Vector3.up * 0.5f;
        }
    }

    /// <summary>
    /// 指定した経路に沿ってキャラクターを移動
    /// </summary>
    /// <param name="path">移動経路</param>
    public virtual async UniTask MoveAsync(List<HexTile> path)
    {
        // すでに移動中なら何もしない
        if (IsMoving)
        {
            return;
        }

        // 前回の移動処理が残っていれば破棄する
        moveCancellation?.Cancel();
        moveCancellation?.Dispose();

        moveCancellation = new CancellationTokenSource();

        IsMoving = true;

        await movement.MoveAlongPathAsync(
            transform,
            path,
            moveCancellation.Token,
            tile =>
            {
                // 1マス移動するたび現在位置を更新する
                currentTile = tile;
            });

        IsMoving = false;
    }

    /// <summary>
    /// 現在の移動をキャンセル
    /// </summary>
    public virtual void CancelMove()
    {
        moveCancellation?.Cancel();
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage">受けるダメージ量</param>
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
    /// <param name="amount">回復量</param>
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

    /// <summary>
    /// オブジェクト破棄時の後始末を行う
    /// </summary>
    protected virtual void OnDestroy()
    {
        moveCancellation?.Cancel();
        moveCancellation?.Dispose();
    }
}