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
    public int ID;
    public string name;
    public GameObject seed;
    public GameObject sprout;
    public GameObject harvest;

    /// <summary>
    /// オブジェクトを返す
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetPrefab(eCropsProcess type) {
        switch(type) {
            case eCropsProcess.Seed:
            return seed;
            case eCropsProcess.Sprout:
            return sprout;
            case eCropsProcess.Harvest:
            return harvest;
        }
        return null;
    }
}