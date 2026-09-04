using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム起動時に移行するパート
/// </summary>
public class StandbyPart : BasePart
{
    public override async UniTask Init()
    {
        await base.Init();
        await base.Setup();
    }

    public override async UniTask Execute()
    {
        UniTask task = PartManager.instance.TransitionPart(GameEnum.eGamePart.Title);
        await FadeManeger.instance.FadeIn(1.0f);
    }
}
