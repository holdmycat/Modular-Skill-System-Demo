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
            // if (_levelText) _levelText.text = $"Lv.{ViewModel.Level}";
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
            _levelBtn.onClick.AddListener(BtnClickLevelUp);
        }

        protected override void OnDestroy()
        {
            if (_levelBtn != null)
            {
                _levelBtn.onClick.RemoveListener(BtnClickLevelUp);
            }
            base.OnDestroy();
        }

        /// <summary>
        /// _levelBtn : Click to Level Up
        /// </summary>
        private void BtnClickLevelUp()
        {
            log.Info($"[{GetType().Name}] BtnClickLevelUp.");
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
