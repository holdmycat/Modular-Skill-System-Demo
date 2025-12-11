//------------------------------------------------------------
// File: UIBase.cs
// Purpose: Async-first base class for all UI panels.
//------------------------------------------------------------
using System;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
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
        [Header("Blackboard")]
        [SerializeField] protected UiBlackboardBase blackboard;
        
        [SerializeField] protected CanvasGroup _bufferCanvasGroup;
        
        protected CanvasGroup _canvasGroup;
        protected RectTransform _rectTransform;
        
        public UIState CurrentState { get; private set; } = UIState.None;
        
        // Is this UI currently visible and interactive?
        public bool IsActive => CurrentState == UIState.Active;

        [Header("Animation")]
        [SerializeField] private UIAnimStyle openAnim = UIAnimStyle.Scale;
        [SerializeField] private UIAnimStyle closeAnim = UIAnimStyle.Scale;
        [SerializeField] private float openDuration = 0.3f;
        [SerializeField] private float closeDuration = 0.2f;
        [SerializeField] private float slideDistance = 600f;
        [SerializeField] private float scaleOvershoot = 1.05f;

        #region Lifecycle API (Called by UIManager)

        public async UniTask InternalCreateAsync()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            
            UIHelper.OnSetCanvasState(_bufferCanvasGroup, false);
            
            // Default state
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);

            CurrentState = UIState.Loading;
            await OnCreateAsync();
            CurrentState = UIState.Deactive;
        }

        /// <summary>Typed access to attached blackboard.</summary>
        public T GetBlackboard<T>() where T : UiBlackboardBase => blackboard as T;

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
            
            UIHelper.OnSetCanvasState(_bufferCanvasGroup, false);
            
            CurrentState = UIState.Closing;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            // Play Animation
            if (_rectTransform != null)
            {
                try
                {
                    await PlayCloseAnimAsync();
                }
                catch (MissingReferenceException)
                {
                    // UI was destroyed during shutdown; skip animation.
                }
            }
            
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

        protected abstract UniTask OnCreateAsync();
        protected abstract UniTask OnOpenAsync();   
        protected abstract UniTask OnCloseAsync();
        protected abstract UniTask OnDestroyAsync();

        #endregion

        #region Animation

        protected virtual async UniTask PlayOpenAnimAsync()
        {
            await UIAnimUtil.PlayAsync(_canvasGroup, _rectTransform, openAnim, true, openDuration, slideDistance, scaleOvershoot);
        }

        protected virtual async UniTask PlayCloseAnimAsync()
        {
            await UIAnimUtil.PlayAsync(_canvasGroup, _rectTransform, closeAnim, false, closeDuration, slideDistance, scaleOvershoot);
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
