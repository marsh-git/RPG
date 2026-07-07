using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexDirExpansion {
    /// <summary>
    /// 現在の方向から見て左隣の方向取得
    /// </summary>
    public static eDirectionHex GetLeftDir(this eDirectionHex currentDir) {
        if(currentDir == eDirectionHex.Invalid || currentDir == eDirectionHex.Max) return eDirectionHex.Invalid;
        // 時計回り定義なので、左に曲がる=インデックスを -1 する
        // 負の数を考慮して+6してから%6
        int nextIndex = ((int)currentDir - 1 + 6) % 6;
        return (eDirectionHex)nextIndex;
    }
    /// <summary>
    /// 現在の方向から見て右隣の方向取得
    /// </summary>
    public static eDirectionHex GetRightDir(this eDirectionHex currentDir) {
        if(currentDir == eDirectionHex.Invalid || currentDir == eDirectionHex.Max) return eDirectionHex.Invalid;
        // 右に曲がる=インデックスを+1する
        int nextIndex = ((int)currentDir + 1) % 6;
        return (eDirectionHex)nextIndex;
    }
    /// <summary>
    /// 現在の方向の真後ろの方向取得
    /// </summary>
    public static eDirectionHex GetOppositeDir(this eDirectionHex currentDir) {
        if(currentDir == eDirectionHex.Invalid || currentDir == eDirectionHex.Max) return eDirectionHex.Invalid;
        // 真後ろ=インデックスを+3する
        int nextIndex = ((int)currentDir + 3) % 6;
        return (eDirectionHex)nextIndex;
    }
}