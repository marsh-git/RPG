using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUICanvas : MonoBehaviour
{
    PlayerBase player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerBase.instance;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshProUGUI>().SetText(
            "maxHp = " + player.Status.maxHp +
            "\nHP = " + player.HP +
            "\nattack = " + player.Status.attack +
            "\ndefence = " + player.Status.defense +
            "\nluck = " + player.Status.luck +
            "\nradius = " + player.Status.radius +
            "\ncoin = " + player.Coin);
    }
}
