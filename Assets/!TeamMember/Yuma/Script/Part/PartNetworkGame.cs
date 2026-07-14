using Cysharp.Threading.Tasks;
using Mirror;

/// <summary>
/// ネットワーク同期用パート
/// </summary>
public class PartNetworkGame : NetworkBehaviour
{
    public static PartNetworkGame instance;

    public bool isNetworkReady = false;

    [SyncVar(hook = nameof(OnPartChanged))]
    public GameEnum.eGamePart currentPart;

    void Awake()
    {
        instance = this;
    }

    public override void OnStartServer()
    {
        isNetworkReady = true;
    }

    public override void OnStartClient()
    {
        isNetworkReady = true;
    }

    /// <summary>
    /// サーバー側パート変更
    /// </summary>
    /// <param name="_next"></param>
    [Server]
    public void SetPart(GameEnum.eGamePart _next)
    {
        currentPart = _next;
    }



    /// <summary>
    /// サーバー側パート変更発火時処理
    /// </summary>
    /// <param name="_oldPart"></param>
    /// <param name="_newPart"></param>
    void OnPartChanged(GameEnum.eGamePart _oldPart, GameEnum.eGamePart _newPart)
    {
        PartManager.instance.ChangePartClient(_newPart).Forget();
    }

    #region ロビー用処理

    public bool CheckAllReady()
    {
        int readyCount = 0;

        foreach (var conn in NetworkServer.connections)
        {
            var player = conn.Value.identity.GetComponent<LobbyPlayer>();
            if (player == null || !player.isReady)
            {
                return false;
            }
            readyCount++;
        }

        if (readyCount == NetworkServer.connections.Count)
        {
            return true;
        }
        return false;
    }
    #endregion
}
