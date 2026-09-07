using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {
    public static AttackManager Instance { get; private set; }

    private PlayerBase attacker;
    private ActionData actionData;

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

    public void StartAttackSelection(PlayerBase player, ActionData action) {
        if (player == null || action == null) return;

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

    private void CalculateAttackRange() {
        attackableTiles.Clear();

        HexTileData center =
            HexTileManager.instance.GetTileData(attacker.GetTileID());

        if (center == null) return;

        switch (actionData.RangeType) {
            case AttackRangeType.Adjacent:
                attackableTiles.AddRange(
                    TileRangeExpansion.GetTilesWithinRadius(
                        center,
                        1
                    )
                );
                break;

            case AttackRangeType.Circle:
                attackableTiles.AddRange(
                    TileRangeExpansion.GetTilesWithinRadius(
                        center,
                        actionData.Range
                    )
                );
                break;

            case AttackRangeType.Line:
                foreach (eDirectionHex dir in new[]
                {
                    eDirectionHex.UpRight,
                    eDirectionHex.Right,
                    eDirectionHex.DownRight,
                    eDirectionHex.DownLeft,
                    eDirectionHex.Left,
                    eDirectionHex.UpLeft
                }) {
                    attackableTiles.AddRange(
                        TileRangeExpansion.GetNumDirTile(
                            center,
                            dir,
                            actionData.Range
                        )
                    );
                }
                break;
        }
    }

    public void SelectTarget(HexTileData targetTile) {
        if (!isAttackSelecting) return;
        if (targetTile == null) return;

        // 自分中心の範囲攻撃
        if (actionData.AreaType == ActionAreaType.SelfAround) {
            HexTileData selfTile =
                HexTileManager.instance.GetTileData(attacker.GetTileID());

            ExecuteAreaAttack(selfTile);
            return;
        }

        if (!attackableTiles.Contains(targetTile)) {
            Debug.Log("攻撃範囲外です");
            return;
        }

        // 指定地点を中心とした範囲攻撃
        if (actionData.AreaType == ActionAreaType.Around) {
            ExecuteAreaAttack(targetTile);
            return;
        }

        // 単体攻撃
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

        ExecuteAttack((EnemyBase)target);
    }

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

    private void ExecuteAreaAttack(HexTileData centerTile) {
        List<HexTileData> areaTiles =
            TileRangeExpansion.GetTilesWithinRadius(
                centerTile,
                actionData.Area
            );

        // 中心マスも対象にする
        areaTiles.Add(centerTile);

        List<EnemyBase> targets = new();

        foreach (HexTileData tile in areaTiles) {
            CharacterBase character =
                CharacterManager.Instance.GetCharacter(tile.ID);

            if (character == null)
                continue;

            if (actionData.Target == ActionTarget.Enemy &&
                !character.IsEnemy())
                continue;

            EnemyBase enemy = character as EnemyBase;

            if (enemy != null) {
                targets.Add(enemy);
            }
        }

        // 範囲内に対象がいない
        if (targets.Count == 0) {
            Debug.Log(
                $"「{actionData.ActionName}」：範囲内に攻撃対象がいないため攻撃をスキップします。"
            );

            EndAttack();
            return;
        }

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

    private void EndAttack() {
        isAttackSelecting = false;

        attackableTiles.Clear();

        ClickableSelectionManager.instance.ClearHighlights();

        attacker = null;
        actionData = null;

        TurnManager.Instance.EndPlayerTurn();
    }

    public void CancelAttackSelection() {
        isAttackSelecting = false;

        attackableTiles.Clear();

        ClickableSelectionManager.instance.ClearHighlights();

        attacker = null;
        actionData = null;
    }
}