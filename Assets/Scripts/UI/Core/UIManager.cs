//------------------------------------------------------------
// File: UIManager.cs
// Purpose: Global manager for UI stack and input routing.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.UI
{
    public class UIManager : MonoBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UIManager));
        
        public enum UiGlobalCommand
        {
            Cancel,
            OpenMenu
        }

        private Transform _uiRoot;

        // Cache loaded UIs: Type -> Instance
        private Dictionary<Type, UIBase> _uiDict = new Dictionary<Type, UIBase>();
        
        // Stack to track active UIs (for input handling order)
        private List<UIBase> _activeStack = new List<UIBase>();

        private Action<UiGlobalCommand> _globalUiHandler;

        public void Init(Transform root)
        {
            _uiRoot = root;
        }

        public void SetGlobalUiHandler(Action<UiGlobalCommand> handler)
        {
            _globalUiHandler = handler;
        }

        public void OnUpdate(float deltaTime, PlayerInputRouter router)
        {
            HandleInput(deltaTime, router);
        }

        public void Exit()
        {
            // Cleanup all UIs
            foreach (var ui in _uiDict.Values)
            {
                if (ui != null) Destroy(ui.gameObject);
            }
            _uiDict.Clear();
            _activeStack.Clear();
        }

        #region Public API

        /// <summary>
        /// Open a UI panel. Loads it if not already cached.
        /// </summary>
        public async UniTask<T> OpenUIAsync<T>() where T : UIBase
        {
            Type type = typeof(T);
            T ui = null;

            var prefabPath = type.Name;
            
            // 1. Check Cache
            if (_uiDict.TryGetValue(type, out var baseUI))
            {
                ui = baseUI as T;
            }
            else
            {
                var prefab = await GlobalServices.ResourceLoader.LoadAsset<GameObject>(prefabPath, ResourceAssetType.UiPrefab);
                
                if (prefab == null)
                {
                    log.Error($"Failed to load UI prefab at path: {prefabPath}");
                    return null;
                }

                var go = Instantiate(prefab, _uiRoot);
                ui = go.GetComponent<T>();
                if (ui == null)
                {
                    log.Error($"Prefab at {prefabPath} does not have component {type.Name}");
                    Destroy(go);
                    return null;
                }

                await ui.InternalCreateAsync();
                _uiDict.Add(type, ui);
            }

            // 3. Open
            // Bring to front
            if (ui != null)
            {
                ui.transform.SetAsLastSibling();

                if (!_activeStack.Contains(ui))
                {
                    _activeStack.Add(ui);
                }

                await ui.InternalOpenAsync();
                return ui;
            }

            
            log.Error("Fatal error, fail to load:" + prefabPath);
            
            return null;
        }

        /// <summary>
        /// Close a UI panel.
        /// </summary>
        public async UniTask CloseUIAsync<T>() where T : UIBase
        {
            if (_uiDict.TryGetValue(typeof(T), out var ui))
            {
                await CloseUIAsync(ui);
            }
        }

        public async UniTask CloseUIAsync(UIBase ui)
        {
            if (ui == null) return;

            await ui.InternalCloseAsync();
            
            if (_activeStack.Contains(ui))
            {
                _activeStack.Remove(ui);
            }
        }

        public T GetUI<T>() where T : UIBase
        {
            if (_uiDict.TryGetValue(typeof(T), out var ui))
            {
                return ui as T;
            }
            return null;
        }

        #endregion

        #region Input Handling

        private void HandleInput(float deltaTime, PlayerInputRouter router)
        {
            // 1. Check Global Permission via PlayerInputRouter
            if (router != null && !router.UiEnabled)
            {
                return;
            }

            bool openMenu = router != null && router.OpenMenuTriggered;
            bool cancel = router != null && router.CancelTriggered;

            if (openMenu || cancel)
            {
                // Forward global commands to outer controller (e.g., GameClientManager)
                if (_globalUiHandler != null)
                {
                    if (cancel) _globalUiHandler.Invoke(UiGlobalCommand.Cancel);
                    if (openMenu) _globalUiHandler.Invoke(UiGlobalCommand.OpenMenu);
                }
                return;
            }

            // 2. Get Top Active UI
            if (null == _activeStack || _activeStack.Count == 0) return;
            
            // The last element is the "top" one
            var topUI = _activeStack[^1];

            // 3. Dispatch Input
            // Only dispatch if it's fully active (not animating)
            if (topUI.IsActive)
            {
                topUI.HandleInput(router);
                topUI.OnUpdate(deltaTime);
            }
        }

        #endregion
    }
}
