using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class SelectModePart : BasePart
{
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private GameObject nameObj;

    private bool startGame = false;

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
            CustomNetworkManager.instance.StartHost();
             this.ServerExecute().Forget();
        }
        else
        {
            CustomNetworkManager.instance.StartClient();
        }


        Instantiate(nameObj, rect);
    }

    public override async UniTask Execute()
    {
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// サーバー側処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask ServerExecute()
    {
        await UniTask.WaitUntil(() => startGame);

        //ゲーム開始
        await PartManager.instance.TransitionPart(GameEnum.eGamePart.MainGame);
    }

    /// <summary>
    /// ゲーム開始
    /// ボタンに実装
    /// </summary>
    public void StartGame()
    {
        if (!PartNetworkGame.instance.CheckAllReady()) return;

        startGame = true;
    }
}
