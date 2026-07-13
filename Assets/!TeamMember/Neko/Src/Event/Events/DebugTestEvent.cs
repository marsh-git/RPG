using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/EventData/DebugTest")]
public class DebugTestEvent : EventDataBase
{

    private Button button;

    protected override void EventUpdate()
    {

    }

    protected override void EndEvent()
    {
        DestroyButton();

        EventManager.instance.CloseEventUI();
    }

    public override void SetEventUI(Canvas eventUI)
    {
        base.SetEventUI(eventUI);

        //  ボタンを追加
        button = SetButton();
        button.GetComponentInChildren<TextMeshProUGUI>().text = "Test Button";
        button.onClick.AddListener(OnDebugClicked);

    }

    /// <summary>
    /// ボタンを設定
    /// </summary>
    /// <returns></returns>
    private Button SetButton()
    {
        return Instantiate(eventButton, uiChildren[EVENTUI_BUTTON_PARENT]);
    }

    private void DestroyButton()
    {
        Destroy(button.gameObject);
    }

    /// <summary>
    /// ボタンのイベント
    /// </summary>
    private void OnDebugClicked()
    {
        Debug.Log("イベントが承諾されました");
        EndEvent();
    }

}
