using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventDataBase : ScriptableObject
{

    [Header("イベント名")]
    public string eventName;

    [Header("イベント説明")]
    [TextArea(3, 6)]public string eventDescription;

    [Header("イベント画像")]
    public Sprite eventImage;

    /// <summary>
    /// イベント開始関数
    /// </summary>
    public abstract void StartEvent();

    protected virtual void SetEventUI()
    {

    }

}
