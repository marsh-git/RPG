using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator{

    public static int DecideSeedByLevel() {
        // 難易度選択のみでゲームが開始されるときは、難易度に応じたシード値を決定する。
        return -1;
    }
    public static int DecideSeedByCustom() {
        // カスタムルールでゲームが開始されるときは、カスタム内容に応じたシード値を決定する。
        return -1;
    }
    public static void CreateMap() {
        // シード値に応じたマップ生成を行う。
    }
    /// <summary>
    /// 街マスの取得
    /// </summary>
    private static void CreateTown() {
        // 中心からタイルを決定する

        // 中心タイルから周囲6マスも街マスとする

        // ※そのため、街マスの中心は端マスより1マス内側でなければいけない
    }
}
