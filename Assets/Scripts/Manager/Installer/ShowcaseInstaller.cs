using Ebonor.DataCtrl;
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
            // --- Network Layer ---
            // Bind NetworkBus
            Container.BindInterfacesAndSelfTo<SimulatedNetworkBus>().AsSingle();
            
            // Data Providers
            Container.Bind<IPlayerDataProvider>().To<LocalPlayerDataProvider>().AsSingle();
            Container.Bind<ITeamIdGenerator>().To<TeamIdGenerator>().AsSingle();
            
            // Bind ServerTickManager
            Container.BindInterfacesAndSelfTo<ServerTickManager>().AsSingle();

            // --- Server Logic World ---
            Container.Bind<ServerRoomManager>().AsSingle();
            Container.Bind<ServerManager>().AsSingle();

            // --- Client Presentation World ---
            Container.Bind<ClientRoomManager>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<ClientManager>().FromNewComponentOnNewGameObject().AsSingle();
            
            // --- Factories ---
            // Deleted - Re-implement based on Faction hierarchy later

            // --- Scene Root ---
            log.Debug("[ShowcaseInstaller] InstallBindings called.");
            Container.BindInterfacesAndSelfTo<ShowcaseSceneManager>().AsSingle().NonLazy();
        }
    }
}
