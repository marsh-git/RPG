using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/JobData")]
public class JobData : ScriptableObject
{

    [Header("Jobのステータス")]
    [SerializeField] public string jobName;
    [SerializeField] public int maxHp;
    [SerializeField] public int attack;
    [SerializeField] public int defense;
    [SerializeField] public int luck;

    //  アクションのデータ配列

    //  初期アクションデータ

}
