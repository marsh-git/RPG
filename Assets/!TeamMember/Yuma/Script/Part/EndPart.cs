using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPart : BasePart
{
    public override async UniTask Init()
    {
        await base.Init();
    }

    public override async UniTask Execute()
    {
        await UniTask.CompletedTask;
    }
}
