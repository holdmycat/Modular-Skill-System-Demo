using System;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Ebonor.UI;
using UnityEngine.Assertions; // Assuming UIScene_Loading is here or we use IUIService to get it
using UObject = UnityEngine.Object;
namespace Ebonor.Manager
{
    public class SceneLoaderService : ISceneLoaderService
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(SceneLoaderService));
        
        private readonly ZenjectSceneLoader _zenjectSceneLoader;
        private readonly ResourceLoader _resourceLoader;
        private readonly DiContainer _container;
        
        // ZenjectSceneLoader is a built-in Zenject service for loading scenes with bindings.
        // If we don't need extra bindings, standard SceneManager works too, but ZenjectSceneLoader is safer for Contexts.
        public SceneLoaderService(ResourceLoader resourceLoader, DiContainer container)
        {
            _resourceLoader = resourceLoader;
            _container = container;
        }

        public async UniTask LoadSceneAsync(string sceneName)
        {
            log.Debug($"[SceneLoaderService] Starting load sequence for: {sceneName}");

            var obj = await _resourceLoader.LoadAsset<UObject>(sceneName, ResourceAssetType.SceneStateManager);

            if (null == obj)
            {
               log.ErrorFormat("[SceneLoaderService] Fatal error, sceneName:{0} not exist", sceneName);
               return;
            }
            
            // Use Zenject's container to instantiate the prefab. 
            // This ensures the GameObjectContext is properly initialized and parented if needed.
            var prefab = obj as GameObject;
            var instance = _container.InstantiatePrefab(prefab);
            
            log.Debug($"[SceneLoaderService] Instantiated: {instance.name}, Active: {instance.activeInHierarchy}");
            
            // Optional: If the prefab was inactive, force it active.
            if (!instance.activeSelf)
            {
                instance.SetActive(true);
            }
            
            log.Debug($"[SceneLoaderService] Load complete: {sceneName}");
        }
    }
}
