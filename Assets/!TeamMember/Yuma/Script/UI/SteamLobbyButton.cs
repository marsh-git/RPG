using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ロビー選択用ボタンクラス
/// 一覧にして生成する
/// </summary>
public class SteamLobbyButton : MonoBehaviour
{
    public CSteamID lobbyID;
    public TextMeshProUGUI label;
    public Button button;

    public void Setup(CSteamID _lobbyID, System.Action<CSteamID> _onclick)
    {
        lobbyID = _lobbyID;
        label.text = SteamMatchmaking.GetLobbyData(_lobbyID, "name");

        button.onClick.AddListener(() => _onclick(lobbyID));
    }
}
