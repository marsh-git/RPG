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
        UniTask task = ServerExecute();
        task = ClientExecute();

        await UniTask.CompletedTask;
    }

    public override async UniTask ServerExecute()
    {
        await UniTask.CompletedTask;
    }

    public override async UniTask ClientExecute()
    {
        await UniTask.CompletedTask;
    }
}
