using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Ebonor.UI
{
    public class CommanderInfoWidget : BaseWidget<CommanderInfoViewModel>
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _levelText;
        [SerializeField] private Slider _hpSlider;
        [SerializeField] private Text _hpText;

        public RectTransform RectTransform => _rectTransform;
        
        protected override async UniTask OnShowAsync()
        {
            await base.OnShowAsync();
            Refresh();
            
            log.Info("[CommanderInfoWidget] OnShowAsync");
        }

        private void Refresh()
        {
            if (ViewModel == null) return;
            
            // if (_nameText) _nameText.text = ViewModel.Name;
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

        // Ideally, subscribe to ViewModel events here to auto-refresh

        public class Factory : PlaceholderFactory<CommanderInfoViewModel, CommanderInfoWidget>
        {
            public Factory()
            {
                log.Info("[CommanderInfoWidget] Factory Construction");
            }
        }

    }
}
