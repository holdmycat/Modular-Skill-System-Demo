//------------------------------------------------------------
// File: UIPanelBase.cs
// Purpose: Base class for embedded UI panels with shared animation.
//------------------------------------------------------------
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ebonor.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIPanelBase : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private UIAnimStyle showAnim = UIAnimStyle.Scale;
        [SerializeField] private UIAnimStyle hideAnim = UIAnimStyle.Scale;
        [SerializeField] private float showDuration = 0.25f;
        [SerializeField] private float hideDuration = 0.2f;
        [SerializeField] private float slideDistance = 600f;
        [SerializeField] private float scaleOvershoot = 1.05f;

        protected CanvasGroup canvasGroup;
        protected RectTransform rectTransform;

        public bool IsVisible { get; private set; }

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
           
            InstantHide();
        }

        protected async UniTask ShowAsync()
        {
            if (IsVisible) return;

            gameObject.SetActive(true);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            await OnShowAsync();
            await UIAnimUtil.PlayAsync(canvasGroup, rectTransform, showAnim, true, showDuration, slideDistance, scaleOvershoot);

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            IsVisible = true;
        }

        public async UniTask HideAsync()
        {
            if (!IsVisible) return;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            await UIAnimUtil.PlayAsync(canvasGroup, rectTransform, hideAnim, false, hideDuration, slideDistance, scaleOvershoot);
            await OnHideAsync();

            gameObject.SetActive(false);
            IsVisible = false;
        }

        private void InstantHide()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            if (rectTransform != null) rectTransform.localScale = Vector3.one;
            gameObject.SetActive(false);
            IsVisible = false;
        }

        protected virtual UniTask OnShowAsync() => UniTask.CompletedTask;
        protected virtual UniTask OnHideAsync() => UniTask.CompletedTask;
    }
}
