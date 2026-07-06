using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// システムオブジェクトの基底クラス
/// </summary>
public abstract class SystemObject : MonoBehaviour{

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public abstract UniTask Initialize();
    
}
