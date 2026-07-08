using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 元あるNetworkManagerの派生クラス
/// </summary>
public class CustomNetworkManager : NetworkManager
{
   
    /// <summary>
    /// タイトルからロビーに行ったか
    /// </summary>
    private static bool titleToLobby = true;
    /// <summary>
    /// タイトルシーンから移動してきたときに通る処理
    /// </summary>
    public override void Start()
    {
#if DEBUG
        if (TitleManager.instance == null)
        {
            base.Start();
            return;
        }
#endif
        if (TitleManager.instance.isHost)
        {
            //ホストとして開始
            StartHost();
        }
        else if (TitleManager.instance.isClient)
        {
            //クライアントとして開始
            networkAddress = TitleManager.instance.ipAddress;
            StartClient();
        }
        //サーバー参加時にカーソルロック
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// サーバー開始時処理
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();

        // サーバーが起動したタイミングで SystemManager に Network 系の Spawn を任せる
        //if (SystemManager.Instance != null)
        //{
        //    SystemManager.Instance.SpawnNetworkSystems();
        //}
        if(true)
        {
            Debug.LogWarning("SystemManager が見つかりません。SystemManager は最初のシーンに配置しておいてください。");
        }
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

    /// <summary>
    /// オーバーライドしたOnServerAddPlayer
    /// サーバーに参加したことを伝える(具体的にはconnectPlayerに参加したタイミングでAddする)
    /// </summary>
    /// <param name="_conn"></param>
    public override void OnServerAddPlayer(NetworkConnectionToClient _conn)
    {

        
    }

    /// <summary>
    /// クライアントが参加した時の処理
    /// </summary>
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        
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
            titleToLobby = true;
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
}
