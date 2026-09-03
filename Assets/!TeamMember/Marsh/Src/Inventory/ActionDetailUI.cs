using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionDetailUI : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Image icon;

    [SerializeField] private TMP_Text actionName;
    [SerializeField] private TMP_Text element;
    [SerializeField] private TMP_Text damage;
    [SerializeField] private TMP_Text statusEffect;
    [SerializeField] private TMP_Text attackRange;
    [SerializeField] private TMP_Text description;

    public void Show(ActionData data) {
        icon.sprite = data.Icon;

        actionName.text = data.ActionName;

        element.text = $"属性：{data.Element}";

        damage.text = $"ダメージ：{data.Damage}";

        attackRange.text = $"攻撃範囲：{data.AttackRange}";

        if (data.StatusEffect != null)
            statusEffect.text = $"状態異常：{data.StatusEffect.EffectName}";
        else
            statusEffect.text = "状態異常：なし";

        description.text = $"{data.Description}";
    }

    public void Clear() {
        icon.sprite = null;

        actionName.text = "";
        element.text = "";
        damage.text = "";
        statusEffect.text = "";
        attackRange.text = "";
        description.text = "";
    }
}