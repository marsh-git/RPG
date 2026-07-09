using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance { get; private set; }

    [Header("ゲーム内で使用するイベントデータの配列")]
    [SerializeField] private EventDataBase[] eventDatas;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 任意のイベント開始
    /// </summary>
    /// <param name="eventNum"></param>
    public void StartEvent(int eventNum)
    {
        eventDatas[eventNum].StartEvent();
    }

}
