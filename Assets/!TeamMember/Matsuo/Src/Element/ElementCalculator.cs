using UnityEngine;

/// <summary>
/// 属性相性の計算
/// </summary>
public static class ElementCalculator
{
    /// <summary>
    /// 属性倍率を取得
    /// </summary>
    /// <param name="attack">攻撃属性</param>
    /// <param name="defense">防御属性</param>
    /// <returns>2.0：有利　0.5：不利　1.0：等倍</returns>
    public static float GetRate(ElementType attack, ElementType defense)
    {
        switch (attack)
        {
            case ElementType.Fire:
                if (defense == ElementType.Ice || defense == ElementType.Nature)
                    return 2f;
                if (defense == ElementType.Water || defense == ElementType.Earth)
                    return 0.5f;
                break;

            case ElementType.Water:
                if (defense == ElementType.Fire || defense == ElementType.Earth)
                    return 2f;
                if (defense == ElementType.Thunder || defense == ElementType.Nature)
                    return 0.5f;
                break;

            case ElementType.Ice:
                if (defense == ElementType.Earth || defense == ElementType.Nature)
                    return 2f;
                if (defense == ElementType.Fire || defense == ElementType.Thunder)
                    return 0.5f;
                break;

            case ElementType.Thunder:
                if (defense == ElementType.Water || defense == ElementType.Ice)
                    return 2f;
                if (defense == ElementType.Earth || defense == ElementType.Nature)
                    return 0.5f;
                break;

            case ElementType.Earth:
                if (defense == ElementType.Fire || defense == ElementType.Thunder)
                    return 2f;
                if (defense == ElementType.Water || defense == ElementType.Ice)
                    return 0.5f;
                break;

            case ElementType.Nature:
                if (defense == ElementType.Water || defense == ElementType.Earth)
                    return 2f;
                if (defense == ElementType.Fire || defense == ElementType.Ice)
                    return 0.5f;
                break;

            case ElementType.Light:
                if (defense == ElementType.Dark)
                    return 2f;
                break;

            case ElementType.Dark:
                if (defense == ElementType.Light)
                    return 2f;
                break;
        }

        return 1f;
    }
}