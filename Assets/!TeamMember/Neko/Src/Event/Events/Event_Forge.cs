using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/EventData/Forge")]
public class Event_Forge : EventDataBase
{
    private Button[] button;

    private readonly int BUTTON_COUNT = 2;

    [Header("イベントを決めるボタンのテキスト")]
    [SerializeField] EventButtonInfo[] buttonInfos = new EventButtonInfo[3];

    [Header("付与するレリック")]
    [SerializeField] RelicDataBase[] relicData;

    [Header("次のページのイベントの説明")]
    [TextArea(3, 6)] public string[] nextEventDescription;

    private bool luckCheck = false;

    protected override void EndEvent()
    {
        //  ボタン破棄
        for (int i = 0; i < button.Length; i++)
        {
            if (button[i] != null) DestroyButton(i);
        }

        EventManager.instance.CloseEventUI();
    }

    protected override void EventUpdate()
    {
        //  ボタン破棄
        for (int i = 0; i < button.Length; i++)
        {

            DestroyButton(i);
        }

        SetUpdateEventUI();

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

        button[0].onClick.AddListener(AddSwordRelic);
        button[1].onClick.AddListener(AddShieldRelic);
    }

    private void SetUpdateEventUI()
    {
        if (!luckCheck)
            uiChildren[EVENTUI_DESCRIPTION].GetComponent<TextMeshProUGUI>().text = nextEventDescription[0];
        else
            uiChildren[EVENTUI_DESCRIPTION].GetComponent<TextMeshProUGUI>().text = nextEventDescription[1];

        button = new Button[1];
        button[0] = SetButton();
        buttonInfos[2].SetButtonText(button[0].transform);

        button[0].onClick.AddListener(EndEvent);
    }

    private void DestroyButton(int num)
    {
        Destroy(button[num].gameObject);
    }

    private void LuckCheck()
    {
        int randMax = 10 - PlayerBase.instance.Status.luck;
        if (randMax <= 0) randMax = 1;
        int rand = Random.Range(0, randMax);
        if(rand == 0) luckCheck = true;
        else luckCheck = false;
    }

    private void AddSwordRelic()
    {
        LuckCheck();
        if(!luckCheck) PlayerBase.instance.AddRelic(relicData[0]);
        else PlayerBase.instance.AddRelic(relicData[1]);
        EventUpdate();
    }

    private void AddShieldRelic()
    {
        LuckCheck();
        if (!luckCheck) PlayerBase.instance.AddRelic(relicData[2]);
        else PlayerBase.instance.AddRelic(relicData[3]);
        EventUpdate();
    }

}
