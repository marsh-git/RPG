using UnityEngine;

/// <summary>
/// 作物の成長過程
/// </summary>
public enum eCropsProcess {
    None,    // 無し
    Seed,    // 植える（種・土壌）
    Sprout,  // 苗
    Harvest  // 作物（収穫可能）
}

/// <summary>
/// 作物の見た目データ
/// </summary>
[System.Serializable]
public struct CropsVisual {
    public eCropsProcess state;
    public GameObject prefab;
}