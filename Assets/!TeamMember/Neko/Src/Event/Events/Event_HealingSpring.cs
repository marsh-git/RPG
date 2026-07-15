using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/EventData/HealingSpring")]
public class Event_HealingSpring : EventDataBase
{

    private Button[] button;

    private readonly int BUTTON_COUNT = 3;

    [Header("イベントを決めるボタンのテキスト")]
    [SerializeField] EventButtonInfo[] buttonInfos = new EventButtonInfo[3];

    protected override void EndEvent()
    {
        //  ボタン破棄
        for (int i = 0; i < BUTTON_COUNT; i++)
        {
            DestroyButton(i);
        }

        EventManager.instance.CloseEventUI();
    }

    protected override void EventUpdate()
    {
        
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

        button[0].onClick.AddListener(OnHealHpEvent);
        button[1].onClick.AddListener(OnAllHealHpEvent);
        button[2].onClick.AddListener(EndEvent);
    }

    private void DestroyButton(int num)
    {
        Destroy(button[num].gameObject);
    }

    //  最大HPの50パーセント回復
    private void OnHealHpEvent()
    {
        int healAmout = PlayerBase.instance.Status.maxHp / 2;
        PlayerBase.instance.Heal(healAmout);
        EndEvent();
    }

    //  HPを全快させる。ステータス補正を与える
    private void OnAllHealHpEvent()
    {
        int healAmout = PlayerBase.instance.Status.maxHp;
        PlayerBase.instance.Heal(healAmout);
        PlayerBase.instance.AddPermanentStatus(addStatus);
        EndEvent();
    }


}
