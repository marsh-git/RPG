using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : CharacterBase, IClickable
{
    public bool isSelect { get; private set; } = false;
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

    /// <summary>
    /// 選択フラグの設定
    /// </summary>
    /// <param name="setFlag"></param>
    public void SetIsSelect(bool setFlag) {
        isSelect = setFlag;
    }
    /// <summary>
    /// クリックされたときの処理
    /// </summary>
    public void OnClick() {
        var ClickableHighlight = ClickableSelectionManager.instance;
        if(isSelect) {
            // ハイライトのクリア
            ClickableHighlight.ClearHighlights();
            // 選択フラグの変更
            isSelect = false;
        } else {
            List<HexTileData> movementTileList = new List<HexTileData>();
            // 現在いるマスを取得
            HexTileData targetTile = HexTileManager.instance.GetTileData(tileID);
            // 自身のマスを光らせる
            ClickableHighlight.OnTileHighlight(targetTile, true, eTileHighlight.PlayerHighlight);
            // 移動可能範囲の取得
            var rangeSet = HexRouteSearcher.CalculateMovementRange(targetTile, 3, false);
            movementTileList = new List<HexTileData>(rangeSet);
            // 移動可能範囲を光らせる
            ClickableHighlight.HighlightRangeTile(movementTileList, false);
            // 選択フラグの変更
            isSelect = true;
        }
    }
}
