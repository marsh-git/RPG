using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropsDatabase", menuName = "ScriptableObject/Map/Crops Database")]
public class CropsDatabaseSO : ScriptableObject {
    [Header("全バイオーム共通の作物設定 (種/苗/収穫)")]
    public List<CropsVisual> cropsDataList = new List<CropsVisual>();

    /// <summary>
    /// IDに応じたオブジェクトを返す
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public CropsVisual GetCropsVisual(int ID) {
        if(CommonModule.IsEmpty(cropsDataList)) return new CropsVisual();

        for(int i = 0, max = cropsDataList.Count; i < max; i++) {
            CropsVisual cropsData = cropsDataList[i];
            if(cropsData.ID == ID) return cropsData;
        }
        return new CropsVisual();
    }
}