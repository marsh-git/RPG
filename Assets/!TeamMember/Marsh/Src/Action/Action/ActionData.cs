using UnityEngine;


public enum ActionTarget {
    Enemy,
    Ally,
    Self,
    Area
}

public enum AttackRangeType {
    Adjacent,      // 隣接1マス
    Circle,        // 周囲
    Line,          // 直線
    Cone,          // 扇形
    Cross,         // 十字
    Custom
}

public enum ActionAreaType {
    Single,     // 指定した1体
    Around,     // 指定地点の周囲
    SelfAround, // 自分の周囲
}

[CreateAssetMenu(fileName = "ActionData", menuName = "RPG/Action")]
public class ActionData : ScriptableObject {
    [Header("基本情報")]
    public string ActionName;
    public Sprite Icon;
    [TextArea(3, 5)]
    public string Description;

    [Header("性能")]
    public ElementType Element;
    public ActionTarget Target;

    public int Damage;

    [Header("攻撃範囲")]
    public AttackRangeType RangeType;
    public int Range;

    [Header("攻撃対象")]
    public ActionAreaType AreaType = ActionAreaType.Single;
    public int Area = 1;

    [Header("状態異常")]
    public StatusEffectData StatusEffect;

    [Range(0, 100)]
    public int StatusChance;

    [Header("実行")]
    public string ActionID;

    public string AttackRange {
        get {
            return RangeType switch {
                AttackRangeType.Adjacent => $"隣接{Range}マス",
                AttackRangeType.Line => $"直線{Range}マス",
                AttackRangeType.Circle => $"周囲{Range}マス",
                AttackRangeType.Cone => $"扇形{Range}マス",
                _ => $"{Range}マス"
            };
        }
    }
}