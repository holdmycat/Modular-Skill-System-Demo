using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Ebonor.UI
{
    public class CommanderInfoWidget : BaseWidget<CommanderInfoViewModel>
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _buffText;
        [SerializeField] private Slider _hpSlider;
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private Button _levelBtn;
        [SerializeField] private Button _levelResetBtn;
        [SerializeField] private Image _commanderImg;

        [Header("GridLayout")]
        [SerializeField] private Transform _rtGridLayout;
        
        private IUIAtlasRepository _uiAtlasRepository;

        private SquadInfoWidget.Factory _squadFactory;
        
        private readonly List<SquadInfoWidget> _listSquadInfoWidget = new List<SquadInfoWidget>();
        
        [Inject]
        public void Construct(IUIAtlasRepository uiAtlasRepository, SquadInfoWidget.Factory factory)
        {
            _uiAtlasRepository = uiAtlasRepository;

            _squadFactory = factory;
            
            
        }
            
            
        
        public RectTransform RectTransform => _rectTransform;
        
        protected override async UniTask OnShowAsync()
        {
            await base.OnShowAsync();
            
            if (ViewModel != null)
            {
                ViewModel.OnDataUpdated += Refresh;
                
                foreach (var variable in _listSquadInfoWidget)
                {
                    await variable.Show();
                }
            }
            
            Refresh();
            
            log.Info("[CommanderInfoWidget] OnShowAsync");
        }

        protected override async UniTask OnHideAsync()
        {
            if (ViewModel != null)
            {
                ViewModel.OnDataUpdated -= Refresh;
                foreach (var variable in _listSquadInfoWidget)
                {
                    await variable.Hide();
                }
            }
            await base.OnHideAsync();
        }

        private void Refresh()
        {
            if (ViewModel == null) return;
            
            if (_nameText) _nameText.text = ViewModel.Name;
            if (_levelText) 
                _levelText.text = $"Commander Lv.{ViewModel.Level}";
            
            if (_buffText) 
                _buffText.text = $"{ViewModel.BuffText}";

            _commanderImg.sprite = _uiAtlasRepository.GetUICharacterAtlas(ViewModel.IconName);

            if (ViewModel.Squads.Count - _listSquadInfoWidget.Count == 1)
            {
                var squadWidget = _squadFactory.Create(ViewModel.Squads[^1]);
                squadWidget.transform.SetParent(_rtGridLayout);
                _listSquadInfoWidget.Add(squadWidget);
                squadWidget.Show().Forget();
                LayoutRebuilder.ForceRebuildLayoutImmediate(_rtGridLayout as RectTransform);
            }
            
            foreach (var variable in _listSquadInfoWidget)
            {
                variable.Refresh();
            }
        
            log.Info("[CommanderInfoWidget] Refresh");
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            if (_levelBtn != null)
            {
                _levelBtn.onClick.AddListener(BtnClickLevelUp);
            }

            if (_levelResetBtn != null)
            {
                _levelResetBtn.onClick.AddListener(BtnClickLevelReset);
            }

            _listSquadInfoWidget.Clear();

            // foreach (var variable in ViewModel.Squads)
            // {
            //     var squadWidget = _squadFactory.Create(variable);
            //     squadWidget.transform.SetParent(_rtGridLayout);
            //     _listSquadInfoWidget.Add(squadWidget);
            // }
        }

        protected override void OnDestroy()
        {
            if (_levelBtn != null)
            {
                _levelBtn.onClick.RemoveListener(BtnClickLevelUp);
            }
            
            if (_levelResetBtn != null)
            {
                _levelResetBtn.onClick.RemoveListener(BtnClickLevelReset);
            }
            
            base.OnDestroy();
        }

        /// <summary>
        /// _levelBtn : Click to Level Up
        /// </summary>
        private void BtnClickLevelUp()
        {
            log.Info($"[{GetType().Name}] BtnClickLevelUp.");
            if (ViewModel != null)
            {
                ViewModel.LevelUp();
            }
        }
        
        private void BtnClickLevelReset()
        {
            log.Info($"[{GetType().Name}] BtnClickLevelReset.");
            if (ViewModel != null)
            {
                ViewModel.LevelReset();
            }
        }
        
        public class Factory : PlaceholderFactory<CommanderInfoViewModel, CommanderInfoWidget>
        {
            public Factory()
            {
                log.Info("[CommanderInfoWidget] Factory Construction");
            }
        }

    }
}
