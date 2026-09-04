using Custom.Network;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 元あるNetworkManagerの派生クラス
/// </summary>
public class CustomNetworkManager : NetworkManager
{
    public static CustomNetworkManager instance;

    public GameObject lobbyPlayerPrefab;
    public override void Awake()
    {
        base.Awake();
        instance = this;
    }
    

    /// <summary>
    /// サーバー開始時処理
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();

        
        //起動時タイトルマネージャーのインスタンスが存在していたら、
        if (TitleManager.instance != null)
        {
            //その後は不必要なので更新しないようにする
            TitleManager.instance.enabled = false;
        }
    }

    /// <summary>
    /// クライアント開始時
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();

        //if (Application.isBatchMode) return;
        GameObject uiRoot = GameObject.Find("GameUI");
        
    }

    /// <summary>
    /// サーバーに接続したタイミングで処理される
    /// 主にサーバー接続可能人数を判定
    /// </summary>
    /// <param name="_conn"></param>
    public override void OnServerConnect(NetworkConnectionToClient _conn)
    {
        //もし参加人数が既定の数超えていたら
        if (NetworkServer.connections.Count >= maxConnections)
        {
            _conn.Disconnect();
            return;
        }
        base.OnServerConnect(_conn);

    }

    public override void OnServerReady(NetworkConnectionToClient _conn)
    {
        base.OnServerReady(_conn);
        PlayerConnection(_conn, lobbyPlayerPrefab);
    }

    /// <summary>
    /// オーバーライドしたOnServerAddPlayer
    /// サーバーに参加したことを伝える(具体的にはconnectPlayerに参加したタイミングでAddする)
    /// </summary>
    /// <param name="_conn"></param>
    public override void OnServerAddPlayer(NetworkConnectionToClient _conn)
    {
        
        if (!ServerManager.instance.connectPlayer.Contains(_conn.identity))
        {
            ServerManager.instance.connectPlayer.Add(_conn.identity);
        }

        Debug.Log(ServerManager.instance.connectPlayer.Count);
    }

    /// <summary>
    /// クライアントが参加した時の処理
    /// </summary>
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.Ready();
    }

    /// <summary>
    /// オーバーライドしたOnServerDisconnect
    /// クライアントが抜けたタイミングでconnectPlayerからRemoveする
    /// </summary>
    /// <param name="_conn"></param>
    public override void OnServerDisconnect(NetworkConnectionToClient _conn)
    {

    }

    /// <summary>
    /// クライアントが止まった時の処理
    /// </summary>
    public override void OnStopClient()
    {
        base.OnStopClient();
       
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

    }

    /// <summary>
    /// アプリ終了時の解放処理
    /// </summary>
    public override void OnApplicationQuit()
    {
        // サーバー or クライアントとして接続中なら安全に終了
        if (NetworkServer.active || NetworkClient.isConnected)
        {
            StopHost();
        }
    }


    public override void OnStopHost()
    {
        base.OnStopHost();
        var udpBroadcaster = FindObjectOfType<UDPBroadcaster>();
        udpBroadcaster?.StopBroadcast();
        FindObjectOfType<UDPListener>()?.StopReceiveIP();
        if (udpBroadcaster != null)
            Destroy(udpBroadcaster.gameObject);
        Destroy(gameObject);
    }

    /// <summary>
    /// プレイヤーをスポーンさせて結びつける
    /// </summary>
    public void PlayerConnection(NetworkConnectionToClient _conn,GameObject _playerPrefab)
    {
        GameObject player = Instantiate(_playerPrefab);
        NetworkServer.AddPlayerForConnection(_conn, player);

    }


    /// <summary>
    /// SteamLobbyを使ってホストを開始
    /// UIから呼べるよ
    /// </summary>
    public void  StartSteamHost()
    {
        var steamLobby = GetComponent<SteamLobby>();

        if(steamLobby == null)
        {
            Debug.LogError("SteamLobby conponent not found");
            return;
        }

        steamLobby.CreateLobby();
    }

    /// <summary>
    /// SteamLobbyからルーム検索(ロビー一覧作成)
    /// ボタンからロビーIDを取得しルームに入る
    /// </summary>
    public void SearchSteamLobby()
    {
        var steamLobby = GetComponent<SteamLobby>();

        if (steamLobby == null)
        {
            Debug.LogError("SteamLobby conponent not found");
            return;
        }

        steamLobby.ShowRequesetLobbyList();
       
    }
}
