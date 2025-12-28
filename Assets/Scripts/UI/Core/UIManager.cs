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
using Zenject;

namespace Ebonor.UI
{
    public class UIManager : MonoBehaviour, IUIService, ITickable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UIManager));
        
        // Cache loaded UIs: Type -> Instance
        private Dictionary<Type, UIBase> _uiDict;
        
        // Stack to track active UIs (for input handling order)
        private List<UIBase> _activeStack;
        
        private IInputService _inputService;

        private ResourceLoader _resourceLoader;
        private IInstantiator _container;

        [Inject]
        public void Construct(IInputService inputService, ResourceLoader loader, IInstantiator container)
        {
            _inputService = inputService;
            _resourceLoader = loader;
            _container = container;
            _uiDict = new Dictionary<Type, UIBase>();
            _activeStack = new List<UIBase>();
        }
        
        public void Tick()
        {
            // Zenject Tick
            if (_inputService != null)
            {
                HandleInput(Time.deltaTime, _inputService as PlayerInputRouter);
            }
        }

        private void OnDestroy()
        {
            // Clean up resources here if owner destroys the manager
            Exit(); 
        }

        private void Exit()
        {
            //Cleanup all UIs
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
        /// <summary>
        /// Open a UI panel. Loads it if not already cached.
        /// </summary>
        /// <param name="beforeOpen">Action to configure the UI before it opens</param>
        /// <param name="canvasScope">Optional: The DI Container to use for instantiation (e.g., Scene Context)</param>
        public async UniTask<T> OpenUIAsync<T>(Action<T> beforeOpen = null, IInstantiator canvasScope = null) where T : UIBase
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
                var prefab = await _resourceLoader.LoadAsset<GameObject>(prefabPath, ResourceAssetType.UiPrefab);
                
                if (prefab == null)
                {
                    log.Error($"Failed to load UI prefab at path: {prefabPath}");
                    return null;
                }
            
                // Use the provided scope (Scene) or fallback to global (Project)
                IInstantiator containerToUse = canvasScope != null ? canvasScope : _container;

                // Use Zenject to Instantiate, ensuring [Inject] works inside the UI Prefab
                ui = containerToUse.InstantiatePrefabForComponent<T>(prefab, gameObject.transform);
                if (ui == null)
                {
                    log.Error($"Prefab at {prefabPath} does not have component {type.Name}");
                    // InstantiatePrefabForComponent might return null if component missing? 
                    // Actually it throws exception usually if missing, or returns null. 
                    // If null, we can't destroy the GO easily because we don't have reference if it failed.
                    // But safe to assume if T is missing it might be an issue.
                    return null;
                }
                
                ui.gameObject.name = prefab.name;
            
                await ui.InternalCreateAsync();
                _uiDict.Add(type, ui);
            }
            
            // 3. Open
            // Bring to front
            if (ui != null)
            {
                // Allow caller to inject context before lifecycle callbacks run
                beforeOpen?.Invoke(ui);
            
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
        public async UniTask CloseUIAsync<T>(bool destroy = false) where T : UIBase
        {
            if (_uiDict.TryGetValue(typeof(T), out var ui))
            {
                await CloseUIAsync(ui, destroy);
            }
        }

        public async UniTask CloseUIAsync(UIBase ui, bool destroy = false)
        {
            if (ui == null || ui.Equals(null)) return;

            try
            {
                await ui.InternalCloseAsync();
            }
            catch (MissingReferenceException)
            {
                // Object was destroyed externally (e.g., during quit); skip further work.
                return;
            }
            
            if (_activeStack.Contains(ui))
            {
                _activeStack.Remove(ui);
            }

            if (destroy)
            {
                _uiDict.Remove(ui.GetType());
                if (ui != null && !ui.Equals(null))
                {
                    await ui.InternalDestroyAsync();
                }
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
                // if (_globalUiHandler != null)
                // {
                //     if (cancel) _globalUiHandler.Invoke(UiGlobalCommand.Cancel);
                //     if (openMenu) _globalUiHandler.Invoke(UiGlobalCommand.OpenMenu);
                // }
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
