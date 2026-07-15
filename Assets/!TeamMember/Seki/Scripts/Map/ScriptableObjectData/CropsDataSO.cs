using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropsDatabase", menuName = "Map/Crops Database")]
public class CropsDatabaseSO : ScriptableObject {
    [Header("全バイオーム共通の作物設定 (種/苗/収穫)")]
    public List<CropsVisual> cropsPhases = new List<CropsVisual>();

    /// <summary>
    /// 指定された成長段階のプレハブを高速検索
    /// </summary>
    public GameObject GetCropsPrefab(eCropsProcess state) {
        for(int i = 0; i < cropsPhases.Count; i++) {
            if(cropsPhases[i].state == state) return cropsPhases[i].prefab;
        }
        return null;
    }
}