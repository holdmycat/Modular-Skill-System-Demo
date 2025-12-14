using UnityEngine;
using Zenject;
using Ebonor.DataCtrl;
using Ebonor.UI;

namespace Ebonor.Manager
{
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField] private GlobalGameConfig _globalConfig;

        public override void InstallBindings()
        {
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

            Container.BindInterfacesAndSelfTo<ModelRepository>().AsSingle();
            
            Container.Bind<IDataLoaderService>().To<DataLoaderService>().AsSingle();
            
            //Container.Bind<ICharacterDataRepository>().To<CharacterDataRepository>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterDataRepository>().AsSingle();
            
            // 5. Bind GameStartup
            // "BindInterfacesTo" means it will be bound to IInitializable, so Initialize() will be called.
            // "AsSingle" means only one instance.
            // "NonLazy" is CRITICAL: it forces the object to be created immediately on startup.
            Container.BindInterfacesTo<GameStartup>().AsSingle().NonLazy();
        }
    }
}