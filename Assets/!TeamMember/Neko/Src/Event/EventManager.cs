using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager instance { get; private set; }

    [Header("生成するイベントUI")]
    [SerializeField] public Canvas eventUI;

    [Header("ボタン")]
    [SerializeField] public Button eventButton;

    [Header("ゲーム内で使用するイベントデータの配列")]
    [SerializeField] public EventDataBase[] eventDatas;

    //  生成したCanvasを格納する専用変数
    private Canvas canvas;

    private bool doingEvent = false;

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
        if(eventDatas == null)
        {
            Debug.LogWarning("イベントデータが取得できませんでした");
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
        canvas = Instantiate(eventUI);
        

        eventDatas[eventNum].SetEventUI(canvas);
    }

    /// <summary>
    /// イベントのUIを閉じる
    /// </summary>
    public void CloseEventUI()
    {
        doingEvent = false;
        Destroy(canvas.gameObject);
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
