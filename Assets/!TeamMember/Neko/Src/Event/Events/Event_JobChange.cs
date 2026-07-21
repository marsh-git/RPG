using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/EventData/JobChange")]
public class Event_JobChange : EventDataBase
{

    private Button[] button;

    private readonly int BUTTON_COUNT = 4;

    [Header("イベントを決めるボタンのテキスト")]
    [SerializeField] EventButtonInfo[] buttonInfos = new EventButtonInfo[4];

    [Header("次のページのイベントの説明")]
    [TextArea(3, 6)] public string[] nextEventDescription;

    [Header("転職する役職の番号（JobManagerから参照）")]
    [SerializeField] private int[] jobNum = new int[3];

    protected override void EventUpdate()
    {
        //  ボタン破棄
        for (int i = 0; i < button.Length; i++)
        {

            DestroyButton(i);
        }

        SetUpdateEventUI();
    }

    protected override void EndEvent()
    {
        //  ボタン破棄
        for (int i = 0; i < button.Length; i++)
        {
            if (button[i] != null) DestroyButton(i);
        }

        endEventFlag = true;

        EventManager.instance.CloseEventUI();
    }

    public override void SetEventUI(Canvas eventUI)
    {
        base.SetEventUI(eventUI);

        //  ボタン追加
        button = new Button[BUTTON_COUNT];
        for (int i = 0; i < BUTTON_COUNT; i++)
        {
            button[i] = SetButton();

            buttonInfos[i].SetButtonText(button[i].transform);
        }
        
        //  ボタンイベント適応
        for(int i = 0; i < jobNum.Length; i++)
        {
            button[i].onClick.AddListener(() => OnJobChangeEvent(i));
        }
        button[jobNum.Length].onClick.AddListener(EndEvent);

    }

    private void SetUpdateEventUI()
    {
        uiChildren[EVENTUI_DESCRIPTION].GetComponent<TextMeshProUGUI>().text = nextEventDescription[0];

        button = new Button[1];
        button[0] = SetButton();
        buttonInfos[jobNum.Length].SetButtonText(button[0].transform);

        button[0].onClick.AddListener(EndEvent);
    }

    private void DestroyButton(int num)
    {
        Destroy(button[num].gameObject);
    }

    //  ジョブを変更するイベント発火
    private void OnJobChangeEvent(int num)
    {
        PlayerBase.instance.SetJob(jobNum[num]);
        EventUpdate();
    }
}
