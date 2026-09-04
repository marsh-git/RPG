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

        if (!attackableTiles.Contains(targetTile)) {
            Debug.Log("攻撃範囲外です");
            return;
        }

        CharacterBase target =
            CharacterManager.Instance.GetCharacter(targetTile.ID);

        if (target == null) {
            Debug.Log("攻撃対象がいません");
            return;
        }

        // 敵攻撃なら敵以外は対象外
        if (actionData.Target == ActionTarget.Enemy &&
            !target.IsEnemy()) {
            Debug.Log("敵ではありません");
            return;
        }

        ExecuteAttack(target);
    }

    private void ExecuteAttack(CharacterBase target) {
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