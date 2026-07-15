using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct EventButtonInfo
{
    private const int MAIN_TEXT_CHILD = 0;
    private const int SUB_TEXT_CHILD = 1;

    [Header("ボタンのテキスト")]
    public string buttonText;

    [Header("効果説明")]
    public string subText;

    /// <summary>
    /// ボタンのテキストをセットする
    /// </summary>
    /// <param name="button"></param>
    public void SetButtonText(Transform button)
    {
        button.GetChild(MAIN_TEXT_CHILD).GetComponent<TextMeshProUGUI>().text = buttonText;
        button.GetChild(SUB_TEXT_CHILD).GetComponent<TextMeshProUGUI>().text = subText;
    }

}
