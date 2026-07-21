using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/JobData")]
public class JobData : ScriptableObject
{
    [Header("役職の名前")]
    [SerializeField] public string jobName;
    [Header("役職を選んだ際の説明")]
    [TextArea(3, 6)] public string jobDescription;
    [Header("役職のステータス")]
    [SerializeField] public CharacterStatus status;

    [Header("役職の初期レリック(絶対適応しておけ)")]
    [SerializeField] public RelicDataBase jobRelic = null;

    //  アクションのデータ配列


    //  初期アクションデータ

}
