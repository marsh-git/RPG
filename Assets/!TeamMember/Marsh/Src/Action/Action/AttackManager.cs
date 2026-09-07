using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {
    public static AttackManager Instance { get; private set; }

    private PlayerBase attacker;
    private ActionData actionData;

    // 現在選択可能なマス
    private readonly List<HexTileData> attackableTiles = new();

    private bool isAttackSelecting = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public bool IsAttackSelecting() {
        return isAttackSelecting;
    }

    /// <summary>
    /// 攻撃選択開始
    /// </summary>
    public void StartAttackSelection(PlayerBase player, ActionData action) {
        if (player == null || action == null)
            return;

        attacker = player;
        actionData = action;

        isAttackSelecting = true;

        CalculateAttackRange();

        ClickableSelectionManager.instance.ClearHighlights();

        ClickableSelectionManager.instance.HighlightRangeTile(
            attackableTiles,
            false,
            eTileHighlight.BattleHighlight
        );

        Debug.Log($"攻撃選択開始：{action.ActionName}");
    }

    /// <summary>
    /// 攻撃可能範囲を計算
    /// </summary>
    private void CalculateAttackRange() {
        attackableTiles.Clear();

        HexTileData center =
            HexTileManager.instance.GetTileData(attacker.GetTileID());

        if (center == null)
            return;

        switch (actionData.AreaType) {
            case ActionAreaType.Single:

                if (actionData.RangeType == AttackRangeType.Adjacent) {
                    attackableTiles.AddRange(
                        TileRangeExpansion.GetTilesWithinRadius(
                            center,
                            1
                        )
                    );
                }
                else if (actionData.RangeType == AttackRangeType.Circle) {
                    attackableTiles.AddRange(
                        TileRangeExpansion.GetTilesWithinRadius(
                            center,
                            actionData.Range
                        )
                    );
                }
                else if (actionData.RangeType == AttackRangeType.Line) {
                    AddDirectionTiles(center);
                }

                break;


            case ActionAreaType.Around:

                attackableTiles.AddRange(
                    TileRangeExpansion.GetTilesWithinRadius(
                        center,
                        actionData.Range
                    )
                );

                break;


            case ActionAreaType.DirectionLine:

                AddDirectionTiles(center);

                break;


            case ActionAreaType.DirectionCone:

                // 扇形の方向を選択するため、隣接6マスを選択
                foreach (eDirectionHex direction in GetAllDirections()) {
                    HexTileData tile =
                        HexTileManager.instance.GetToDirTile(
                            center.gridPosX,
                            center.gridPosY,
                            direction
                        );

                    if (tile != null)
                        attackableTiles.Add(tile);
                }

                break;


            case ActionAreaType.SelfAround:

                // 自分自身を選択
                attackableTiles.Add(center);

                break;


            case ActionAreaType.Cross:

                // 自分自身を選択
                attackableTiles.Add(center);

                break;
        }
    }

    /// <summary>
    /// マスをクリックしたとき
    /// </summary>
    public void SelectTarget(HexTileData targetTile) {
        if (!isAttackSelecting)
            return;

        if (targetTile == null)
            return;

        // 攻撃範囲外
        if (!attackableTiles.Contains(targetTile)) {
            Debug.Log("攻撃範囲外です");
            return;
        }

        // 指定地点を中心とした範囲攻撃
        if (actionData.AreaType == ActionAreaType.Around) {
            ExecuteAreaAttack(targetTile);
            return;
        }

        // 指定方向の直線攻撃
        if (actionData.AreaType == ActionAreaType.DirectionLine) {
            ExecuteDirectionLineAttack(targetTile);
            return;
        }

        // 指定方向の扇形攻撃
        if (actionData.AreaType == ActionAreaType.DirectionCone) {
            ExecuteDirectionConeAttack(targetTile);
            return;
        }

        if (actionData.AreaType == ActionAreaType.SelfAround) {
            ExecuteAreaAttack(targetTile);
            return;
        }

        if (actionData.AreaType == ActionAreaType.Cross) {
            ExecuteCrossAttack(targetTile);
            return;
        }

        // 単体攻撃
        ExecuteSingleAttack(targetTile);
    }

    /// <summary>
    /// 単体攻撃
    /// </summary>
    private void ExecuteSingleAttack(HexTileData targetTile) {
        CharacterBase target =
            CharacterManager.Instance.GetCharacter(targetTile.ID);

        if (target == null) {
            Debug.Log(
                $"「{actionData.ActionName}」：攻撃対象がいないため攻撃をスキップします。"
            );

            EndAttack();
            return;
        }

        if (actionData.Target == ActionTarget.Enemy &&
            !target.IsEnemy()) {
            Debug.Log("敵ではありません");
            return;
        }

        EnemyBase enemy = target as EnemyBase;

        if (enemy == null) {
            Debug.Log("攻撃対象がEnemyではありません");
            EndAttack();
            return;
        }

        ExecuteAttack(enemy);
    }

    /// <summary>
    /// 単体攻撃を実行
    /// </summary>
    private void ExecuteAttack(EnemyBase target) {
        int damage =
            attacker.DamageCalculate(
                actionData.Damage,
                attacker.GetActionStatus()
            );

        target.TakeDamage(
            damage,
            actionData.Element
        );

        Debug.Log(
            $"{attacker.name} → {target.name} : {damage}ダメージ"
        );

        EndAttack();
    }

    /// <summary>
    /// 指定地点を中心とした範囲攻撃
    /// </summary>
    private void ExecuteAreaAttack(HexTileData centerTile) {
        if (centerTile == null) {
            EndAttack();
            return;
        }

        List<HexTileData> areaTiles =
            TileRangeExpansion.GetTilesWithinRadius(
                centerTile,
                actionData.Area
            );

        // 中心マスも対象にする
        if (!areaTiles.Contains(centerTile)) {
            areaTiles.Add(centerTile);
        }

        List<EnemyBase> targets = GetEnemyTargets(areaTiles);

        if (targets.Count == 0) {
            Debug.Log(
                $"「{actionData.ActionName}」：範囲内に攻撃対象がいないため攻撃をスキップします。"
            );

            EndAttack();
            return;
        }

        ExecuteDamage(targets);
    }

    /// <summary>
    /// 指定方向の直線範囲をすべて攻撃
    /// </summary>
    private void ExecuteDirectionLineAttack(HexTileData targetTile) {
        HexTileData center =
            HexTileManager.instance.GetTileData(
                attacker.GetTileID()
            );

        if (center == null) {
            EndAttack();
            return;
        }

        eDirectionHex direction =
            GetDirection(center, targetTile);

        if (direction == eDirectionHex.Invalid) {
            Debug.Log("攻撃方向を取得できませんでした");
            return;
        }

        List<HexTileData> lineTiles =
            TileRangeExpansion.GetNumDirTile(
                center,
                direction,
                actionData.Range
            );

        List<EnemyBase> targets = GetEnemyTargets(lineTiles);

        if (targets.Count == 0) {
            Debug.Log(
                $"「{actionData.ActionName}」：直線範囲内に攻撃対象がいないため攻撃をスキップします。"
            );

            EndAttack();
            return;
        }

        ExecuteDamage(targets);
    }

    /// <summary>
    /// 指定方向の扇形範囲をすべて攻撃
    /// </summary>
    private void ExecuteDirectionConeAttack(HexTileData targetTile) {
        HexTileData center =
            HexTileManager.instance.GetTileData(
                attacker.GetTileID()
            );

        if (center == null) {
            EndAttack();
            return;
        }

        eDirectionHex direction =
            GetDirection(center, targetTile);

        if (direction == eDirectionHex.Invalid) {
            Debug.Log("攻撃方向を取得できませんでした");
            return;
        }

        List<HexTileData> coneTiles =
            TileRangeExpansion.GetFanShapedTile(
                center,
                direction,
                actionData.Range
            );

        List<EnemyBase> targets = GetEnemyTargets(coneTiles);

        if (targets.Count == 0) {
            Debug.Log(
                $"「{actionData.ActionName}」：扇形範囲内に攻撃対象がいないため攻撃をスキップします。"
            );

            EndAttack();
            return;
        }

        ExecuteDamage(targets);
    }

    /// <summary>
    /// 自分中心の十字範囲をすべて攻撃
    /// </summary>
    private void ExecuteCrossAttack(HexTileData centerTile) {
        if (centerTile == null) {
            EndAttack();
            return;
        }

        List<HexTileData> crossTiles = new();

        // 6方向へRange分伸ばす
        eDirectionHex[] directions =
        {
            eDirectionHex.UpRight,
            eDirectionHex.Right,
            eDirectionHex.DownRight,
            eDirectionHex.DownLeft,
            eDirectionHex.Left,
            eDirectionHex.UpLeft
        };

        foreach (eDirectionHex direction in directions) {
            crossTiles.AddRange(
                TileRangeExpansion.GetNumDirTile(
                    centerTile,
                    direction,
                    actionData.Range
                )
            );
        }

        // 中心マスも含める
        if (!crossTiles.Contains(centerTile)) {
            crossTiles.Add(centerTile);
        }

        List<EnemyBase> targets = GetEnemyTargets(crossTiles);

        if (targets.Count == 0) {
            Debug.Log(
                $"「{actionData.ActionName}」：十字範囲内に攻撃対象がいないため攻撃をスキップします。"
            );

            EndAttack();
            return;
        }

        ExecuteDamage(targets);
    }

    /// <summary>
    /// タイル一覧から敵だけを取得
    /// </summary>
    private List<EnemyBase> GetEnemyTargets(
        List<HexTileData> tiles) {
        List<EnemyBase> targets = new();

        foreach (HexTileData tile in tiles) {
            if (tile == null)
                continue;

            CharacterBase character =
                CharacterManager.Instance.GetCharacter(tile.ID);

            if (character == null)
                continue;

            if (actionData.Target == ActionTarget.Enemy &&
                !character.IsEnemy()) {
                continue;
            }

            EnemyBase enemy = character as EnemyBase;

            if (enemy != null && !targets.Contains(enemy)) {
                targets.Add(enemy);
            }
        }

        return targets;
    }

    /// <summary>
    /// 複数の敵へダメージ
    /// </summary>
    private void ExecuteDamage(List<EnemyBase> targets) {
        int damage =
            attacker.DamageCalculate(
                actionData.Damage,
                attacker.GetActionStatus()
            );

        foreach (EnemyBase target in targets) {
            target.TakeDamage(
                damage,
                actionData.Element
            );

            Debug.Log(
                $"{attacker.name} → {target.name} : {damage}ダメージ"
            );
        }

        EndAttack();
    }

    /// <summary>
    /// 中心タイルから対象タイルへの方向を取得
    /// </summary>
    private eDirectionHex GetDirection(
        HexTileData center,
        HexTileData target) {
        int dx = target.gridPosX - center.gridPosX;
        int dy = target.gridPosY - center.gridPosY;

        // 右上
        if (dx == 0 && dy > 0)
            return eDirectionHex.UpRight;

        // 右
        if (dx > 0 && dy == 0)
            return eDirectionHex.Right;

        // 右下
        if (dx > 0 && dy < 0)
            return eDirectionHex.DownRight;

        // 左下
        if (dx == 0 && dy < 0)
            return eDirectionHex.DownLeft;

        // 左
        if (dx < 0 && dy == 0)
            return eDirectionHex.Left;

        // 左上
        if (dx < 0 && dy > 0)
            return eDirectionHex.UpLeft;

        return eDirectionHex.Invalid;
    }

    private void AddDirectionTiles(HexTileData center) {
        foreach (eDirectionHex direction in GetAllDirections()) {
            attackableTiles.AddRange(
                TileRangeExpansion.GetNumDirTile(
                    center,
                    direction,
                    actionData.Range
                )
            );
        }
    }

    private eDirectionHex[] GetAllDirections() {
        return new[]
        {
        eDirectionHex.UpRight,
        eDirectionHex.Right,
        eDirectionHex.DownRight,
        eDirectionHex.DownLeft,
        eDirectionHex.Left,
        eDirectionHex.UpLeft
    };
    }

    /// <summary>
    /// 攻撃終了
    /// </summary>
    private void EndAttack() {
        isAttackSelecting = false;

        attackableTiles.Clear();

        ClickableSelectionManager.instance.ClearHighlights();

        attacker = null;
        actionData = null;

        TurnManager.Instance.EndPlayerTurn();
    }

    /// <summary>
    /// 攻撃選択キャンセル
    /// </summary>
    public void CancelAttackSelection() {
        isAttackSelecting = false;

        attackableTiles.Clear();

        ClickableSelectionManager.instance.ClearHighlights();

        attacker = null;
        actionData = null;
    }
}