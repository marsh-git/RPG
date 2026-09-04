using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {
    public static AttackManager Instance { get; private set; }

    private PlayerBase attacker;
    private ActionData currentAction;

    // 現在選択可能なTile
    private List<HexTileData> attackableTiles = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 攻撃選択開始
    /// </summary>
    public void StartAttackSelection(
        PlayerBase player,
        ActionData action) {
        if (player == null || action == null)
            return;

        attacker = player;
        currentAction = action;

        ClickableSelectionManager.instance.ClearHighlights();

        HexTileData playerTile =
            HexTileManager.instance.GetTileData(
                player.GetTileID()
            );

        if (playerTile == null)
            return;

        // 攻撃範囲取得
        attackableTiles =
            GetAttackableTiles(playerTile, action);

        // ハイライト
        ClickableSelectionManager.instance.HighlightRangeTile(
            attackableTiles,
            false,
            eTileHighlight.BattleHighlight
        );

        Debug.Log(
            $"攻撃選択：{action.ActionName}"
        );
        Debug.Log(
            $"攻撃範囲：{action.AttackRange}"
        );
    }

    /// <summary>
    /// 攻撃対象を選択
    /// </summary>
    public void SelectTarget(HexTileData targetTile) {
        if (attacker == null ||
            currentAction == null ||
            targetTile == null) {
            return;
        }

        // 範囲外
        if (!attackableTiles.Contains(targetTile)) {
            Debug.Log("攻撃範囲外です");
            return;
        }

        CharacterBase target =
            CharacterManager.Instance.GetCharacter(
                targetTile.ID
            );

        if (target == null) {
            Debug.Log("対象がいません");
            return;
        }

        // 今回は敵のみ
        if (currentAction.Target == ActionTarget.Enemy) {
            if (!target.IsEnemy()) {
                Debug.Log("敵ではありません");
                return;
            }
        }

        ExecuteAttack(target);
    }

    /// <summary>
    /// 攻撃実行
    /// </summary>
    private void ExecuteAttack(CharacterBase target) {
        if (target == null || target.IsDead)
            return;

        // 技ダメージ + 攻撃力
        int damage =
            attacker.DamageCalculate(
                currentAction.Damage,
                attacker.GetActionStatus()
            );

        Debug.Log(
            $"{currentAction.ActionName} → {target.name}"
        );

        Debug.Log(
            $"攻撃ダメージ：{damage}"
        );

        // 防御力・属性相性を適用
        target.TakeDamage(
            damage,
            currentAction.Element
        );

        // TODO：状態異常
        // currentAction.StatusEffect
        // currentAction.StatusChance

        EndAttack();
    }

    /// <summary>
    /// 攻撃範囲取得
    /// </summary>
    private List<HexTileData> GetAttackableTiles(
        HexTileData playerTile,
        ActionData action) {
        List<HexTileData> result = new();

        switch (action.RangeType) {
            case AttackRangeType.Adjacent:

                result =
                    TileRangeExpansion.GetTilesWithinRadius(
                        playerTile,
                        1
                    );

                break;

            case AttackRangeType.Circle:

                result =
                    TileRangeExpansion.GetTilesWithinRadius(
                        playerTile,
                        action.Range
                    );

                break;

            case AttackRangeType.Line:

                result =
                    GetLineTiles(
                        playerTile,
                        action.Range
                    );

                break;

            case AttackRangeType.Cone:

                // 現在は仮
                result =
                    TileRangeExpansion.GetFanShapedTile(
                        playerTile,
                        eDirectionHex.UpRight
                    );

                break;

            case AttackRangeType.Cross:

                // 後で実装
                break;

            case AttackRangeType.Custom:

                // 後で実装
                break;
        }

        return result;
    }

    /// <summary>
    /// 6方向への直線
    /// ※現在は向きを持っていないため仮
    /// </summary>
    private List<HexTileData> GetLineTiles(
        HexTileData startTile,
        int range) {
        List<HexTileData> result = new();

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
            result.AddRange(
                TileRangeExpansion.GetNumDirTile(
                    startTile,
                    direction,
                    range
                )
            );
        }

        return result;
    }

    /// <summary>
    /// 攻撃終了
    /// </summary>
    private void EndAttack() {
        ClickableSelectionManager.instance.ClearHighlights();

        attacker = null;
        currentAction = null;
        attackableTiles.Clear();
    }

    /// <summary>
    /// 攻撃選択キャンセル
    /// </summary>
    public void CancelAttackSelection() {
        EndAttack();
    }

    public bool IsAttackSelecting() {
        return attacker != null &&
               currentAction != null;
    }
}