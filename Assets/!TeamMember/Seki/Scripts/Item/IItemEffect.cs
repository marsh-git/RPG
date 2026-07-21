using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemEffect {
    /// <summary>
    /// アイテムの使用
    /// </summary>
    /// <param name="targetChara"></param>
    public void UseItem(CharacterBase targetChara) {

    }
}