using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HexRouteSearcher;

public class PlayerBase : CharacterBase, IClickable
{

    public static PlayerBase instance { get; private set; }

    //  経験値変数
    private int lv = 1;
    //private int exp = 0;
    //private int needExp;
    //private readonly int BASE_NEED_EXP = 50;
    //private float needExpRatio = 1.5f;

    [Header("▼　現在の職業　▼")]
    [SerializeField] private JobData jobData = null;
    private JobManager jobManager = null;

    [Header("▼　所持遺物一覧　▼")]
    [SerializeField] private List<RelicDataBase> currentRelic = new();

    //  参照するためだけのステータス
    public CharacterStatus Status => status;
    //  一時的バフ、デバフ
    private CharacterStatus temporaryStatus;
    //  永続バフ、デバフ(レリックは適応しない。イベントなどで貰えるバフ、デバフ)
    private CharacterStatus permanentStatus;

    // プレイヤーのダイス
    private DiceManager diceManager;

    protected override void Awake()
    {
        base.Awake();

        instance = this;

        jobManager = JobManager.instance;

        //  初期職業がnullなら初期職業に変更する
        if (jobData == null) SetJob(jobManager.START_JOB);
    }

    /// <summary>
    /// 敵から得れる経験値を引数に呼び出す
    /// </summary>
    /// <param name="getExp"></param>
    //public void GetExp(int getExp)
    //{
    //    exp += getExp;

    //    while(exp >= needExp)
    //    {
    //        Levelup();
    //    }
    //}

    /// <summary>
    /// レベルアップ処理
    /// </summary>
    public void Levelup()
    {
        //exp -= needExp;
        ////  万が一経験値が0を下回ったら
        //if(exp < 0) exp = 0;

        lv += 1;
        //  小数点以下切り捨て
        //needExp = Mathf.FloorToInt(BASE_NEED_EXP * Mathf.Pow(lv, needExpRatio));
    }

    /// <summary>
    /// 経験値とレベルをリセットする
    /// </summary>
    private void ResetExpAndLevel()
    {
        //exp = 0;
        //needExp = BASE_NEED_EXP;
        lv = 1;
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

        //  経験値とレベルをリセット
        ResetExpAndLevel();

        //  役職のステータスをセットする
        SetJobStatus();
    }

    /// <summary>
    /// 役職のステータスをセットする
    /// </summary>
    private void SetJobStatus()
    {
        if (jobData == null) return;

        //  ステータス更新
        RefreshStatus();

        //  hpを全快させる
        hp = status.maxHp;
    }

    /// <summary>
    /// ステータスを更新する
    /// </summary>
    private void RefreshStatus()
    {
        //  ジョブのステータスに書き換え
        status = jobData.status;

        //  レリックのバフを再適応する
        ReconfigureRelicsStatus();

        //  永続バフを適応
        status.Add(permanentStatus);
    }

    /// <summary>
    /// レリックのバフを再適応する(ジョブ入れ替え時のステータスに入れ込む)
    /// </summary>
    private void ReconfigureRelicsStatus()
    {
        //  初期の遺物は配列0番目固定にする(初回生成時は追加)
        if(currentRelic.Count == 0)
        {
            currentRelic.Add(jobData.jobRelic);
        }
        else {
            currentRelic[0] = jobData.jobRelic;
        }

        for(int i = 0; i < currentRelic.Count; i++)
        {
            RelicDataBase relics = currentRelic[i];
            status.Add(relics.status);
        }
    }

    /// <summary>
    /// レリックを新規取得時に適応する
    /// </summary>
    /// <param name="addRelic"></param>
    public void AddRelic(RelicDataBase addRelic)
    {
        currentRelic.Add(addRelic);
        RefreshStatus();
    }

    /// <summary>
    /// 永続バフを追加する
    /// </summary>
    /// <param name="addStatus"></param>
    public void AddPermanentStatus(CharacterStatus addStatus)
    {
        permanentStatus.Add(addStatus);
        RefreshStatus();
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
