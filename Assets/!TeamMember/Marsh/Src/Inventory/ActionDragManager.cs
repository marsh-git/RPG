using UnityEngine;
using UnityEngine.UI;

public class ActionDragManager : MonoBehaviour {
    public static ActionDragManager Instance { get; private set; }

    [SerializeField] private Image dragIcon;

    public ActionData DraggingAction { get; private set; }

    private RectTransform iconRect;

    private void Awake() {
        Instance = this;
        iconRect = dragIcon.rectTransform;
        dragIcon.gameObject.SetActive(false);
    }

    private void Update() {
        if (dragIcon.gameObject.activeSelf) {
            iconRect.position = Input.mousePosition;
        }
    }

    public void BeginDrag(ActionData action) {
        DraggingAction = action;

        dragIcon.sprite = action.Icon;
        dragIcon.gameObject.SetActive(true);
    }

    public void EndDrag() {
        DraggingAction = null;

        dragIcon.gameObject.SetActive(false);
    }
}