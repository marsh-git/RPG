using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SelectModePart : BasePart
{
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private GameObject nameObj;

    public override async UniTask Init()
    {
        await base.Init();
        
    }

    public override async UniTask Setup()
    {
        await base.Setup();
        //ホストかどうかで分岐
        if (TitlePart.isHost)
        {
            CustomNetworkManager.instance.StartServer();
        }
        else
        {

            CustomNetworkManager.instance.StartClient();
        }
        Instantiate(nameObj, rect);
    }

    public override async UniTask Execute()
    {
        //全員準備完了まで待ち
        await UniTask.WaitUntil(() => WaitReadyAllPlayer());

        await UniTask.Delay(3000);

        //ゲーム開始
        await PartManager.instance.TransitionPart(GameEnum.eGamePart.MainGame);
    }

    /// <summary>
    /// 参加人数と準備完了人数が同じならtrueを返す(方法考え中)
    /// </summary>
    /// <returns></returns>
    private bool WaitReadyAllPlayer()
    {
        return ServerManager.instance.connectPlayer.Count == int.MaxValue;
    } 
}
