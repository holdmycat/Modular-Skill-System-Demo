using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.GamePlay;
using Ebonor.UI;
using UnityEngine;
using Zenject;

namespace Ebonor.Manager
{
    public partial class ShowcaseInstaller : MonoInstaller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowcaseInstaller));

        [Header("Scene Id")]
        public string SceneId;
        
        [Header("UI Commander Info Widget")]
        public GameObject UICommanderInfoWidget;
        
        [Header("UI Squad Info Widget")]
        public GameObject UISquadInfoWidget;
        
        public override void InstallBindings()
        {
            // --- Signals & Event Bus ---
            SignalBusInstaller.Install(Container);
            
            // Declare Signals
            Container.DeclareSignal<SignalUI_UnitSelected>();
            
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
            
            // --- Client Presentation World ---
            Container.Bind<ClientRoomManager>().FromNewComponentOnNewGameObject().AsSingle();
            
            // Bind Context (Data Layer)
            Container.Bind<ShowcaseContext>().AsSingle();
            
            // Bind ViewModels (Logic Layer)
            Container.Bind<ShowcaseViewModel>().AsTransient();
            
            // Bind Prefab(UICommanderInfoWidget)
            Container.BindFactory<CommanderInfoViewModel, CommanderInfoWidget, CommanderInfoWidget.Factory>()
                .FromComponentInNewPrefab(UICommanderInfoWidget);
            
            // Bind Prefab(UISquadInfoWidget)
            Container.BindFactory<SquadInfoViewModel, SquadInfoWidget, SquadInfoWidget.Factory>()
                .FromComponentInNewPrefab(UISquadInfoWidget);
            
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
            // 1. 绑定抽象基类指向具体的 Showcase 实现
            Container.Bind<BaseServerManager>().To<ShowCaseSceneServerManager>().AsSingle();
            Container.Bind<BaseClientManager>().To<ShowCaseSceneClientManager>().FromNewComponentOnNewGameObject().AsSingle();
            
            // 2. 绑定通用的双世界管理器
            // 它可以直接注入 BaseServerManager/BaseClientManager
            Container.BindInterfacesAndSelfTo<DualWorldSceneManager>().AsSingle().NonLazy();

            InstallLocalBindings();
            
            // --- Scene Root ---
            log.Info("[ShowcaseInstaller] InstallBindings called.");
        }


        private GlobalGameConfig _globalGameConfig;
        private IScenarioIdRegistry _scenarioIdRegistry;

        [Inject]
        public void Construct(GlobalGameConfig globalGameConfig, IScenarioIdRegistry scenarioIdRegistry)
        {
            _globalGameConfig = globalGameConfig;
            _scenarioIdRegistry = scenarioIdRegistry;

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
            subContainer.BindFactory<ServerSquad, ServerSquad.Factory>().AsSingle();
            
            // Bind Numeric Factories
            subContainer.BindFactory<uint, CommanderNumericComponent, CommanderNumericComponent.Factory>().AsSingle();
            subContainer.BindFactory<uint, SlgUnitSquadAttributesNodeData, SquadNumericComponent, SquadNumericComponent.Factory>().AsSingle();
            
            // Bind the Wrapper Factory
            subContainer.Bind<NumericComponentFactory>().AsSingle();
            
            
        }
        
        private void InstallClientCommander(DiContainer subContainer)
        {
            subContainer.Bind<CommanderContextData>().AsSingle();
            
            subContainer.Bind<ClientCommander>().AsSingle();
            
            subContainer.BindFactory<ClientSquad, ClientSquad.Factory>().AsSingle();
            
            // Bind Numeric Factories
            subContainer.BindFactory<uint, CommanderNumericComponent, CommanderNumericComponent.Factory>().AsSingle();
            subContainer.BindFactory<uint, SlgUnitSquadAttributesNodeData,SquadNumericComponent, SquadNumericComponent.Factory>().AsSingle();
            
            // Bind the Wrapper Factory
            subContainer.Bind<NumericComponentFactory>().AsSingle();
            
            
        }

        partial void InstallLocalBindings();
    }

    
}

