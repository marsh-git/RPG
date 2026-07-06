using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/TilePrefabData")]
public class TileObjectData : ScriptableObject{
    /// <summary>
    /// 地形、バイオームごとのプレファブを取り出して、オブジェクトの管理をしやすくしたい
    /// </summary>
    [Header("地形ごとのPrefab")]
    public string tileName = null;
}
