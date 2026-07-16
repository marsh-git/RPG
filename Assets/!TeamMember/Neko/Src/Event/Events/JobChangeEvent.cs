using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/EventData/JobChange")]
public class JobChangeEvent : EventDataBase
{

    private Button[] button;

    private readonly int BUTTON_COUNT = 4;

    [Header("ジョブを決めるボタンのテキスト")]
    [SerializeField] private string selectJobButtonText1 = "";
    [SerializeField] private string selectJobButtonText2 = "";
    [SerializeField] private string selectJobButtonText3 = "";

    private readonly string selectJobCancelButtonText = "No";

    [Header("転職する役職の番号（JobManagerから参照）")]
    [SerializeField] private int JobNum1 = 0;
    [SerializeField] private int JobNum2 = 0;
    [SerializeField] private int JobNum3 = 0;

    protected override void EventUpdate()
    {
        
    }

    /// <summary>
    /// イベント終了
    /// </summary>
    protected override void EndEvent()
    {
        //  ボタン破棄
        for (int i = 0; i < button.Length; i++)
        {
            if (button[i] != null) DestroyButton(i);
        }

        EventManager.instance.CloseEventUI();
    }

    /// <summary>
    /// イベントをセットする
    /// </summary>
    /// <param name="eventUI"></param>
    public override void SetEventUI(Canvas eventUI)
    {
        base.SetEventUI(eventUI);

        //  ボタン追加
        button = new Button[BUTTON_COUNT];
        for(int i = 0; i < BUTTON_COUNT; i++)
        {
            button[i] = SetButton();
        }

        button[0].GetComponentInChildren<TextMeshProUGUI>().text = selectJobButtonText1;
        button[0].onClick.AddListener(OnJobChangeEvent1);
        button[1].GetComponentInChildren<TextMeshProUGUI>().text = selectJobButtonText2;
        button[1].onClick.AddListener(OnJobChangeEvent2);
        button[2].GetComponentInChildren<TextMeshProUGUI>().text = selectJobButtonText3;
        button[2].onClick.AddListener(OnJobChangeEvent3);
        button[3].GetComponentInChildren<TextMeshProUGUI>().text = selectJobCancelButtonText;
        button[3].onClick.AddListener(OnEventCancel);

    }

    private void DestroyButton(int num)
    {
        Destroy(button[num].gameObject);
    }

    //  ここから下は職業を変更するイベント関数
    private void OnJobChangeEvent1()
    {
        PlayerBase.instance.SetJob(JobNum1);
        EndEvent();
    }

    private void OnJobChangeEvent2()
    {
        PlayerBase.instance.SetJob(JobNum2);
        EndEvent();
    }

    private void OnJobChangeEvent3()
    {
        PlayerBase.instance.SetJob(JobNum3);
        EndEvent();
    }

    private void OnEventCancel()
    {
        EndEvent();
    }

}
