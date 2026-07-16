using Custom.Network;
using Cysharp.Threading.Tasks;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// タイトルパート
/// </summary>
public class TitlePart : BasePart
{
    public static bool isHost { get; private set; } = false;
    public bool goToSelectPart = false;
    private SteamLobby steamLobby;
    public override async UniTask Init()
    {
        await base.Init();
        steamLobby = FindAnyObjectByType<SteamLobby>();
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
        //steamLobby.CreateLobby();
        isHost = true;
        goToSelectPart = true;
    }

    /// <summary>
    /// クライアントで始める(部屋検索)
    /// </summary>
    public void StartClient(CSteamID _lobbyID)
    {
        Debug.Log("StartClient");
        //SteamMatchmaking.JoinLobby(_lobbyID);
        isHost = false;
        goToSelectPart = true;
    }
}
