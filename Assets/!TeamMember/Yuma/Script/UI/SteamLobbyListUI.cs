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

    private void Start()
    {
        steamLobby = FindAnyObjectByType<SteamLobby>();
        steamLobby.OnLobbyListUpdated += UpdateLobbyList;
    }

    private void UpdateLobbyList(CSteamID[] _lobbyIDs)
    {
        root.gameObject.SetActive(true);
        foreach(Transform child in root)
        {
            Destroy(child.gameObject);
        }

        foreach(var id in _lobbyIDs)
        {
            //フィルター掛け
            string hostKey = SteamMatchmaking.GetLobbyData(id, "HostAddress");
            if (hostKey != "2") continue;

            var button = Instantiate(buttonPrefab, root);
            button.Setup(id,OnClickJoinLobby);
        }
    }

    private void OnClickJoinLobby(CSteamID _lobbyID)
    {
        SteamMatchmaking.JoinLobby(_lobbyID);
    }
}
