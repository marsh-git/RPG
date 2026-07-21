using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class EventDataBase : ScriptableObject
{
    //  イベント固有ID
    private int eventID;
    public int EventID => eventID;

    [Header("イベント名")]
    [SerializeField] protected string eventName;

    [Header("イベント説明")]
    [TextArea(3, 6)] public string eventDescription;

    [Header("イベント画像")]
    [SerializeField] protected Sprite eventImage;

    [Header("キャラクターに与えるステータス補正(何もなければ0)")]
    [SerializeField] protected CharacterStatus addStatus;

    //  子オブジェクトのUI配置（固定）
    protected readonly int EVENTUI_IMAGE = 1;
    protected readonly int EVENTUI_NAME = 2;
    protected readonly int EVENTUI_DESCRIPTION = 3;
    protected readonly int EVENTUI_BUTTON_PARENT = 4;

    protected Transform[] uiChildren;

    public bool endEventFlag = false;

    protected abstract void EventUpdate();

    /// <summary>
    /// イベント終了関数
    /// </summary>
    protected abstract void EndEvent();

    /// <summary>
    /// UIをセットする
    /// </summary>
    /// <param name="eventUI"></param>
    public virtual void SetEventUI(Canvas eventUI)
    {
        Transform uiParent = eventUI.transform;
        uiChildren = GetChildren(uiParent);
        uiChildren[EVENTUI_IMAGE].GetComponent<Image>().sprite = eventImage;
        uiChildren[EVENTUI_NAME].GetComponent<TextMeshProUGUI>().text = eventName;
        uiChildren[EVENTUI_DESCRIPTION].GetComponent<TextMeshProUGUI>().text = eventDescription;
    }

    /// <summary>
    /// ボタンを設定
    /// </summary>
    /// <returns></returns>
    protected Button SetButton()
    {
        return Instantiate(EventManager.instance.eventButton, uiChildren[EVENTUI_BUTTON_PARENT]);
    }

    /// <summary>
    /// 子オブジェクトを取得する
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    private Transform[] GetChildren(Transform parent)
    {
        var children = new Transform[parent.childCount];

        for (var i = 0; i < children.Length; i++)
        {
            children[i] = parent.GetChild(i);
        }

        return children;
    }


    public void SetEventID(int id)
    {
        eventID = id;
    }
}
