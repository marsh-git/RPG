using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntRange {
    public int min;
    public int max;

    public IntRange(int minValue, int maxValue) {
        min = minValue;
        max = maxValue;
    }
    /// <summary>
    /// 全範囲を返す
    /// </summary>
    public static IntRange Full => new IntRange(int.MinValue, int.MaxValue);
    /// <summary>
    /// 値が範囲内にあるか判定
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(int value) => value >= min && value <= max;
    /// <summary>
    /// 乱数取得
    /// </summary>
    /// <returns></returns>
    public int Random() => UnityEngine.Random.Range(min, max + 1);
    /// <summary>
    /// シード値内での乱数取得
    /// </summary>
    /// <param name="rand"></param>
    /// <returns></returns>
    public int SeedRandom(System.Random rand) => rand.Next(min, max + 1);
}
