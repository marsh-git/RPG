using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{

    [SyncVar] public bool isReady = false;

    [Command]
    public void CmdToggleReady()
    {
        isReady = !isReady;
    }
}
