using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全パートの基底クラス
/// </summary>
public abstract class BasePart : MonoBehaviour
{

    /// <summary>
    /// ゲーム開始時に一度だけ呼ばれる
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Init()
    {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// パート遷移前に呼ばれる
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Setup()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// パート実行中に呼ばれる
    /// ※UI等Host、Client共通処理※
    /// </summary>
    /// <returns></returns>
    public abstract UniTask Execute();

    /// <summary>
    /// パートが終了する際に呼ばれる
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Teardown()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// サーバー専用実行処理
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask ServerExecute()
    {
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// クライアント専用実行処理
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask ClientExecute()
    {
        await UniTask.CompletedTask;
    }
}
