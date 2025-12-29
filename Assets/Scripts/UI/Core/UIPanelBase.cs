//------------------------------------------------------------
// File: UIPanelBase.cs
// Purpose: Base class for embedded UI panels with shared animation.
//------------------------------------------------------------
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIPanelBase : MonoBehaviour
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(UIPanelBase));
        
        [Header("Animation")]
        [SerializeField] private UIAnimStyle showAnim = UIAnimStyle.Scale;
        [SerializeField] private UIAnimStyle hideAnim = UIAnimStyle.Scale;
        [SerializeField] private float showDuration = 0.25f;
        [SerializeField] private float hideDuration = 0.2f;
        [SerializeField] private float slideDistance = 600f;
        [SerializeField] private float scaleOvershoot = 1.05f;

        protected CanvasGroup canvasGroup;
        protected RectTransform rectTransform;

        protected bool IsVisible { get; private set; }

        protected virtual void Awake()
        {
            
            log.Info($"[{GetType().Name}] OnCreateAsync.");
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
           
            InstantHide();
        }

        protected async UniTask ShowAsync()
        {
            
            log.Info($"[{GetType().Name}] ShowAsync.");
            
            if (IsVisible) return;

            gameObject.SetActive(true);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            await OnShowAsync();
            
            // Check if destroyed during OnShowAsync
            if (this == null || canvasGroup == null) return;
            
            await UIAnimUtil.PlayAsync(canvasGroup, rectTransform, showAnim, true, showDuration, slideDistance, scaleOvershoot)
                .AttachExternalCancellation(this.GetCancellationTokenOnDestroy());

            // Check if destroyed during PlayAsync
            if (this == null || canvasGroup == null) return;

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            IsVisible = true;
        }

        public async UniTask HideAsync()
        {
            
            log.Info($"[{GetType().Name}] HideAsync.");
            
            if (!IsVisible) return;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            await UIAnimUtil.PlayAsync(canvasGroup, rectTransform, hideAnim, false, hideDuration, slideDistance, scaleOvershoot)
                .AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
            
            if (this == null) return;

            await OnHideAsync();

            if (this == null) return;

            gameObject.SetActive(false);
            IsVisible = false;
        }

        private void InstantHide()
        {
            
            log.Info($"[{GetType().Name}] InstantHide.");
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
