//------------------------------------------------------------
// File: ShowCaseSceneManager.cs
// Created: 2025-12-06
// Purpose: Data-driven scene manager for the ShowcaseScene sandbox.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.UI;
using UnityEngine;
using Ebonor.GamePlay;
using ResourceLoader = Ebonor.DataCtrl.ResourceLoader;

namespace Ebonor.Manager
{

    //uiscene_showcase
    public partial class ShowCaseSceneManager : SceneManagerBase
    {
        private UIScene_ShowCase _uiShowCase;
        
        private async UniTask OpenShowcaseUI(ActorInstanceBase playerInstance)
        {
            var uiManager = ClientManager != null ? ClientManager.GetUiManager() : null;
            if (uiManager == null)
            {
                log.Error("UIManager is null; cannot open showcase UI.");
                return;
            }

            var numeric = playerInstance != null ? playerInstance.ActorNumericComponentBase : null;

            _uiShowCase = await uiManager.OpenUIAsync<UIScene_ShowCase>(ui =>
            {
                var bb = ui.GetBlackboard<ShowCaseUiBlackboard>();
                if (bb != null)
                {
                    bb.SetPlayerNumeric(numeric);
                }
                else
                {
                    log.Error("ShowCaseUiBlackboard missing on UIScene_ShowCase.");
                }
            });
        }

        private async UniTask CloseShowcaseUI()
        {
            if (_uiShowCase != null && !_uiShowCase.Equals(null))
            {
                var uiManager = ClientManager != null ? ClientManager.GetUiManager() : null;
                var bb = _uiShowCase.GetBlackboard<ShowCaseUiBlackboard>();
                bb?.Clear();
                if (uiManager != null)
                {
                    await uiManager.CloseUIAsync(_uiShowCase);
                }
            }

            _uiShowCase = null;
        }
    }
    
    
    //system
    public partial class ShowCaseSceneManager : SceneManagerBase
    {
        private SceneLoadConfig _sceneConfig;

        private ResourceLoader _resLoader;
        
        private Transform _characterRoot;
        private Transform _uiRoot;
        private Transform _audioRoot;
        private Transform _logicRoot;
        
        protected override async UniTask OnEnter()
        {

            log.Info("Enter showcase scene: loading content (data-driven).");
            
            _sceneConfig = await GlobalServices.ResourceLoader.LoadAsset<SceneLoadConfig>(this.name, ResourceAssetType.ScriptableObject);
            
            _resLoader = GlobalServices.ResourceLoader;
            
            if (null == _sceneConfig)
            {
                log.Error("Scene config is missing; skipping content load");
                return;
            }

            if (null == _resLoader)
            {
                log.Error("_resLoader is null");
                return;
            }
            
            await LoadRoom();
            
            var player = await _roomInstance.LoadPlayer();
            
            await OpenShowcaseUI(player);
        }

        protected override async UniTask OnExit()
        {
            log.Info("Exit showcase scene: cleaning up content.");
            await CloseShowcaseUI();
            await UnloadRoom();
        }

        protected override UniTask OnResetScene()
        {
            log.Debug("Reset showcase scene to defaults.");
            return UniTask.CompletedTask;
        }
        
        private async UniTask LoadRoom()
        {
            if (null != _roomInstance)
            {
                log.Error("Fatal error, _roomInstance not null");
                return;
            }
            _roomInstance = gameObject.AddComponent<GamePlayRoomManager>();
            await _roomInstance.OnInitRoomManager();
        }
        
        private async UniTask UnloadRoom()
        {
            if (_roomInstance != null)
            {
                await _roomInstance.OnUnInitRoomManager();
                Destroy(_roomInstance);
                _roomInstance = null;
            }
            await UniTask.CompletedTask;
        }
        
        private void DestroySafe(GameObject go)
        {
            if (go == null) return;
            if (Application.isPlaying) Destroy(go);
            else DestroyImmediate(go);
        }
    }

}
