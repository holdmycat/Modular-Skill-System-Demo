using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Ebonor.UI
{
    public class SquadInfoWidget : BaseWidget<SquadInfoViewModel>
    {
        [Header("UI Elements")]
        [SerializeField] private Image _iconImg;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private TMP_Text _atkText;
        [SerializeField] private TMP_Text _hpText;

        private IUIAtlasRepository _uiAtlasRepository;

        [Inject]
        public void Construct(IUIAtlasRepository uiAtlasRepository)
        {
            _uiAtlasRepository = uiAtlasRepository;
        }

        protected override async UniTask OnShowAsync()
        {
            await base.OnShowAsync();
            
            if (ViewModel != null)
            {
                ViewModel.OnDataUpdated += Refresh;
            }
            
            Refresh();
        }

        protected override async UniTask OnHideAsync()
        {
            if (ViewModel != null)
            {
                ViewModel.OnDataUpdated -= Refresh;
            }
            await base.OnHideAsync();
        }

        public void Refresh()
        {
            if (ViewModel == null) return;

            if (_nameText) _nameText.text = ViewModel.UnitName;
            
            if (_levelText) _levelText.text = $"Lv.{ViewModel.Level}";
            
            if (_countText) _countText.text = $"x{ViewModel.Count}";
            
            if (_atkText) _atkText.text = $"ATK: {ViewModel.Attack:F0}";
            
            if (_hpText) _hpText.text = $"HP: {ViewModel.Hp:F0}";
            
            if (_iconImg && _uiAtlasRepository != null)
            {
                var iconName = ViewModel.IconName;
                if (ViewModel.FactionType == FactionType.Player)
                {
                    iconName += "_blue";
                }
                else
                {
                    iconName += "_red";
                }
                _iconImg.sprite = _uiAtlasRepository.GetUICharacterAtlas(iconName);
            }
        }
        
        public class Factory : PlaceholderFactory<SquadInfoViewModel, SquadInfoWidget>
        {
        }
    }
}
