using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    // 今のターン
    public TurnState CurrentTurn { get; private set; }

    // 参照用
    [SerializeField] private HexGridManager gridManager;
    [SerializeField] private EnemyBase enemy;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartPlayerTurn();
    }

    // プレイヤーのターン
    public void StartPlayerTurn()
    {
        CurrentTurn = TurnState.PlayerTurn;

        // プレイヤー操作開始
        if (gridManager != null)
            gridManager.enabled = true;
    }

    public void EndPlayerTurn()
    {
        if (CurrentTurn != TurnState.PlayerTurn)
            return;

        if (gridManager != null)
            gridManager.enabled = false;

        StartEnemyTurn();
    }

    // 敵のターン
    private void StartEnemyTurn()
    {
        CurrentTurn = TurnState.EnemyTurn;

        if (enemy != null)
        {
            //enemy.StartTurn();
        }
    }

    public void EndEnemyTurn()
    {
        StartPlayerTurn();
    }

    public bool IsPlayerTurn()
    {
        return CurrentTurn == TurnState.PlayerTurn;
    }

    public bool IsEnemyTurn()
    {
        return CurrentTurn == TurnState.EnemyTurn;
    }
}