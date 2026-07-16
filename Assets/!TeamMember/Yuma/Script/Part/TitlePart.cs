using Cysharp.Threading.Tasks;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// タイトルパート
/// </summary>
public class TitlePart : BasePart
{
    public static bool isHost { get; private set; } = false;
    public bool goToSelectPart = false;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    public override async UniTask Init()
    {
        await base.Init();
        await UniTask.CompletedTask;
    }
    public override async UniTask Setup()
    {
        await base.Setup();
        startHostButton.onClick.AddListener(() => CustomNetworkManager.instance.StartSteamHost());

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
    public void StartClient(CSteamID _lobbyID)
    {
        Debug.Log("StartClient");
        isHost = false;
        goToSelectPart = true;
    }
}
