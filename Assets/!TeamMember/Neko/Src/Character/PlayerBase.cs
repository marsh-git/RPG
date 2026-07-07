using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : CharacterBase
{

    //  経験値変数
    private int exp = 0;
    private int lv = 1;
    private int needExp;
    private readonly int baseNeedExp = 50;
    private float needExpRatio = 1.5f;

    //  外部参照用
    public int Exp => exp;
    public int Lv => lv;
    public int NeedExp => needExp;

    //  職業
    [SerializeField] private JobData jobData = null;
    private JobManager jobManager = null;

    // プレイヤーのダイス
    private DiceManager diceManager;

    protected override void Awake()
    {
        base.Awake();

        jobManager = JobManager.instance;
        needExp = baseNeedExp;

        //  初期職業がnullなら初期職業に変更する
        if (jobData == null) SetJob(jobManager.START_JOB);
    }

    private void Update()
    {
        // 左クリックで経験値取得(デバッグ用)
        if (Input.GetMouseButtonDown(0))
        {
            GetExp(10);
            Debug.Log("現在のexp = " + Exp);
            Debug.Log("現在のlv = " + Lv);
            Debug.Log("次の経験値まで = " + NeedExp);
        }
        //  デバッグ用
        if (Input.GetMouseButtonDown(1))
        {
            SetJob(1);
        }
    }

    /// <summary>
    /// 敵から得れる経験値を引数に呼び出す
    /// </summary>
    /// <param name="getExp"></param>
    public void GetExp(int getExp)
    {
        exp += getExp;

        while(exp >= needExp)
        {
            Levelup();
        }
    }

    /// <summary>
    /// レベルアップ処理
    /// </summary>
    private void Levelup()
    {
        exp -= needExp;
        //  万が一経験値が0を下回ったら
        if(exp < 0) exp = 0;

        lv += 1;
        //  小数点以下切り捨て
        needExp = Mathf.FloorToInt(baseNeedExp * Mathf.Pow(lv, needExpRatio));
    }

    /// <summary>
    /// Jobをプレイヤーにセットする
    /// </summary>
    /// <param name="jobNum"></param>
    private void SetJob(int jobNum)
    {
        jobData = jobManager.GetJobData(jobNum);

        //  職業がnullなら初期職業に変更する
        if (jobData == null) SetJob(jobManager.START_JOB);

        SetStatus(jobData);
    }

    /// <summary>
    /// ステータスをセットする
    /// </summary>
    private void SetStatus(JobData jobData)
    {
        if (jobData == null) return;

        maxHp = jobData.maxHp;
        hp = maxHp;
        attack = jobData.attack;
        defense = jobData.defense;
        luck = jobData.luck;
    }

}
