using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.GamePlay
{
    public class CharacterEntity : GameEntity
    {
        protected ActorNumericComponentBase _numericComponent;
        public ActorNumericComponentBase NumericComponent => _numericComponent;

        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            // Additional character initialization if needed
        }

        public async UniTask LoadDataAsync<T>(CharacterRuntimeData runtimeData) where T : ActorNumericComponentBase
        {
            if (_numericComponent == null)
            {
                // Use Zenject to instantiate the component so it gets injected if needed
                _numericComponent = _container.InstantiateComponent<T>(gameObject);
            }

            _numericComponent.OnInitActorNumericComponent(runtimeData, NetId);
            await UniTask.CompletedTask;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_numericComponent != null)
            {
                _numericComponent.OnUnInitActorNumericComponent();
            }
        }
    }
}
