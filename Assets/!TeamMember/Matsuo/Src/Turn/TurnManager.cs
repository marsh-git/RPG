using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    // 今のターン
    public TurnState CurrentTurn { get; private set; }

    // 参照用
    [SerializeField] private HexGridManager gridManager;
    [SerializeField] private EnemyBase enemy;

    // プレイヤー一覧
    [SerializeField]
    private List<PlayerBase> players = new List<PlayerBase>();

    // 現在行動中のプレイヤー番号
    private int currentPlayerIndex = 0;

    // キャラクター管理
    [SerializeField]
    private CharacterManager characterManager;

    // 現在ターン中のプレイヤー
    public PlayerBase CurrentPlayer => players[currentPlayerIndex];

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentPlayerIndex = 0;
        StartPlayerTurn();
    }

    /// <summary>
    /// プレイヤーターン
    /// </summary>
    public void StartPlayerTurn()
    {
        CurrentTurn = TurnState.PlayerTurn;

        Debug.Log($"プレイヤー{currentPlayerIndex + 1}のターン開始");

        // プレイヤー操作開始
        if (gridManager != null)
        {
            gridManager.enabled = true;
        }
    }

    /// <summary>
    /// プレイヤーターン終了
    /// </summary>
    public void EndPlayerTurn()
    {
        if (CurrentTurn != TurnState.PlayerTurn)
        {
            return;
        }

        if (gridManager != null)
        {
            gridManager.enabled = false;
        }

        // 次のプレイヤーへ
        currentPlayerIndex++;

        // 全プレイヤー行動後なら敵ターン
        if (currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;
            StartEnemyTurn();
        }
        else
        {
            StartPlayerTurn();
        }
    }

    /// <summary>
    /// 敵ターン開始
    /// </summary>
    private void StartEnemyTurn()
    {
        CurrentTurn = TurnState.EnemyTurn;

        Debug.Log("敵ターン開始");

        foreach (CharacterBase character in characterManager.GetCharacters())
        {
            if (character is EnemyBase enemy)
            {
                // enemy.StartTurn();
            }
        }

        EndEnemyTurn();
    }

    /// <summary>
    /// 敵ターン終了
    /// </summary>
    public void EndEnemyTurn()
    {
        Debug.Log("敵ターン終了");

        StartPlayerTurn();
    }

    /// <summary>
    /// プレイヤーをターン管理へ登録する
    /// </summary>
    /// <param name="player">登録するプレイヤー</param>
    public void RegisterPlayer(PlayerBase player)
    {
        if (player == null)
        {
            return;
        }

        if (players.Contains(player))
        {
            return;
        }

        players.Add(player);
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