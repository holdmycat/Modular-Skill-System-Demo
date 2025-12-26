using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using UObject = UnityEngine.Object;
namespace Ebonor.Manager
{
    public class SceneLoaderService : ISceneLoaderService
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(SceneLoaderService));
        
        private readonly ResourceLoader _resourceLoader;
        private readonly IInstantiator _instantiator;
        
        // ZenjectSceneLoader is a built-in Zenject service for loading scenes with bindings.
        // If we don't need extra bindings, standard SceneManager works too, but ZenjectSceneLoader is safer for Contexts.
        public SceneLoaderService(ResourceLoader resourceLoader, IInstantiator container)
        {
            _resourceLoader = resourceLoader;
            _instantiator = container;
        }

        public async UniTask LoadSceneAsync(string sceneName)
        {
            log.Info($"[SceneLoaderService] Starting load sequence for: {sceneName}");

            var obj = await _resourceLoader.LoadAsset<UObject>(sceneName, ResourceAssetType.SceneStateManager);

            if (null == obj)
            {
               log.ErrorFormat("[SceneLoaderService] Fatal error, sceneName:{0} not exist", sceneName);
               return;
            }
            
            // Use Zenject's container to instantiate the prefab. 
            // This ensures the GameObjectContext is properly initialized and parented if needed.
            var prefab = obj as GameObject;
            var instance = _instantiator.InstantiatePrefab(prefab);
            instance.name = prefab.name;
            log.Info($"[SceneLoaderService] Instantiated: {instance.name}, Active: {instance.activeInHierarchy}");
            
            // Ensure root before moving to current scene (required by MoveGameObjectToScene)
            instance.transform.SetParent(null, worldPositionStays: false);
            SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());
            
            // Optional: If the prefab was inactive, force it active.
            if (!instance.activeSelf)
            {
                instance.SetActive(true);
            }
            
            // Get the GameObjectContext component to access its container
            var context = instance.GetComponentNoAlloc<GameObjectContext>();
            if (context != null)
            {
                // Resolve ISceneManager from the child container
                var sceneManager = context.Container.Resolve<ISceneManager>();
                if (sceneManager != null)
                {
                    log.Info("[SceneLoaderService] Calling ISceneManager.StartupSequence...");
                    await sceneManager.StartupSequence();
                }
                else
                {
                    log.Warn("[SceneLoaderService] ISceneManager not found in scene context.");
                }
            }
            else
            {
                log.Warn("[SceneLoaderService] GameObjectContext component not found on instantiated object.");
            }
            
            log.Info($"[SceneLoaderService] Load complete: {sceneName}");
        }
    }
}
