using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MainGamePart : BasePart
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
