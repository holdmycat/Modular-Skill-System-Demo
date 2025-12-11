//------------------------------------------------------------
// File: UIAnimUtil.cs
// Purpose: Shared UI animation helpers for screens and panels.
//------------------------------------------------------------
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ebonor.UI
{
    public enum UIAnimStyle
    {
        None,
        Scale,
        Fade,
        SlideFromLeft,
        SlideFromRight,
        SlideFromTop,
        SlideFromBottom
    }
    
    public static class UIAnimUtil
    {
        public static async UniTask PlayAsync(CanvasGroup canvasGroup, RectTransform rectTransform, UIAnimStyle style, bool show, float duration, float slideDistance = 600f, float scaleOvershoot = 1.05f)
        {
            switch (style)
            {
                case UIAnimStyle.Scale:
                    await PlayScaleAsync(canvasGroup, rectTransform, show, duration, scaleOvershoot);
                    break;
                case UIAnimStyle.Fade:
                    await PlayFadeAsync(canvasGroup, show, duration);
                    break;
                case UIAnimStyle.SlideFromLeft:
                    await PlaySlideAsync(canvasGroup, rectTransform, show, duration, new Vector2(-slideDistance, 0f));
                    break;
                case UIAnimStyle.SlideFromRight:
                    await PlaySlideAsync(canvasGroup, rectTransform, show, duration, new Vector2(slideDistance, 0f));
                    break;
                case UIAnimStyle.SlideFromTop:
                    await PlaySlideAsync(canvasGroup, rectTransform, show, duration, new Vector2(0f, slideDistance));
                    break;
                case UIAnimStyle.SlideFromBottom:
                    await PlaySlideAsync(canvasGroup, rectTransform, show, duration, new Vector2(0f, -slideDistance));
                    break;
                case UIAnimStyle.None:
                default:
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = show ? 1f : 0f;
                    }
                    if (rectTransform != null && !show)
                    {
                        rectTransform.localScale = Vector3.one;
                    }
                    await UniTask.CompletedTask;
                    break;
            }
        }

        private static async UniTask PlayScaleAsync(CanvasGroup canvasGroup, RectTransform rectTransform, bool show, float duration, float overshoot)
        {
            if (rectTransform == null)
            {
                await UniTask.CompletedTask;
                return;
            }

            float timer = 0f;
            Vector3 startScale = show ? Vector3.zero : rectTransform.localScale;
            Vector3 endScale = show ? Vector3.one : Vector3.zero;

            float startAlpha = canvasGroup != null ? (show ? 0f : canvasGroup.alpha) : 0f;
            float endAlpha = show ? 1f : 0f;

            rectTransform.localScale = startScale;
            if (canvasGroup != null) canvasGroup.alpha = startAlpha;

            while (timer < duration)
            {
                if (rectTransform == null) break;
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                float eased = EaseOutBack(t, overshoot);

                rectTransform.localScale = Vector3.LerpUnclamped(startScale, endScale, eased);
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                }
                await UniTask.Yield();
            }

            if (rectTransform == null) return;
            rectTransform.localScale = endScale;
            if (canvasGroup != null) canvasGroup.alpha = endAlpha;
        }

        private static async UniTask PlayFadeAsync(CanvasGroup canvasGroup, bool show, float duration)
        {
            if (canvasGroup == null)
            {
                await UniTask.CompletedTask;
                return;
            }

            float timer = 0f;
            float start = canvasGroup.alpha;
            float target = show ? 1f : 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                canvasGroup.alpha = Mathf.Lerp(start, target, t);
                await UniTask.Yield();
            }

            canvasGroup.alpha = target;
        }

        private static async UniTask PlaySlideAsync(CanvasGroup canvasGroup, RectTransform rectTransform, bool show, float duration, Vector2 offset)
        {
            if (rectTransform == null)
            {
                await UniTask.CompletedTask;
                return;
            }

            Vector2 targetPos = rectTransform.anchoredPosition;
            Vector2 startPos = show ? targetPos + offset : targetPos;
            Vector2 endPos = show ? targetPos : targetPos + offset;

            float timer = 0f;
            float startAlpha = canvasGroup != null ? (show ? 0f : canvasGroup.alpha) : 0f;
            float endAlpha = show ? 1f : 0f;

            rectTransform.anchoredPosition = startPos;
            if (canvasGroup != null) canvasGroup.alpha = startAlpha;

            while (timer < duration)
            {
                if (rectTransform == null) break;
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                float eased = EaseOutQuad(t);

                rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, eased);
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                }
                await UniTask.Yield();
            }

            if (rectTransform == null) return;
            rectTransform.anchoredPosition = endPos;
            if (canvasGroup != null) canvasGroup.alpha = endAlpha;
        }

        private static float EaseOutBack(float x, float overshoot)
        {
            float c1 = overshoot;
            float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }

        private static float EaseOutQuad(float x)
        {
            return 1f - (1f - x) * (1f - x);
        }
    }
}
