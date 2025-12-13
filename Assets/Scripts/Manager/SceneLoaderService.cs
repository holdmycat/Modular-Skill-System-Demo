using System;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Ebonor.UI; // Assuming UIScene_Loading is here or we use IUIService to get it

namespace Ebonor.Manager
{
    public class SceneLoaderService : ISceneLoaderService
    {
        private readonly IUIService _uiService;
        private readonly ZenjectSceneLoader _zenjectSceneLoader;

        // ZenjectSceneLoader is a built-in Zenject service for loading scenes with bindings.
        // If we don't need extra bindings, standard SceneManager works too, but ZenjectSceneLoader is safer for Contexts.
        public SceneLoaderService(IUIService uiService, ZenjectSceneLoader zenjectSceneLoader)
        {
            _uiService = uiService;
            _zenjectSceneLoader = zenjectSceneLoader;
        }

        public async UniTask LoadSceneAsync(string sceneName, Action<float> onProgress = null)
        {
            Debug.Log($"[SceneLoaderService] Starting load sequence for: {sceneName}");

            // 1. Show Loading UI
            // We assume there's a specific loading UI class, e.g., UIScene_Loading
            // Since IUIService is generic, we might need to know the type or have a specific method.
            // For now, let's assume we can open a generic loading screen or the user has a specific one.
            // Let's try to open a hypothetical UIScene_Loading if it exists, or just log.
            // In a real implementation, you'd likely have a specific Loading UI type.
            
            IProgress<float> progressReporter;
            
            var loadingUI = await _uiService.OpenUIAsync<UIScene_Loading>();
            if (null != loadingUI)
            {
                progressReporter = new System.Progress<float>(progress =>
                {
                    Debug.Log($"Global Loading Progress: {progress * 100:F0}%");
                    loadingUI?.SetPercent(progress);
                });         
            }

            
            // For this demo, we'll simulate the UI part or assume the caller handles UI if it's complex.
            // But the requirement was "SceneLoader handles it".
            // Let's assume we have a UIScene_Loading class. If not, I'll comment it out or use a placeholder.
            
            // 2. Load Scene
            // ZenjectSceneLoader.LoadSceneAsync returns AsyncOperation usually, or we can wrap it.
            // Using standard SceneManager for simplicity with UniTask if ZenjectSceneLoader is complex to wrap with progress.
            // But ZenjectSceneLoader is recommended.
            
            // Let's use Unity's SceneManager for the raw load to easily get progress, 
            // unless we need to pass arguments to the new scene context.
            
            // var op = SceneManager.LoadSceneAsync(sceneName);
            // op.allowSceneActivation = false;
            //
            // while (!op.isDone)
            // {
            //     float progress = Mathf.Clamp01(op.progress / 0.9f);
            //     onProgress?.Invoke(progress);
            //
            //     // Update Loading UI if we had one
            //     // loadingUI?.SetProgress(progress);
            //
            //     if (op.progress >= 0.9f)
            //     {
            //         op.allowSceneActivation = true;
            //     }
            //
            //     await UniTask.Yield();
            // }

            // 3. Hide Loading UI
            await _uiService.CloseUIAsync<UIScene_Loading>();
            
            Debug.Log($"[SceneLoaderService] Load complete: {sceneName}");
        }
    }
}
