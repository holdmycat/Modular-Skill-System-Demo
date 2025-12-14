using Ebonor.Framework;
using Ebonor.GamePlay;
using Zenject;

namespace Ebonor.Manager
{
    public class ShowcaseInstaller : MonoInstaller
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowcaseInstaller));
        
        public override void InstallBindings()
        {
            //room manager
            Container.BindInterfacesAndSelfTo<RoomManagerService>().FromNewComponentOnNewGameObject().AsSingle();
            
            log.Debug("[ShowcaseInstaller] InstallBindings called.");
            Container.BindInterfacesAndSelfTo<ShowcaseSceneManager>().AsSingle().NonLazy();
        }
    }
}
