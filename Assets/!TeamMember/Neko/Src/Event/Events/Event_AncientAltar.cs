using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/EventData/AncientAltar")]
public class Event_AncientAltar : EventDataBase
{
    private Button[] button;

    private readonly int BUTTON_COUNT = 2;

    [Header("イベントを決めるボタンのテキスト")]
    [SerializeField] EventButtonInfo[] buttonInfos = new EventButtonInfo[2];

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

        button[0].onClick.AddListener(AddMaxHP);
        button[1].onClick.AddListener(AddAttack);
    }

    private void DestroyButton(int num)
    {
        Destroy(button[num].gameObject);
    }

    private void AddMaxHP()
    {
        addStatus.maxHp = 5;
        PlayerBase.instance.AddPermanentStatus(addStatus);
        PlayerBase.instance.Heal(addStatus.maxHp);
        EndEvent();
    }

    private void AddAttack()
    {
        addStatus.maxHp = -5;
        addStatus.attack = 3;
        PlayerBase.instance.AddPermanentStatus(addStatus);
        PlayerBase.instance.TakeDamage(addStatus.maxHp);
        EndEvent();
    }

}
