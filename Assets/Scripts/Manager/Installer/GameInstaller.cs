using UnityEngine;
using Ebonor.DataCtrl;
using Ebonor.UI;
using Zenject;

namespace Ebonor.Manager
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private PlayerInputRouter _inputRouterPrefab;
        private UIManager _uiManagerPrefab;

        public override void InstallBindings()
        {
            // Bind Input Service
            // We assume the prefab is assigned in the inspector.
            // If it's already in the scene, we could use FromComponentInHierarchy, but FromComponentInNewPrefab is safer for a fresh start.
            if (_inputRouterPrefab != null)
            {
                Container.BindInterfacesAndSelfTo<PlayerInputRouter>()
                    .FromComponentInNewPrefab(_inputRouterPrefab)
                    .AsSingle()
                    .NonLazy();
            }
            else
            {
                // Fallback: Bind to a new GameObject if no prefab
                Container.BindInterfacesAndSelfTo<PlayerInputRouter>()
                    .FromNewComponentOnNewGameObject()
                    .AsSingle()
                    .NonLazy();
            }

            // Bind UI Service
            Container.BindInterfacesAndSelfTo<UIManager>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();

            // Bind DataCtrl (Singleton)
            // DataCtrl is a bit special as it's a singleton. We can bind it to the existing instance or manage it via Zenject.
            // For now, let's bind it as a simple external dependency if needed, or just let Bootstrapper handle it.
        }
    }
}
