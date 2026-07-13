using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/JobData")]
public class JobData : ScriptableObject
{

    [Header("役職のステータス")]
    [SerializeField] public string jobName;
    [SerializeField] public int maxHp;
    [SerializeField] public int attack;
    [SerializeField] public int defense;
    [SerializeField] public int luck;

    [Header("役職の初期レリック(絶対適応しておけ)")]
    [SerializeField] public RelicDataBase jobRelic = null;

    //  アクションのデータ配列

    //  初期アクションデータ

}
