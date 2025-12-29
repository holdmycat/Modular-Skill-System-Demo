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

        private IUIAtlasRepository _uiAtlasRepository;
        
        [Inject]
        public void Construct(IUIAtlasRepository uiAtlasRepository)
        {
            _uiAtlasRepository = uiAtlasRepository;
        }
            
            
        
        public RectTransform RectTransform => _rectTransform;
        
        protected override async UniTask OnShowAsync()
        {
            await base.OnShowAsync();
            
            if (ViewModel != null)
            {
                ViewModel.OnDataUpdated += Refresh;
            }
            
            Refresh();
            
            log.Info("[CommanderInfoWidget] OnShowAsync");
        }

        protected override async UniTask OnHideAsync()
        {
            if (ViewModel != null)
            {
                ViewModel.OnDataUpdated -= Refresh;
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
            
            //
            // if (_hpSlider)
            // {
            //     _hpSlider.maxValue = ViewModel.MaxHealth;
            //     _hpSlider.value = ViewModel.Health;
            // }
            //
            // if (_hpText)
            // {
            //     _hpText.text = $"{ViewModel.Health}/{ViewModel.MaxHealth}";
            // }
            
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
