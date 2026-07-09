using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDataBase : ScriptableObject
{

    [Header("イベント名")]
    private string eventName;

    [Header("イベント説明")]
    [TextArea(3, 6)]private string eventDescription;

    [Header("イベント画像")]
    public Sprite eventImage;

}
