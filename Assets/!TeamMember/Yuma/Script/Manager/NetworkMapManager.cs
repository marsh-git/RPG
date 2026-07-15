using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ネットワーク用タイル生成クラス
/// 現状HexMapGeneratorを触っていいかわからないので放置
/// </summary>
public class NetworkMapManager : NetworkBehaviour
{
    public struct TileInfo
    {
        public int id;
        public int q;
        public int r;
        public eTerrain terrain;
        public eAttribute attribute;
    }

    public static NetworkMapManager instance;

    [Server]
    public void CreateMapNetwork()
    {
        
    }

    [ClientRpc]
    private void RpcSendMapData(List<TileInfo> _tiles)
    {

    }
}
