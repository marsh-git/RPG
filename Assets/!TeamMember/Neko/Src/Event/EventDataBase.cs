using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class EventDataBase : ScriptableObject
{

    [Header("イベント名")]
    public string eventName;

    [Header("イベント説明")]
    [TextArea(3, 6)]public string eventDescription;

    [Header("イベント画像")]
    public Sprite eventImage;

    //  子オブジェクトのUI配置（固定）
    private readonly int EVENTUI_IMAGE = 1;
    private readonly int EVENTUI_DESCRIPTION = 2;

    /// <summary>
    /// イベント開始関数
    /// </summary>
    public abstract void StartEvent();

    /// <summary>
    /// UIをセットする
    /// </summary>
    /// <param name="eventUI"></param>
    public virtual void SetEventUI(Canvas eventUI)
    {
        Transform uiParent = eventUI.transform;
        var uiChildren = GetChildren(uiParent);
        uiChildren[EVENTUI_IMAGE].GetComponent<Image>().sprite = eventImage;
        uiChildren[EVENTUI_DESCRIPTION].GetComponent<TextMeshProUGUI>().text = eventDescription;
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

}
