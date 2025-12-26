using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.GamePlay;
using UnityEngine;
using Zenject;

namespace Ebonor.Manager
{
    public class ShowcaseInstaller : MonoInstaller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowcaseInstaller));

        [Header("Scene Id")]
        public string SceneId;
        
        public override void InstallBindings()
        {
            // --- Network Layer ---
            // Bind NetworkBus
            Container.BindInterfacesAndSelfTo<SimulatedNetworkBus>().AsSingle();
            
            // Bind ServerTickManager
            Container.BindInterfacesAndSelfTo<ServerTickManager>().AsSingle();
            
            //Bind SceneResourceManager
            Container.Bind<ISceneResourceManager>()
                .To<SceneResourceManager>()
                .AsSingle()
                .WithArguments(SceneId);
            
            // --- Server Logic World ---
            Container.Bind<ServerRoomManager>().AsSingle();
            Container.Bind<ServerManager>().AsSingle();

            // --- Client Presentation World ---
            Container.Bind<ClientRoomManager>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<ClientManager>().FromNewComponentOnNewGameObject().AsSingle();
            
            Container.BindFactory<ServerCommander, ServerCommander.Factory>().AsSingle();
            Container.BindFactory<ClientCommander, ClientCommander.Factory>().AsSingle();
            
            Container.BindFactory<ServerLegion, ServerLegion.Factory>().AsSingle();
            Container.BindFactory<ClientLegion, ClientLegion.Factory>().AsSingle();

            Container.BindFactory<ServerSquad, ServerSquad.Factory>().AsSingle();
            Container.BindFactory<ClientSquad, ClientSquad.Factory>().AsSingle();
            
            
            // --- Scene Root ---
            log.Debug("[ShowcaseInstaller] InstallBindings called.");
            Container.BindInterfacesAndSelfTo<ShowcaseSceneManager>().AsSingle().NonLazy();
            
          
            
        }
    }
}
