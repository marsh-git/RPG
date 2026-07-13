using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicDataBase : ScriptableObject
{
    [Header("レリックの名前")]
    [SerializeField] public string relicName;

    [Header("ステータス代入（なければ0）")]
    [SerializeField] public int maxHp;
    [SerializeField] public int attack;
    [SerializeField] public int defense;
    [SerializeField] public int luck;

}
