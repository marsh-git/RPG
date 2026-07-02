using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// システムオブジェクトを管理するクラス
/// </summary>
public class SystemManager : SystemObject
{
    /// <summary>
    /// このゲームで管理するシステムオブジェクト群
    /// </summary>
    [SerializeField]
    private SystemObject[] systemObjectList = null;
    void Start()
    {
        UniTask task = Initialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        //for文でsystemObjectを複製
        for(int i = 0, max = systemObjectList.Count(); i < max; i++) {
            SystemObject origin = systemObjectList[i];
            if (origin == null) continue;
            //複製したオブジェクトの初期化
            SystemObject createObject = Instantiate(origin, transform);
            await createObject.Initialize();
        }
        //スタンバイパートに遷移

        UniTask task = PartManager.instance.TransitionPart(GameEnum.eGamePart.Standby);
    }
    
}
