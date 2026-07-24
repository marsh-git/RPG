using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    // 今のターン
    public TurnState CurrentTurn { get; private set; }

    // 参照用
    [SerializeField] private HexTileManager tileManager;
    [SerializeField] private EnemyAIManager enemyAIManager;

    // プレイヤー一覧
    [SerializeField]
    private List<PlayerBase> players = new List<PlayerBase>();

    // 現在行動中のプレイヤー番号
    private int currentPlayerIndex = 0;

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
        if (tileManager != null)
        {
            tileManager.enabled = true;
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

        if (tileManager != null)
        {
            tileManager.enabled = false;
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
    private async void StartEnemyTurn()
    {
        CurrentTurn = TurnState.EnemyTurn;

        Debug.Log("敵ターン開始");

        if (enemyAIManager != null)
        {
            await enemyAIManager.StartEnemyTurn();
        }
    }

    /// <summary>
    /// 敵ターン終了
    /// </summary>
    public void EndEnemyTurn()
    {
        Debug.Log("敵ターン終了");

        // ターン経過によるタイル処理
        foreach (HexTileData tile in HexTileManager.instance.GetAllTiles())
        {
            // 属性が存在するタイルのみ処理
            if (tile.attributeTile != null)
            {
                tile.attributeTile.OnTickTile(tile);
            }
        }

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