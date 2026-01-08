using Cysharp.Threading.Tasks;
using TMPro;
using Ebonor.DataCtrl;
using UnityEngine;
using Zenject;

namespace Ebonor.UI
{
    public class UIScene_ShowCaseScene : BaseWindow<ShowcaseViewModel>
    {

        private CommanderInfoWidget _playerWidget;
        private CommanderInfoWidget _enemyWidget;
        private CommanderInfoWidget.Factory _factory;
        private BattleStateManager _battleStateManager;

        [SerializeField] private TMP_Text _battleStatusText;
        
        [Inject]
        public void Construct(CommanderInfoWidget.Factory factory, BattleStateManager battleStateManager)
        {
            log.Info($"[{GetType().Name}] Construct.");
            _factory = factory;
            _battleStateManager = battleStateManager;
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

            EnsureBattleStatusText();

        }
        
        protected override async UniTask OnOpenAsync()
        {
            log.Info($"[{GetType().Name}] OnOpenAsync.");
            // Call base to trigger ViewModel.OnOpen()
            await base.OnOpenAsync();

            DataEventManager.OnAttach<ClientAllSquadsIdleEvent>(OnClientAllSquadsIdle);
           
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
            DataEventManager.OnDetach<ClientAllSquadsIdleEvent>(OnClientAllSquadsIdle);
            if (ViewModel != null)
            {
                await _playerWidget.Hide();
                await _enemyWidget.Hide();
            }
            await base.OnCloseAsync();
        }

        private void OnClientAllSquadsIdle(ClientAllSquadsIdleEvent evt)
        {
            log.Info($"[Squad Behavior][{GetType().Name}] All squads idle. Commander:{evt.CommanderNetId} Count:{evt.SquadCount}");
        }

        private void Update()
        {
            if (_battleStateManager == null || _battleStatusText == null)
            {
                return;
            }

            _battleStatusText.text = $"Battle: {_battleStateManager.State}\nTime: {_battleStateManager.GetFormattedTime()}";
        }

        private void EnsureBattleStatusText()
        {
            if (_battleStatusText != null)
            {
                return;
            }

            var go = new GameObject("BattleStatusText", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            _battleStatusText = go.AddComponent<TextMeshProUGUI>();
            _battleStatusText.fontSize = 22;
            _battleStatusText.alignment = TextAlignmentOptions.TopLeft;

            var rect = _battleStatusText.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(16f, -16f);
            rect.sizeDelta = new Vector2(320f, 64f);
        }

        // protected override async UniTask OnDestroyAsync()
        // {
        //     await base.OnDestroyAsync();
        // }
    }
}
