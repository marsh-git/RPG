using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HexRouteSearcher;

public class PlayerBase : CharacterBase, IClickable
{

    public static PlayerBase instance { get; private set; }

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

        instance = this;

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
    public void SetJob(int jobNum)
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
    /// クリックされたときの処理（移動範囲・攻撃可能範囲の計算とハイライト制御）
    /// </summary>
    public void OnClick() {
        var ClickableHighlight = ClickableSelectionManager.instance;
        var MovementManager = CharacterMovementManager.instance;
        // ハイライト削除
        ClickableHighlight.ClearHighlights();
        if(isSelect) {
            // 移動の片付け処理
            MovementManager.TeardownMovement();
            // 選択フラグの変更
            isSelect = false;
        } else {
            HexTileData targetTile = HexTileManager.instance.GetTileData(tileID);
            // プレイヤーがいるタイルのハイライト処理
            ClickableHighlight.OnTileHighlight(targetTile, false, eTileHighlight.PlayerHighlight);
            // 移動範囲とスキャン範囲を同時に取得
            MovementRangeResult rangeResult = HexRouteSearcher.CalculateMovementRange(targetTile, 3);
            // 移動可能マスのハイライト処理（
            List<HexTileData> movementTileList = new List<HexTileData>(rangeResult.MovableTiles);
            ClickableHighlight.HighlightRangeTile(movementTileList, false);
            // 敵のいるマスも含まれているリストを引数に渡して、攻撃対象を抽出
            HashSet<HexTileData> attackableSet = HexRouteSearcher.FindAttackableTilesInCandidates(targetTile, rangeResult.AttackableTiles, IsEnemy());
            List<HexTileData> attackableTileList = new List<HexTileData>(attackableSet);
            // 攻撃可能マスのハイライト処理
            ClickableHighlight.HighlightRangeTile(attackableTileList, false, eTileHighlight.BattleHighlight);
            // マネージャーへは実際に移動して止まれるマスだけを登録
            MovementManager.SetMovableTileList(movementTileList);
            MovementManager.AddMoveCharacter(this);

            isSelect = true;
        }
    }
    /// <summary>
    /// 移動終了処理
    /// </summary>
    public override void EndMove() {
        base.EndMove();
        isSelect = false;
    }
    /// <summary>
    /// 敵か判別
    /// </summary>
    /// <returns></returns>
    public override bool IsEnemy() {
        return false;
    }
}
