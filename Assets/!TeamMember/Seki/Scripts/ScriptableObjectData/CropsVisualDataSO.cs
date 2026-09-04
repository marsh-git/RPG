using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropsData", menuName = "ScriptableObject/Map/Crops DataBase")]
public class CropsVisualDataSO : ScriptableObject {
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