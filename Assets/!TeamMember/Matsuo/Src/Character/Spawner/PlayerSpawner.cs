using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // プレイヤープレハブ
    [SerializeField]
    private PlayerBase playerPrefab;

    // キャラクター管理クラス
    [SerializeField]
    private CharacterManager characterManager;

    /// <summary>
    /// 指定したタイルにプレイヤーを生成する
    /// </summary>
    /// <param name="spawnTile">生成先タイル</param>
    /// <returns>生成したプレイヤー</returns>
    public PlayerBase Spawn(HexTile spawnTile)
    {
        // タイルが存在しない場合は生成しない
        if (spawnTile == null)
        {
            return null;
        }

        // プレイヤーを生成する
        PlayerBase player = Instantiate(playerPrefab);

        // タイルへ配置する
        player.SetTile(spawnTile);

        // 管理対象へ登録する
        characterManager.Register(player);

        return player;
    }
}