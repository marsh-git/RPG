using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance { get; private set; }

    [Header("イベントUI")]
    [SerializeField] public Canvas eventUI;

    [Header("ゲーム内で使用するイベントデータの配列")]
    [SerializeField] private EventDataBase[] eventDatas;

    private bool doingEvent = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(eventUI != null) eventUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 任意のイベント開始
    /// </summary>
    /// <param name="eventNum"></param>
    public void StartEvent(int eventNum)
    {
        if(eventDatas == null)
        {
            Debug.LogWarning("eventUIが取得できませんでした");
            return;
        }

        doingEvent = true;

        SetEventUI(eventNum);
    }

    /// <summary>
    /// イベントUIをセットする
    /// </summary>
    private void SetEventUI(int eventNum)
    {
        if (eventUI == null)
        {
            Debug.LogWarning("eventUIが取得できませんでした");
            return;
        }
        eventUI.gameObject.SetActive(true);

        eventDatas[eventNum].SetEventUI(eventUI);
    }

    /// <summary>
    /// イベントのUIを閉じる
    /// </summary>
    public void CloseEventUI()
    {
        doingEvent = false;
        eventUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// イベントが動いているかどうか
    /// </summary>
    /// <returns></returns>
    public bool GetDoingEvent()
    {
        return doingEvent;
    }

}
