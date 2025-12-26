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
            // --- Client Presentation World ---
            Container.Bind<ClientRoomManager>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<ClientManager>().FromNewComponentOnNewGameObject().AsSingle();
            
            // Bind Commanders with SubContainer
            Container.BindFactory<ServerCommander, ServerCommander.Factory>()
                .FromSubContainerResolve()
                .ByMethod(InstallServerCommander)
                .AsSingle();

            Container.BindFactory<ClientCommander, ClientCommander.Factory>()
                .FromSubContainerResolve()
                .ByMethod(InstallClientCommander)
                .AsSingle();
            
            // Logic for Legion/Squad factories is now inside the Commander's SubContainer
            
            // --- Scene Root ---
            
            
            // --- Scene Root ---
            log.Debug("[ShowcaseInstaller] InstallBindings called.");
            Container.BindInterfacesAndSelfTo<ShowcaseSceneManager>().AsSingle().NonLazy();
            
          
            
        }
        

        private void InstallServerCommander(DiContainer subContainer)
        {
            // Bind Scoped Context Data (Initially empty, populated later via Configure or other means if creating dynamically, 
            // BUT for factories, we usually pass args. 
            // Wait, BindFactory above didn't specify args. 
            // Standard generic factory create() returns T.
            // ServerCommander is creating its own context data from BootstrapInfo usually.
            // BUT, if we want Legion/Squad to access it, we must bind it here.
            
            // PROBLEM: We don't have the BootstrapInfo at the moment of Factory.Create().
            // ServerCommander.Configure() sets it.
            // So we need to simple bind the *Classes* here.
            // AND we need a way to share the data.
            
            // Solution: Bind CommanderContextData as Single in this Scope.
            subContainer.Bind<CommanderContextData>().AsSingle();
            
            subContainer.Bind<ServerCommander>().AsSingle();
            
            // Bind Factories in this Scope so they can access CommanderContextData
            subContainer.BindFactory<ServerLegion, ServerLegion.Factory>().AsSingle();
            subContainer.BindFactory<ServerSquad, ServerSquad.Factory>().AsSingle();
        }

        private void InstallClientCommander(DiContainer subContainer)
        {
            subContainer.Bind<CommanderContextData>().AsSingle();
            
            subContainer.Bind<ClientCommander>().AsSingle();
            
            subContainer.BindFactory<ClientLegion, ClientLegion.Factory>().AsSingle();
            subContainer.BindFactory<ClientSquad, ClientSquad.Factory>().AsSingle();
        }
    }
}
