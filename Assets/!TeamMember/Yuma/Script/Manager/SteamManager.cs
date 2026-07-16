using Steamworks;
using UnityEngine;

namespace Custom.Network
{

    /// <summary>
    /// SteamworksのAPI初期化・終了処理を管理
    /// </summary>
    public class SteamManager : MonoBehaviour
    {
        private bool initialized = false;

        private void Awake()
        {
            if (!SteamAPI.Init())
            {
                Debug.LogError("[SteamManager] Steam API initialization failed! Make sure Steam client is running and steam_appid.txt exists.");
                initialized = false;
                return;
            }
            initialized = true;
            Debug.Log("[SteamManager] Steam API initialized successfully!");
        }

        // Update is called once per frame
        void Update()
        {
            if (!initialized)
                return;

            SteamAPI.RunCallbacks();
        }

        private void OnDestroy()
        {
            if (initialized)
            {
                SteamAPI.Shutdown();
                Debug.Log("[SteamManager] Steam API shutdown.");
            }
        }
    }
}

