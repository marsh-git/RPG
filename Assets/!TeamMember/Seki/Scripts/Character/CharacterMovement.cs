using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CharacterMovement{
    /// <summary>
    /// あらゆるキャラクターオブジェクトを経路に沿って非同期移動させる
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="onStepTile"></param>
    /// <returns></returns>
    public async UniTask MoveAlongPathAsync(
            Transform targetTransform,
            List<HexTileData> path,
            CancellationToken cancellationToken,
            Action<HexTileData> onStepTile = null) {

        if(targetTransform == null || path == null || path.Count == 0) return;
        List<HexTileData> movePath = new List<HexTileData>(path);
        try {
            foreach(HexTileData nextTile in path) {
                if(nextTile == null) continue;

                Vector3 startPos = targetTransform.position;

                // 座標取得関数を使用し、高さをオフセット
                Vector3 endPos = nextTile.GetTilePos() + new Vector3(0f, 0.5f, 0f);

                // 進行方向への旋回ベクトルの計算
                Vector3 moveDir = (endPos - startPos).normalized;
                moveDir.y = 0; // 上下方向の傾きは無視して水平回転させる

                Quaternion startRot = targetTransform.rotation;
                Quaternion endRot = moveDir != Vector3.zero ? Quaternion.LookRotation(moveDir) : startRot;

                float elapsedTime = 0f;
                float duration = 0.2f; // 1マス移動にかかる時間

                while(elapsedTime < duration) {
                    cancellationToken.ThrowIfCancellationRequested();

                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / duration;

                    // 位置の線形補間
                    targetTransform.position = Vector3.Lerp(startPos, endPos, t);

                    // 向きの球面線形補間（滑らかに方向転換、少し早めに回転が終わるよう調整）
                    targetTransform.rotation = Quaternion.Slerp(startRot, endRot, t * 2f);

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }

                // マスにぴったり座標と向きを合わせる
                targetTransform.position = endPos;
                targetTransform.rotation = endRot;

                // 1マス移動が完了したら、座標データを即座に同期させる
                onStepTile?.Invoke(nextTile);
            }
        } catch(OperationCanceledException) {
            Debug.Log($"【移動中断】{targetTransform.name} の移動タスクが安全にキャンセルされました。");
        }
    }
}