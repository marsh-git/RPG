using UnityEngine;

public class UIActivator : MonoBehaviour {
    [Header("表示・非表示を切り替える対象")]
    [SerializeField] private GameObject targetUI;

    [Header("操作キー")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    [Header("初期状態")]
    [SerializeField] private bool startVisible = false;

    private void Awake() {
        if (targetUI == null) {
            Debug.LogError($"{name}: Target UIが設定されていません。");
            return;
        }

        targetUI.SetActive(startVisible);
    }

    private void Update() {
        if (Input.GetKeyDown(toggleKey)) {
            ToggleUI();
        }
    }

    /// <summary>
    /// UIの表示・非表示を切り替える
    /// </summary>
    public void ToggleUI() {
        if (targetUI == null)
            return;

        targetUI.SetActive(!targetUI.activeSelf);
    }

    /// <summary>
    /// UIを表示する
    /// </summary>
    public void ShowUI() {
        if (targetUI == null)
            return;

        targetUI.SetActive(true);
    }

    /// <summary>
    /// UIを非表示にする
    /// </summary>
    public void HideUI() {
        if (targetUI == null)
            return;

        targetUI.SetActive(false);
    }
}