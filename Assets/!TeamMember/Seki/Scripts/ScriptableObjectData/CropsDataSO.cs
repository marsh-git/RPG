using System.Collections.Generic;
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
        switch (type) {
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

[CreateAssetMenu(fileName = "CropsData", menuName = "ScriptableObject/Map/Crops DataBase")]
public class CropsDataSO : ScriptableObject {
    [Header("全バイオーム共通の作物設定 (種/苗/収穫)")]
    [SerializeField] private List<CropsVisual> cropsDataList = new List<CropsVisual>();

    /// <summary>
    /// 指定した作物IDと成長段階に応じたPrefabを取得する
    /// </summary>
    public GameObject GetCropsPrefab(int cropsID, eCropsProcess process) {
        if(CommonModule.IsEmpty(cropsDataList)) return null;

        CropsVisual visual = cropsDataList.Find(c => c.ID == cropsID);
        return visual.GetPrefab(process);
    }
}