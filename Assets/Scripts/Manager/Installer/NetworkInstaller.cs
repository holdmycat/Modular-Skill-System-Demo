using Ebonor.DataCtrl;
using Ebonor.GamePlay;
using Zenject;

namespace Ebonor.Manager
{
    public class NetworkInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Bind the Bus as a Single Interface
            Container.BindInterfacesTo<SimulatedNetworkBus>().AsSingle();
            
            // // Bind the Tick Manager (Simulates Server Loop)
            // Container.BindInterfacesAndSelfTo<ServerTickManager>().AsSingle().NonLazy();

            // // Server Clock (if running headless network simulation)
            // Container.Bind<Clock>().WithId(ClockIds.Server).AsSingle().IfNotBound();
        }
    }
}
