using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPHealItem : IItemEffect {
    /// <summary>
    /// アイテムの使用
    /// </summary>
    /// <param name="targetChara"></param>
    public void UseItem(CharacterBase targetChara) {
        targetChara.Heal(20);
    }
}
