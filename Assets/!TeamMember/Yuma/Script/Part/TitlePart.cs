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
        startHostButton.onClick.AddListener(() => {
            isHost = true;
            CustomNetworkManager.instance.StartSteamHost();
        });
        startClientButton.onClick.AddListener( () =>{
            isHost = false;
            
            CustomNetworkManager.instance.SearchSteamLobby(); 
        });

    }

    /// <summary>
    /// UI等共通処理(ローカル)
    /// ネット起動前は全部こっち
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute()
    {
        await FadeManeger.instance.FadeIn(1.0f);

        await UniTask.WaitUntil(()=>goToSelectPart);
        await FadeManeger.instance.FadeOut(1.0f);
    }
}
