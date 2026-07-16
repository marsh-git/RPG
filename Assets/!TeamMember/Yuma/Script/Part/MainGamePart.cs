using Cysharp.Threading.Tasks;
using Mirror;
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

    /// <summary>
    /// メインゲームのセットアップ
    /// マップやプレイヤーの生成、同期
    /// </summary>
    /// <returns></returns>
    public override async UniTask Setup()
    {
        await base.Setup();
        if (!NetworkServer.active) return;

        foreach(var conn in NetworkServer.connections.Values)
        {
            NetworkServer.AddPlayerForConnection(conn, CustomNetworkManager.instance.playerPrefab);
        }
    }

    /// <summary>
    /// UI等共通処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute()
    {
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// サーバー側の実行処理
    /// ゲーム進行系はこっち
    /// </summary>
    /// <returns></returns>
    public override async UniTask ServerExecute()
    {
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// クライアント側の実行処理
    /// 入力系はこっち
    /// </summary>
    /// <returns></returns>
    public override async UniTask ClientExecute()
    {
        await UniTask.CompletedTask;
    }
}
