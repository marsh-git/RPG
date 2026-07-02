using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// タイトルパート
/// </summary>
public class TitlePart : BasePart
{
    public bool isHost = false;
    public bool goToSelectPart = false;
    public override async UniTask Init()
    {
        await base.Init();
        await UniTask.CompletedTask;
    }
    public override async UniTask Setup()
    {
        await base.Setup();
    }


    public override async UniTask Execute()
    {
        await UniTask.WaitUntil(()=>goToSelectPart);

        await PartManager.instance.TransitionPart(GameEnum.eGamePart.SelectStage);
    }

    /// <summary>
    /// ホストで始める(部屋作成)
    /// </summary>
    public void StartIsHost()
    {
        Debug.Log("StartHost");
        isHost = true;
        goToSelectPart = true;
    }

    /// <summary>
    /// クライアントで始める(部屋検索)
    /// </summary>
    public void StartClient()
    {
        Debug.Log("StartClient");
        isHost = false;
        goToSelectPart = true;
    }
}
