using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パート管理クラス
/// </summary>
public class PartManager : SystemObject
{
    //自身のインスタンス
    public static PartManager instance = null;
    //現在のパート
    public BasePart currentPart { get; private set; } = null;
    //パートのオリジナル
    [SerializeField] private BasePart[] partOrigin = null;
    //パートの配列
    private BasePart[] partList = null;

    /// <summary>
    /// 初期化関数
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        //インスタンスの自身を設定
        instance = this;
        
        //配列をパートの数分容量確保
        int partMax = (int)GameEnum.eGamePart.PartMax;
        partList = new BasePart[partMax];
        //非同期処理のリストに各パート初期化関数を積む
        List<UniTask> tasks = new List<UniTask>();
        for(int i = 0; i < partMax; i++) {
            partList[i] = Instantiate(partOrigin[i], transform);
            tasks.Add(partList[i].Init());
        }
        //一気に処理
        await CommonModule.WaitTask(tasks);
    }

    /// <summary>
    /// パート変更処理
    /// </summary>
    /// <param name="_nextPart"></param>
    /// <returns></returns>
    public async UniTask TransitionPart(GameEnum.eGamePart _nextPart) {
        if (!NetworkServer.active)
        {
            await ChangePartClient(_nextPart);
        }
        else
        {
            await ChangePartServer(_nextPart);
        }

    }

    /// <summary>
    /// サーバー専用パート遷移処理
    /// </summary>
    /// <param name="_nextPart"></param>
    /// <returns></returns>
    [Server]
    private async UniTask ChangePartServer(GameEnum.eGamePart _nextPart)
    {
        //現在のパートの片付け
        if (currentPart != null)
        {
            await currentPart.Teardown();
        }
        //次のパートに移動
        currentPart = partList[(int)_nextPart];
        await currentPart.Setup();

        //実行処理
        currentPart.ServerExecute().Forget();
        currentPart.Execute().Forget();

        PartNetworkGame.instance.SetPart(_nextPart);
    }

    /// <summary>
    /// クライアント専用パート遷移
    /// </summary>
    /// <param name="_nextPart"></param>
    /// <returns></returns>
    public async UniTask ChangePartClient(GameEnum.eGamePart _nextPart)
    {
        //現在のパートの片付け
        if (currentPart != null)
        {
            await currentPart.Teardown();
        }
        //次のパートに移動
        currentPart = partList[(int)_nextPart];
        await currentPart.Setup();

        //実行処理
        currentPart.ClientExecute().Forget();
    }

    public async UniTask ChangePartLocal(GameEnum.eGamePart _nextPart)
    {
        //現在のパートの片付け
        if (currentPart != null)
        {
            await currentPart.Teardown();
        }
        //次のパートに移動
        currentPart = partList[(int)_nextPart];
        await currentPart.Setup();

        currentPart.Execute().Forget();
    }
}
