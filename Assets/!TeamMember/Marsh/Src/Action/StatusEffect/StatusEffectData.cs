using UnityEngine;

public enum StatusEffectType {
    None,

    Stun,
    Burn,
    Freeze,
    Poison,
    Curse,
    Slow
}

[CreateAssetMenu(fileName = "StatusEffect", menuName = "RPG/Status Effect")]
public class StatusEffectData : ScriptableObject {
    [Header("基本情報")]
    public string EffectName;
    public Sprite Icon;

    [TextArea]
    public string Description;

    public StatusEffectType Type;

    [Header("効果")]
    public int Duration;

    public int DamagePerTurn;

    public float DamageMultiplier = 1f;

    public int MoveLimit = -1;

    public bool ForceDiceToOne;

    public bool IgnoreFireEnemy;
}