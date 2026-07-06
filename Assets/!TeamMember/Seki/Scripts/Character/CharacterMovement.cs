using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CharacterMovement{
    /// <summary>
    /// あらゆるキャラクターオブジェクトを経路に沿って非同期移動させる（純粋C#ロジック）
    /// </summary>
    /// <param name="targetTransform">動かしたい3DオブジェクトのTransform</param>
    /// <param name="path">移動経路マスのデータリスト</param>
    /// <param name="cancellationToken">連打・破棄連動用のトークン</param>
    /// <param name="onStepTile">1マス進むごとに座標データを同期するためのコールバック</param>
    public async UniTask MoveAlongPathAsync(
        Transform targetTransform,
        List<HexTile> path,
        CancellationToken cancellationToken,
        Action<HexTile> onStepTile = null) {
        if(targetTransform == null || path == null || path.Count == 0) return;

        try {
            foreach(HexTile nextTile in path) {
                Vector3 startPos = targetTransform.position;
                // マスの上に少し浮かせる位置を計算
                Vector3 endPos = nextTile.transform.position + new Vector3(0, 0.5f, 0);

                float elapsedTime = 0f;
                float duration = 0.2f; // 1マス移動にかかる時間

                while(elapsedTime < duration) {
                    cancellationToken.ThrowIfCancellationRequested();

                    elapsedTime += Time.deltaTime;
                    targetTransform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }

                // マスにぴったり座標を合わせる
                targetTransform.position = endPos;

                // 1マス移動が完了したら、外部（Character側）の座標データを即座に同期させる
                onStepTile?.Invoke(nextTile);
            }
        } catch(OperationCanceledException) {
            Debug.Log($"【移動中断】{targetTransform.name} の移動タスクが安全にキャンセルされました。");
        }
    }
}