using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // マップ上に存在する全キャラクター
    private readonly List<CharacterBase> characters = new();

    /// <summary>
    /// キャラクターを管理対象に追加する
    /// </summary>
    /// <param name="character">追加するキャラクター</param>
    public void Register(CharacterBase character)
    {
        if (character == null)
        {
            return;
        }

        if (characters.Contains(character))
        {
            return;
        }

        characters.Add(character);
    }

    /// <summary>
    /// キャラクターを管理対象から削除する
    /// </summary>
    /// <param name="character">削除するキャラクター</param>
    public void Unregister(CharacterBase character)
    {
        if (character == null)
        {
            return;
        }

        characters.Remove(character);
    }

    /// <summary>
    /// 管理している全キャラクターを取得する
    /// </summary>
    /// <returns>キャラクター一覧</returns>
    public IReadOnlyList<CharacterBase> GetCharacters()
    {
        return characters;
    }

    /// <summary>
    /// 指定したタイル上にいるキャラクターを取得する
    /// </summary>
    /// <param name="tile">検索対象のタイル</param>
    /// <returns>存在しない場合はnull</returns>
    public CharacterBase GetCharacter(HexTile tile)
    {
        foreach (CharacterBase character in characters)
        {
            if (character.CurrentTile == tile)
            {
                return character;
            }
        }

        return null;
    }

    /// <summary>
    /// 全キャラクターを管理対象から削除する
    /// </summary>
    public void Clear()
    {
        characters.Clear();
    }
}