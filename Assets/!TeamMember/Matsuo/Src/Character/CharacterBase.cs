using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    // ネットワークID
    protected int id;
    // 現在いるタイル
    protected int tileID;
    // キャラクターの移動処理
    protected CharacterMovement movement = new();
    // 移動キャンセル用
    protected CancellationTokenSource moveCancellation;
    // キャラクター共通のステータス
    protected CharacterStatus status;
    // 現在HP
    protected int hp;
    // 死亡しているか
    public bool IsDead => hp <= 0;
    // 移動中か
    public bool IsMoving { get; private set; }
    // 選択フラグ
    public bool isSelect { get; protected set; } = false;

    // 移動ルート
    public List<HexTileData> currentMoveRoute { get; private set; } = new List<HexTileData>();
    /// <summary>
    /// キャラクターの初期化
    /// </summary>
    protected virtual void Awake()
    {
        hp = status.maxHp;
    }
    /// <summary>
    /// タイルIDの取得
    /// </summary>
    /// <returns></returns>
    public virtual int GetTileID()
    {
        return tileID;
    }
    /// <summary>
    /// キャラクターを指定したタイルへ配置
    /// </summary>
    /// <param name="tile">配置先のタイル</param>
    public virtual void SetTile(int setTileID)
    {
        tileID = setTileID;

        HexTileData tileData = HexTileManager.instance.GetTileData(tileID);
        if (tileData != null) transform.position = tileData.GetTilePos() + Vector3.up * 0.5f;
    }

    /// <summary>
    /// 指定した経路に沿ってキャラクターを移動
    /// </summary>
    /// <param name="path">移動経路</param>
    public virtual async UniTask MoveAsync(List<HexTileData> path)
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

        List<HexTileData> movePath = new List<HexTileData>(path);

        await movement.MoveAlongPathAsync(
        transform,
        movePath,
        moveCancellation.Token,
        tileData =>
        {
            if (tileData == null)
            {
                return;
            }

            // 今いるマスを通常状態に戻す
            HexTileData oldTile = HexTileManager.instance.GetTileData(tileID);
            if (oldTile != null)
            {
                oldTile.SetTileState(eTileState.Normal);
            }

            // タイルID更新
            tileID = tileData.ID;

            // 新しいマスを占有状態にする
            HexTileData newTile = HexTileManager.instance.GetTileData(tileID);
            if (newTile != null)
            {
                newTile.SetTileState(eTileState.CharacterIn);
            }
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
        int finalDamage = Mathf.Max(1, damage - status.defense);

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
        hp = Mathf.Min(status.maxHp, hp + amount);
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
    /// <summary>
    /// 移動ルートの設定
    /// </summary>
    /// <param name="route"></param>
    public void SetMoveRoute(List<HexTileData> route)
    {
        currentMoveRoute = route;
    }
    /// <summary>
    /// 移動終了処理
    /// </summary>
    public virtual void EndMove()
    {
        currentMoveRoute.Clear();
    }
    /// <summary>
    /// 敵か判別
    /// </summary>
    /// <returns></returns>
    public virtual bool IsEnemy()
    {
        return true;
    }
}