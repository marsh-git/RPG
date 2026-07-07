using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexDirExpansion {
    /// <summary>
    /// 指定方向を2次元上の座標に変換
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Vector2Int GetDirPos(this eDirectionHex dir) {
        switch(dir) {
            case eDirectionHex.UpRight:     // 右上
            return new Vector2Int(0, 1);
            case eDirectionHex.Right:       // 右
            return new Vector2Int(1, 0);
            case eDirectionHex.DownRight:   // 右下
            return new Vector2Int(1, -1);
            case eDirectionHex.DownLeft:    // 左下
            return new Vector2Int(0, -1);
            case eDirectionHex.Left:        // 左
            return new Vector2Int(-1, 0);
            case eDirectionHex.UpLeft:      // 左上
            return new Vector2Int(-1, 1);
        }
        return Vector2Int.zero;
    }
    /// <summary>
    /// 現在の方向から見て左隣の方向取得
    /// </summary>
    public static eDirectionHex GetLeftDir(this eDirectionHex dir) {
        if(dir == eDirectionHex.Invalid || dir == eDirectionHex.Max) return eDirectionHex.Invalid;
        // 時計回り定義なので、左に曲がる=インデックスを -1 する
        // 負の数を考慮して+6してから%6
        int nextIndex = ((int)dir - 1 + 6) % 6;
        return (eDirectionHex)nextIndex;
    }
    /// <summary>
    /// 現在の方向から見て右隣の方向取得
    /// </summary>
    public static eDirectionHex GetRightDir(this eDirectionHex dir) {
        if(dir == eDirectionHex.Invalid || dir == eDirectionHex.Max) return eDirectionHex.Invalid;
        // 右に曲がる=インデックスを+1する
        int nextIndex = ((int)dir + 1) % 6;
        return (eDirectionHex)nextIndex;
    }
    /// <summary>
    /// 現在の方向の真後ろの方向取得
    /// </summary>
    public static eDirectionHex GetOppositeDir(this eDirectionHex dir) {
        if(dir == eDirectionHex.Invalid || dir == eDirectionHex.Max) return eDirectionHex.Invalid;
        // 真後ろ=インデックスを+3する
        int nextIndex = ((int)dir + 3) % 6;
        return (eDirectionHex)nextIndex;
    }
}