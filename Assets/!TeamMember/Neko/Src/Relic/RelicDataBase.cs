using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicDataBase : ScriptableObject
{
    [Header("レリックの名前")]
    [SerializeField] public string relicName;

    [Header("レリックのイメージ画像")]
    [SerializeField] public Sprite relicImg;

    [Header("イベント説明")]
    [TextArea(3, 6)] public string relicDescription;

    [Header("ステータス代入（なければ0）")]
    [SerializeField] public CharacterStatus status;


}
