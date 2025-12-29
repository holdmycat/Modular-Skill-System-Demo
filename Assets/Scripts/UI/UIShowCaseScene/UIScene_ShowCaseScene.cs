using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.UI
{
    public class UIScene_ShowCaseScene : BaseWindow<ShowcaseViewModel>
    {

        private CommanderInfoWidget _playerWidget;
        private CommanderInfoWidget _enemyWidget;
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
            
            _enemyWidget = _factory.Create(ViewModel.EnemyInfo);
            _enemyWidget.transform.SetParent(transform);
            _enemyWidget.RectTransform.anchoredPosition = new Vector2(Screen.width - _enemyWidget.RectTransform.sizeDelta.x, 0f);




        }
        
        protected override async UniTask OnOpenAsync()
        {
            log.Info($"[{GetType().Name}] OnOpenAsync.");
            // Call base to trigger ViewModel.OnOpen()
            await base.OnOpenAsync();
           
            if (ViewModel != null)
            {
                await _playerWidget.Show();
                await _enemyWidget.Show();
                
               // // Bind Widgets
               // if(_playerWidget) _playerWidget.Open(ViewModel.PlayerInfo);
               // if(_enemyWidget)  _enemyWidget.Open(ViewModel.EnemyInfo);
            }
        }

        protected override async UniTask OnCloseAsync()
        {
            log.Info($"[{GetType().Name}] OnCloseAsync.");
            if (ViewModel != null)
            {
                await _playerWidget.Hide();
                await _enemyWidget.Hide();
            }
            await base.OnCloseAsync();
        }

        // protected override async UniTask OnDestroyAsync()
        // {
        //     await base.OnDestroyAsync();
        // }
    }
}
