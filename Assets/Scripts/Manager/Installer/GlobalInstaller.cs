using UnityEngine;
using Zenject;
using Ebonor.DataCtrl;
using Ebonor.GamePlay;
using Ebonor.UI;

namespace Ebonor.Manager
{
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField] private GlobalGameConfig _globalConfig;

        public override void InstallBindings()
        {
#if UNITY_EDITOR
            if (IsRunningPlaymodeTests())
            {
                Debug.Log("[GlobalInstaller] Detected playmode test runner; skipping bindings.");
                return;
            }
#endif
            // 1. Bind Global Config
            if (_globalConfig != null)
            {
                Container.BindInstance(_globalConfig).AsSingle();
            }
            else
            {
                Debug.LogWarning("[GlobalInstaller] GlobalGameConfig is missing!");
            }

            // 2. Bind ResourceLoader (if needed globally)
            Container.Bind<ResourceLoader>().AsSingle();
            
            Container.Bind<IInputService>().To<PlayerInputRouter>().FromNewComponentOnNewGameObject().AsSingle();
            
            Container.BindInterfacesAndSelfTo<UIManager>().FromNewComponentOnNewGameObject().AsSingle();
            
            // 3. Bind SceneLoaderService
            Container.Bind<ISceneLoaderService>().To<SceneLoaderService>().AsSingle();

            // 4. Bind SystemDataService (BSON Registration)
            Container.Bind<ISystemDataService>().To<SystemDataService>().AsSingle();


            Container.Bind<IScenarioIdRegistry>().To<ScenarioIdRegistry>().AsSingle();
            Container.Bind<ILegionIdGenerator>().To<LegionIdGenerator>().AsSingle();
            
            // 4.5 Optional services (graceful degradation)
            Container.Bind<IVoiceChatService>().To<NullVoiceChatService>().AsSingle();

            Container.BindInterfacesAndSelfTo<ModelRepository>().AsSingle();
            
            Container.Bind<IDataLoaderService>().To<DataLoaderService>().AsSingle();
            
            //Container.Bind<ICharacterDataRepository>().To<CharacterDataRepository>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterDataRepository>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<PoolManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            
            Container.Bind<IEntityFactory>().To<EntitySpawnService>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<DataEventBusManager>().AsSingle();

            // Bind NumericComponent Factory
            Container.Bind<IActorNumericComponentFactory>().To<ActorNumericComponentFactory>().AsSingle();
            
            //Container.Bind<ISceneManager>().To<ShowcaseSceneManager>().AsSingle();
            
            // 5. Bind GameStartup
            // "BindInterfacesTo" means it will be bound to IInitializable, so Initialize() will be called.
            // "AsSingle" means only one instance.
            // "NonLazy" is CRITICAL: it forces the object to be created immediately on startup.
            Container.BindInterfacesTo<GameStartup>().AsSingle().NonLazy();
        }

#if UNITY_EDITOR
        private static bool IsRunningPlaymodeTests()
        {
            var type = System.Type.GetType("UnityEngine.TestTools.TestRunner.PlaymodeTestsController, UnityEngine.TestRunner");
            var method = type?.GetMethod("IsControllerOnScene", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (method == null)
            {
                return false;
            }

            return method.Invoke(null, null) is bool running && running;
        }
#endif
    }
}
