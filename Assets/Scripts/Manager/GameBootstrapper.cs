using System;
using UnityEngine;
using Zenject;
using Ebonor.DataCtrl;
using Ebonor.UI;

namespace Ebonor.Manager
{
    public class GameBootstrapper : MonoBehaviour
    {
        private IInputService _inputService;
        private IUIService _uiService;

        [Inject]
        public void Construct(IInputService inputService, IUIService uiService)
        {
            _inputService = inputService;
            _uiService = uiService;
        }

        private async void Start()
        {
            Debug.Log("[GameBootstrapper] Starting game initialization...");
            
            // 1. Initialize UI
            // UIManager needs a root. In the old code, it was passed 'transform'.
            // Here, we might need to ensure the UIManager has a canvas or root.
            // Since UIManager is instantiated by Zenject, we can access it via interface.
            // But IUIService doesn't expose Init. We might need to cast or add Init to interface.
            // For purity, let's assume UIManager finds its own root or we pass it via Zenject Factory.
            // For this refactor, let's cast to UIManager to call Init, or assume it's self-initialized.
            // if (_uiService is UIManager uiManager)
            // {
            //     // Create a UI Root under this Bootstrapper for organization
            //     var uiRoot = new GameObject("UI_Root").transform;
            //     uiRoot.SetParent(transform);
            //     uiManager.Init(uiRoot);
            //     
            //     UIScene_Loading uiLoading = null;
            //     IProgress<float> progressReporter;
            //     uiLoading = await uiManager.OpenUIAsync<UIScene_Loading>();
            //     progressReporter = new System.Progress<float>(progress =>
            //     {
            //         //log.Info($"Global Loading Progress: {progress * 100:F0}%");
            //         uiLoading?.SetPercent(progress);
            //     });
            //     
            //     await DataCtrl.DataCtrl.Inst.LoadAllSystemDataAsync(progressReporter);
            //     Debug.Log("[GameBootstrapper] Data loaded.");
            //     
            //     
            //     
            //     
            //     Debug.Log("[GameBootstrapper] Initialization complete.");
            // }
            
        }
    }
}
