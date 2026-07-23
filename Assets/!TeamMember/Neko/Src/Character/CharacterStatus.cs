using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

[System.Serializable]
public struct CharacterStatus
{
    public int maxHp;
    public int attack;
    public int defense;
    public int luck;
    public int radius;

    public void Add(CharacterStatus status)
    {
        maxHp += status.maxHp;
        attack += status.attack;
        defense += status.defense;
        luck += status.luck;
        radius += status.radius;
    }

    public void Reset()
    {
        maxHp = 0;
        attack = 0;
        defense = 0;
        luck = 0;
        radius = 0;
    }
}
