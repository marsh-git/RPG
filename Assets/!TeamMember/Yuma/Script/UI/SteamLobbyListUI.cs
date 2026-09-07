using Custom.Network;
using Steamworks;
using UnityEngine;

public class SteamLobbyListUI : MonoBehaviour
{
    [SerializeField]
    private SteamLobby steamLobby;

    [SerializeField]
    private SteamLobbyButton buttonPrefab;

    [SerializeField]
    private Transform root;

    [SerializeField,Header("探すゲーム名※確定次第定数化")]
    private string SEARCH_GAME_NAME = "MultiRPG";

    private const string GAME_NAME = "game_name";

    

    private void Start()
    {
        steamLobby = FindAnyObjectByType<SteamLobby>();
        steamLobby.OnLobbyListUpdated += UpdateLobbyList;
    }

    /// <summary>
    /// ロビーリストが揃ったタイミングで呼ばれるデリゲートの中身
    /// </summary>
    /// <param name="_lobbyIDs"></param>
    private void UpdateLobbyList(CSteamID[] _lobbyIDs)
    {
        //リストUIリセット
        root.gameObject.SetActive(true);
        foreach(Transform child in root)
        {
            if(child.name == "LobbyButton(Clone)")
            Destroy(child.gameObject);
        }

        foreach(var id in _lobbyIDs)
        {
            //フィルター掛け
            //string searchGameName = SteamMatchmaking.GetLobbyData(id, GAME_NAME);
            //if (searchGameName != SEARCH_GAME_NAME) continue;

            //ロビー参加ボタン作成
            var button = Instantiate(buttonPrefab, root);
            button.Setup(id,OnClickJoinLobby);
        }
    }

    /// <summary>
    /// ロビー参加処理
    /// </summary>
    /// <param name="_lobbyID"></param>
    private void OnClickJoinLobby(CSteamID _lobbyID)
    {
        SteamMatchmaking.JoinLobby(_lobbyID);
    }

    public void HideLobbyListUI()
    {
        root.gameObject.SetActive(false);
    }
}
