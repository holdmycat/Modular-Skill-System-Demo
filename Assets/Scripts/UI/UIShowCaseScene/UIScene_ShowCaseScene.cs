using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.UI
{
    public class UIScene_ShowCaseScene : BaseWindow<ShowcaseViewModel>
    {
        // No explicit injection needed here anymore, BaseWindow handles it via [Inject] Property.
        // [Header("Widgets")]
        // [UnityEngine.SerializeField] private CommanderInfoWidget _playerWidget;
        // [UnityEngine.SerializeField] private CommanderInfoWidget _enemyWidget;
        
        private static readonly ILog log = LogManager.GetLogger(typeof(UIScene_ShowCaseScene));

        private CommanderInfoWidget _playerWidget;
        
        private CommanderInfoWidget.Factory _factory;
        
        [Inject]
        public void Construct(CommanderInfoWidget.Factory factory)
        {
            log.Info($"[{GetType().Name}] Construct.");
            _factory = factory;
        }
        
        
        protected override async UniTask OnCreateAsync()
        {
            log.Info($"[{GetType().Name}] OnCreateAsync.");
            _playerWidget = _factory.Create(ViewModel.PlayerInfo);
            _playerWidget.transform.SetParent(transform);
            _playerWidget.RectTransform.anchoredPosition = Vector2.zero;
            
           
        }
        
        protected override async UniTask OnOpenAsync()
        {
            // Call base to trigger ViewModel.OnOpen()
            await base.OnOpenAsync();
           
            if (ViewModel != null)
            {
                _playerWidget.Show();
               
                
               // // Bind Widgets
               // if(_playerWidget) _playerWidget.Open(ViewModel.PlayerInfo);
               // if(_enemyWidget)  _enemyWidget.Open(ViewModel.EnemyInfo);
            }
        }

        // protected override async UniTask OnCloseAsync()
        // {
        //     await base.OnCloseAsync();
        // }

        // protected override async UniTask OnDestroyAsync()
        // {
        //     await base.OnDestroyAsync();
        // }
    }
}
