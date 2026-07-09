using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // プレイヤープレハブ
    [SerializeField]
    private PlayerBase playerPrefab = null;

    // キャラクター管理クラス
    [SerializeField]
    private CharacterManager characterManager = null;

    /// <summary>
    /// 候補タイルからランダムな位置へプレイヤーを生成する
    /// </summary>
    /// <param name="candidateTiles">生成候補タイル</param>
    /// <returns>生成したプレイヤー</returns>
    public PlayerBase Spawn(List<HexTileObject> candidateTiles)
    {
        // 候補タイルまたはプレハブが存在しない場合は処理しない
        if (candidateTiles == null || candidateTiles.Count == 0 || playerPrefab == null)
        {
            Debug.LogError("プレイヤーの生成に失敗しました（候補タイルがない、またはPrefabが未設定です）");
            return null;
        }

        // リストの要素数からランダムタイルを取得する
        int randomIndex = Random.Range(0, candidateTiles.Count);
        HexTileObject targetTile = candidateTiles[randomIndex];

        // プレイヤーを生成する
        PlayerBase player = Instantiate(
            playerPrefab,
            targetTile.transform.position,
            Quaternion.identity);

        // デバッグ用の名前を設定する
        player.name = "Player_Debug";

        // プレイヤーの現在タイルを設定する
        player.SetTile(targetTile.ID);

        // CharacterManagerへ登録する
        characterManager.Register(player);

        Debug.Log($"【プレイヤー生成】{targetTile.name} に配置しました。");

        return player;
    }
}