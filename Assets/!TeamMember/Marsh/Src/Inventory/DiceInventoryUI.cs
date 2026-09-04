using UnityEngine;

public class DiceInventoryUI : MonoBehaviour {
    public static DiceInventoryUI Instance { get; private set; }

    [SerializeField] private DiceSlotUI diceSlotPrefab;
    [SerializeField] private Transform slotRoot;
    [SerializeField] private int slotCount = 12;

    private DiceSlotUI[] slots;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start() {
        slots = new DiceSlotUI[slotCount];

        for (int i = 0; i < slotCount; i++) {
            DiceSlotUI slot = Instantiate(diceSlotPrefab, slotRoot);
            slot.Initialize(i + 1);
            slots[i] = slot;
        }
    }

    public ActionData GetAction(int diceNumber) {
        if (slots == null) {
            Debug.LogError("DiceInventoryUIがまだ初期化されていません。");
            return null;
        }

        if (diceNumber < 1 || diceNumber > slots.Length) {
            Debug.LogError($"不正な出目: {diceNumber}");
            return null;
        }

        return slots[diceNumber - 1].GetAction();
    }
}