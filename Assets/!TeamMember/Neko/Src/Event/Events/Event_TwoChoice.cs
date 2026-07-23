using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/EventData/TwoChoice")]
public class Event_TwoChoice : EventDataBase
{
    private Button[] button;

    private readonly int BUTTON_COUNT = 2;

    [Header("イベントを決めるボタンのテキスト")]
    [SerializeField] EventButtonInfo[] buttonInfos = new EventButtonInfo[3];

    [Header("次のページのイベントの説明")]
    [TextArea(3, 6)] public string[] nextEventDescription;

    [Header("イベントを作製する際ステータスを追加するかなどの設定構造体")]
    [SerializeField] EventChoiceData[] choiceData;

    private int choiceNum;

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
            int index = i;

            button[i] = SetButton();

            buttonInfos[i].SetButtonText(button[i].transform);

            button[i].onClick.AddListener(() => OnStartEvent(choiceData[index], index));
        }
    }

    private void SetUpdateEventUI()
    {
        uiChildren[EVENTUI_DESCRIPTION].GetComponent<TextMeshProUGUI>().text = nextEventDescription[choiceNum];

        button = new Button[1];
        button[0] = SetButton();
        buttonInfos[2].SetButtonText(button[0].transform);

        button[0].onClick.AddListener(EndEvent);
    }

    private void DestroyButton(int num)
    {
        Destroy(button[num].gameObject);
    }

    //  イベント中身
    private void OnStartEvent(EventChoiceData choice, int count)
    {
        if (!choice.CanExecute()) return;

        choiceNum = count;

        choice.SwichEvent();

        if (choice.nextPage) EventUpdate();
        else EndEvent();
    }

}
