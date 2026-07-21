using UnityEngine;

/// <summary>
/// 作物の見た目データ
/// </summary>
[System.Serializable]
public struct ItemData {
    public int ID;
    public string name;
    public IItemEffect itemEffect;
}