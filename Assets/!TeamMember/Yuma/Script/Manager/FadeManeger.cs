using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// フェード処理管理クラス
/// </summary>
public class FadeManeger : SystemObject {
    //フェード用黒画像
    [SerializeField] private Image fadeImage = null;
    
    public static FadeManeger instance { get; private set; } = null;
    //デフォルトのフェード時間
    [SerializeField] private const float DEFAULT_FADE_DURATION = 0.3f;
    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() { 
        instance = this;
        await UniTask.CompletedTask;
    }
    /// <summary>
    /// フェードアウト、暗くする
    /// </summary>
    /// <param name="_duration"></param>
    /// <returns></returns>
    public async UniTask FadeOut(float _duration = DEFAULT_FADE_DURATION) {
        await FadeTargetAlpha(1.0f, _duration);
    }
    /// <summary>
    /// フェードイン、明るくする
    /// </summary>
    /// <param name="_duration"></param>
    /// <returns></returns>
    public async UniTask FadeIn(float _duration = DEFAULT_FADE_DURATION) {
        await FadeTargetAlpha(0.0f, _duration);
    }
    /// <summary>
    /// フェード画像を指定の不透明度に変化させる
    /// </summary>
    /// <param name="_duration"></param>
    /// <returns></returns>
    private async UniTask FadeTargetAlpha(float _targetAlpha,float _duration) {
        float elapsedTime = 0.0f;//経過時間
        float startAlpha = fadeImage.color.a;
        Color targetColor = fadeImage.color;
        while (elapsedTime < _duration) {
            //フレーム時間経過
            elapsedTime += Time.deltaTime;
            //補完した不透明度をフェード画像に設定
            float t = elapsedTime / _duration;

            targetColor.a = Mathf.Lerp(startAlpha, _targetAlpha, t);
            fadeImage.color = targetColor;
            //1フレーム待つ
            await UniTask.DelayFrame(1);

        }
        targetColor.a = _targetAlpha;
        fadeImage.color = targetColor;
    }
}
