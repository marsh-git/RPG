using UnityEngine;

public class ActionUIManager : MonoBehaviour {
    public static ActionUIManager Instance { get; private set; }

    [SerializeField] private ActionDetailUI detailUI;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SelectAction(ActionData action) {
        detailUI.Show(action);
    }
}