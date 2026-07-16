using Mirror;
using Steamworks;
using UnityEngine;

namespace Custom.Network
{

    /// <summary>
    /// SteamLobby機能を管理し、ロビー作成、参加を処理するクラス
    /// </summary>
    public class SteamLobby : MonoBehaviour
    {
        private const string HOST_ADDRESS_KEY = "HostAddress";

        private Callback<LobbyCreated_t> lobbyCreated;
        private Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        private Callback<LobbyEnter_t> lobbyEntered;

        private CSteamID currentLobbyID;

        private NetworkManager networkManager;

        private void Awake()
        {
            networkManager = GetComponent<CustomNetworkManager>();
        }

        private void Start()
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        /// <summary>
        /// SteamLobby作成(フレンド限定)
        /// </summary>
        public void CreateLobby()
        {
            if (networkManager == null) return;

            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
        }

        /// <summary>
        /// Lobby作成完了時コールバック
        /// </summary>
        /// <param name="_callback"></param>
        private void OnLobbyCreated(LobbyCreated_t _callback)
        {
            if (_callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError($"Failed to create lobby : {currentLobbyID}");
                return;
            }
            currentLobbyID = new CSteamID(_callback.m_ulSteamIDLobby);

            string hostSteamID = SteamUser.GetSteamID().ToString();
            bool setDataSuccess = SteamMatchmaking.SetLobbyData(currentLobbyID, HOST_ADDRESS_KEY, hostSteamID);

            if (!setDataSuccess)
            {
                Debug.LogError("Failed to set lobby data");
                return;
            }

            networkManager.StartHost();
        }

        /// <summary>
        /// Lobby招待通知受信コールバック
        /// </summary>
        /// <param name="_callback"></param>
        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t _callback)
        {
            SteamMatchmaking.JoinLobby(_callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t _callback)
        {
            currentLobbyID = new CSteamID(_callback.m_ulSteamIDLobby);

            if (NetworkServer.active)
            {
                Debug.Log("Skipping client connection");
                return;
            }

            string hostAddress = SteamMatchmaking.GetLobbyData(currentLobbyID, HOST_ADDRESS_KEY);

            if (string.IsNullOrEmpty(hostAddress))
            {
                Debug.LogError("Failed to get host address from lobby data");
                return;
            }

            networkManager.networkAddress = hostAddress;
            networkManager.StartClient();
        }

        private void OnDestroy()
        {
            if(currentLobbyID != CSteamID.Nil)
            {
                SteamMatchmaking.LeaveLobby(currentLobbyID);
            }
        }
    }
}

