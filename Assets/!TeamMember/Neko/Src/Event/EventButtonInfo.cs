using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct EventButtonInfo
{
    private const int MAIN_TEXT_CHILD = 0;
    private const int SUB_TEXT_CHILD = 1;

    [Header("ボタンのテキスト")]
    public string mainText;

    [Header("効果説明")]
    public string subText;

    /// <summary>
    /// ボタンのテキストをセットする
    /// </summary>
    /// <param name="button"></param>
    public void SetButtonText(Transform button)
    {
        button.GetChild(MAIN_TEXT_CHILD).GetComponent<TextMeshProUGUI>().text = mainText;
        button.GetChild(SUB_TEXT_CHILD).GetComponent<TextMeshProUGUI>().text = subText;
    }

    /// <summary>
    /// ボタンのテキストを引数を適応しながらセットする
    /// </summary>
    /// <param name="button"></param>
    /// <param name="args"></param>
    public void SetButtonText(Transform button, params object[] args)
    {
        button.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            string.Format(mainText, args);

        button.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            string.Format(subText, args);
    }

}
