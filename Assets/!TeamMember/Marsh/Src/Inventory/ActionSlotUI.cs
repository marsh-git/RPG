using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionSlotUI : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler {

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text actionName;
    [SerializeField] private Button button;

    private ActionData actionData;

    public void Initialize(ActionData data) {
        Debug.Log($"icon : {icon}");
        Debug.Log($"actionName : {actionName}");
        Debug.Log($"button : {button}");

        actionData = data;

        icon.sprite = data.Icon;
        actionName.text = data.ActionName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        ActionUIManager.Instance.SelectAction(actionData);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        ActionDragManager.Instance.BeginDrag(actionData);
    }

    public void OnDrag(PointerEventData eventData) {

    }

    public void OnEndDrag(PointerEventData eventData) {
        ActionDragManager.Instance.EndDrag();
    }
}