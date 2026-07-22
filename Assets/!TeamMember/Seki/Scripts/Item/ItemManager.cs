using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {
    public static ItemManager instance { get; private set; } = null;

    private List<ItemData> _itemDataList = null;

    private void Awake() {
        instance = this;
    }
}
