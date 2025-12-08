//------------------------------------------------------------
// File: UIBase.cs
// Purpose: Async-first base class for all UI panels.
//------------------------------------------------------------
using System;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.UI
{
    public enum UIState
    {
        None,
        Loading,
        Opening,
        Active,
        Closing,
        Deactive
    }

    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIBase : MonoBehaviour
    {
        protected CanvasGroup _canvasGroup;
        protected RectTransform _rectTransform;
        
        public UIState CurrentState { get; private set; } = UIState.None;
        
        // Is this UI currently visible and interactive?
        public bool IsActive => CurrentState == UIState.Active;

        #region Lifecycle API (Called by UIManager)

        public async UniTask InternalCreateAsync()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            
            // Default state
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);

            CurrentState = UIState.Loading;
            await OnCreateAsync();
            CurrentState = UIState.Deactive;
        }

        public async UniTask InternalOpenAsync()
        {
            if (CurrentState == UIState.Opening || CurrentState == UIState.Active) return;
            
            CurrentState = UIState.Opening;
            gameObject.SetActive(true);
            
            // Prepare data
            await OnOpenAsync();

            // Play Animation
            _canvasGroup.interactable = false; // Lock input during anim
            await PlayOpenAnimAsync();
            
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            CurrentState = UIState.Active;
        }

        public async UniTask InternalCloseAsync()
        {
            if (CurrentState == UIState.Closing || CurrentState == UIState.Deactive) return;

            CurrentState = UIState.Closing;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            // Play Animation
            await PlayCloseAnimAsync();
            
            // Cleanup
            await OnCloseAsync();
            
            gameObject.SetActive(false);
            CurrentState = UIState.Deactive;
        }

        public async UniTask InternalDestroyAsync()
        {
            await OnDestroyAsync();
            Destroy(gameObject);
        }

        #endregion

        #region Virtual Lifecycle Methods (For Subclasses)

        protected virtual UniTask OnCreateAsync() => UniTask.CompletedTask;
        protected virtual UniTask OnOpenAsync() => UniTask.CompletedTask;
        protected virtual UniTask OnCloseAsync() => UniTask.CompletedTask;
        protected virtual UniTask OnDestroyAsync() => UniTask.CompletedTask;

        #endregion

        #region Animation

        protected virtual async UniTask PlayOpenAnimAsync()
        {
            float duration = 0.3f;
            float timer = 0f;
            
            _rectTransform.localScale = Vector3.zero;
            _canvasGroup.alpha = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                
                // Simple EaseOutBack-ish effect
                float scale = EaseOutBack(t); 
                
                _rectTransform.localScale = Vector3.one * scale;
                _canvasGroup.alpha = t;
                await UniTask.Yield();
            }
            
            _rectTransform.localScale = Vector3.one;
            _canvasGroup.alpha = 1f;
        }

        protected virtual async UniTask PlayCloseAnimAsync()
        {
            float duration = 0.2f;
            float timer = 0f;
            
            _rectTransform.localScale = Vector3.one;
            _canvasGroup.alpha = 1f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                
                // Linear fade out
                float val = 1f - t;
                
                _rectTransform.localScale = Vector3.one * val;
                _canvasGroup.alpha = val;
                await UniTask.Yield();
            }
            
            _rectTransform.localScale = Vector3.zero;
            _canvasGroup.alpha = 0f;
        }
        
        private float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
        }

        #endregion

        #region Input
        /// <summary>
        /// Per-frame update driven by UIManager for the top-most active UI.
        /// </summary>
        public virtual void OnUpdate(float deltaTime) { }

        /// <summary>
        /// Called by UIManager when input is detected and this is the top UI (polled every frame).
        /// </summary>
        public virtual void HandleInput(IUiInputSource input)
        {
            if (input != null)
            {
                OnHandleInput(input);
            }

            
        }


        /// <summary>
        /// Override in subclasses to consume routed UI input.
        /// </summary>
        protected virtual void OnHandleInput(IUiInputSource input) { }

        #endregion
    }
}
