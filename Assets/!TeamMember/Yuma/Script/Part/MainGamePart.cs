using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MainGamePart : BasePart
{

    public override async UniTask Init()
    {
        await base.Init();
    }

    public override async UniTask Setup()
    {
        await base.Setup();
    }

    /// <summary>
    /// UI等共通処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute()
    {
        UniTask task = ServerExecute();
        task = ClientExecute();

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// サーバー側の実行処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask ServerExecute()
    {
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// クライアント側の実行処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask ClientExecute()
    {
        await UniTask.CompletedTask;
    }
}
