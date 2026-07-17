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
        steamLobby.OnLobbyListUpdated += UpdateLobbyList;
    }

    private void UpdateLobbyList(CSteamID[] _lobbyIDs)
    {
        foreach(Transform child in root)
        {
            Destroy(child.gameObject);
        }

        foreach(var id in _lobbyIDs)
        {
            var button = Instantiate(buttonPrefab, root);
            button.Setup(id,OnClickJoinLobby);
        }
    }

    private void OnClickJoinLobby(CSteamID _lobbyID)
    {
        SteamMatchmaking.JoinLobby(_lobbyID);
    }
}
