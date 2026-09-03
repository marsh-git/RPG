using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class DiceSlotUI : MonoBehaviour, IDropHandler {
    [SerializeField] private Image diceImage;
    [SerializeField] private DiceSpriteDatabase database;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text actionName;

    private ActionData actionData;

    public void Initialize(int diceNumber) {
        diceImage.sprite = database.DiceSprites[diceNumber - 1];

        icon.enabled = false;
        actionName.gameObject.SetActive(false);
    }

    public void SetAction(ActionData action) {

        actionData = action;

        icon.sprite = action.Icon;
        icon.enabled = true;

        actionName.text = action.ActionName;
        actionName.gameObject.SetActive(true);
    }

    public void ClearAction() {
        actionData = null;

        icon.sprite = null;
        icon.enabled = false;

        actionName.text = "";
        actionName.gameObject.SetActive(false);
    }

    public ActionData GetAction() {
        return actionData;
    }

    public void OnDrop(PointerEventData eventData) {
        if (ActionDragManager.Instance.DraggingAction == null)
            return;

        SetAction(ActionDragManager.Instance.DraggingAction);
    }
}