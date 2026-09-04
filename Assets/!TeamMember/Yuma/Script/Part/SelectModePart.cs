using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectModePart : BasePart
{
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private GameObject nameObj;

    private bool startGame = false;

    private LobbyPlayer localLobbyPlayer;
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
            this.ServerExecute().Forget();
        }
        await UniTask.WaitUntil(() => NetworkClient.localPlayer != null);


        GameObject localPlayerName = Instantiate(nameObj, rect);
        Toggle checkBox = localPlayerName.GetComponent<Toggle>();

        checkBox.onValueChanged.AddListener(ToggleReady);

        localLobbyPlayer = NetworkClient.localPlayer.GetComponent<LobbyPlayer>();

    }

    public override async UniTask Execute()
    {
        await FadeManeger.instance.FadeIn(1.0f);
        await UniTask.WaitUntil(() => startGame);
        await FadeManeger.instance.FadeOut(1.0f);
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
    /// 解放処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Teardown()
    {
        foreach (var conn in NetworkServer.connections)
        {
            //ロビープレイヤーを全削除
            if (conn.Value.identity != null)
                NetworkServer.Destroy(conn.Value.identity.gameObject);
        }

        await base.Teardown();
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

    /// <summary>
    /// ローカルプレイヤーの準備完了切り替え
    /// </summary>
    /// <param name="_isOn"></param>
    private void ToggleReady(bool _isOn)
    {
        if (!localLobbyPlayer.isLocalPlayer) return;
        localLobbyPlayer.CmdToggleReady(_isOn);
    }
}
