using UnityEngine;

public class ActionInventoryUI : MonoBehaviour {
    [SerializeField] private ActionDatabase database;
    [SerializeField] private Transform content;
    [SerializeField] private ActionSlotUI slotPrefab;

    private void Start() {
        foreach (var action in database.Actions) {
            var slot = Instantiate(slotPrefab, content);
            slot.Initialize(action);
        }
    }
}