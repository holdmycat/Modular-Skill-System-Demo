//------------------------------------------------------------
// File: UIPanel_CharacterProperty.cs
// Purpose: Display basic character properties and live numeric updates.
//------------------------------------------------------------

using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ebonor.UI
{
    public class UIPanel_CharacterProperty : UIPanelBase
    {
        [Header("Static Info")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text professionText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Image avatarImage;
        [SerializeField] private Sprite defaultAvatar;

        
        [SerializeField] public GameObject goSpawnPropertyItem;
        [SerializeField] public GameObject goSpawnPropertyPool;
        [SerializeField] public GridLayoutGroup propertyGridLayout;
        [SerializeField] public RectTransform propertyRectTransform;

        [SerializeField] public Button btnShowHide;
        [SerializeField] public RectTransform btnShowHideRect;
        
        // [Header("Numeric")]
        // [SerializeField] private TMP_Text powerText;
        // [SerializeField] private TMP_Text agilityText;
        // [SerializeField] private TMP_Text vitalityText;
        // [SerializeField] private TMP_Text movementSpeedText;
        // [SerializeField] private TMP_Text rotationSpeedText;
        // [SerializeField] private TMP_Text lifeText;

        private ActorNumericComponentBase _numericComp;
        private UnitAttributesNodeDataBase _unitData;
        private bool _subscribed;
        private StringBuilder _strBld;
        private Vector2 _shownPos;
        private Vector2 _hiddenPos;
        private bool _isHidden;
        private bool _positionsReady;
        private CancellationTokenSource _toggleCts;
      
        /// <summary>Bind this panel to the provided actor numeric component.</summary>
        public void Bind(ActorNumericComponentBase numericComp)
        {

            _strBld ??= new StringBuilder();
            _strBld.Clear();
            
            Unbind();
            _numericComp = numericComp;
            _unitData = ResolveUnitData();
            btnShowHide.onClick.AddListener(ClickShowHide);
            Subscribe();
        }

        /// <summary>Detach from the current actor and stop listening for updates.</summary>
        public void Unbind()
        {
            btnShowHide.onClick.RemoveListener(ClickShowHide);
            UnSubscribe();
            _numericComp = null;
            _unitData = null;
            if (null != _strBld)
            {
                _strBld.Clear();
            }
            _positionsReady = false;
            _isHidden = false;
            _toggleCts?.Cancel();
            _toggleCts?.Dispose();
            _toggleCts = null;
          
        }

        public void ClickShowHide()
        {
            if (!_positionsReady)
            {
                ComputePositions();
            }
            if (!_positionsReady) return;

            _toggleCts?.Cancel();
            _toggleCts?.Dispose();
            _toggleCts = new CancellationTokenSource();
            var target = _isHidden ? _shownPos : _hiddenPos;
            _ = SlideToAsync(target, 0.2f, _toggleCts.Token);
            _isHidden = !_isHidden;
        }
        

        protected override async UniTask OnShowAsync()
        {
            RefreshStaticInfo();
            
            var displayList = _numericComp != null ? _numericComp.DisplayNumericTypes : null;
            var count = 0;
            if (displayList != null && displayList.Count > 0)
            {
                count = displayList.Count;
            }
            
            UIHelper.OnDynamicLoadItem(goSpawnPropertyPool, goSpawnPropertyItem, propertyGridLayout, count);
            RefreshAllNumeric();
            LayoutRebuilder.ForceRebuildLayoutImmediate(propertyRectTransform);
            ComputePositions();
        }

        protected override async UniTask OnHideAsync()
        {
            await UniTask.CompletedTask;
        }

        private void Subscribe()
        {
            if (_numericComp == null) return;
            DataEventManager.OnAttach<UnitNumericChange>(OnNumericChange);
            _subscribed = true;
        }
        
        private void UnSubscribe()
        {
            if (_subscribed)
            {
                DataEventManager.OnDetach<UnitNumericChange>(OnNumericChange);
                _subscribed = false;
            }
        }

        private void RefreshStaticInfo()
        {
            if (_unitData != null)
            {
                _strBld.Clear();
                _strBld.Append("Name: ");
                _strBld.Append(_unitData.UnitName);
                SetText(nameText, _strBld.ToString(), string.Empty);
                
                _strBld.Clear();
                _strBld.Append("Pro: ");
                _strBld.Append(_unitData.HeroProfession);
                SetText(professionText, _strBld.ToString(), string.Empty);
                
                _strBld.Clear();
                _strBld.Append("Lv: ");
                _strBld.Append(_numericComp.GetUILevel());
                SetText(levelText, _strBld.ToString(), string.Empty);
                //UpdateAvatar(_unitData.UnitAvatar);
            }
            else
            {
                SetText(nameText, "Unknown", string.Empty);
                SetText(professionText, "--", string.Empty);
                SetText(levelText, 0f);
                //UpdateAvatar(null);
            }
        }

        private void RefreshAllNumeric()
        {
            var displayList = _numericComp != null ? _numericComp.DisplayNumericTypes : null;
            if (displayList != null && displayList.Count > 0)
            {
                var i = 0;
                foreach (var type in displayList)
                {
                    var trans = propertyGridLayout.transform.GetChild(i);
                    var item = trans.gameObject.GetComponent<UIItem_Font>();
                    var key = type;
                    var value = _numericComp[type];
                    _strBld.Clear();
                    _strBld.Append(key.ToString());
                    _strBld.Append(": ");
                    _strBld.Append(value);
                    item.UpdatePropertyText(_strBld.ToString());
                    i++;
                }
            }
        }

        private void ComputePositions()
        {
            if (rectTransform == null || btnShowHide == null) return;

            var btnRect = btnShowHideRect != null ? btnShowHideRect : btnShowHide.transform as RectTransform;
            if (btnRect == null) return;

            _shownPos = rectTransform.anchoredPosition;

            var width = rectTransform.rect.width;
            var btnWidth = btnRect.rect.width;
            var offset = Mathf.Max(0f, width);
            _hiddenPos = _shownPos + new Vector2(-offset, 0f);
            _positionsReady = true;
        }

        private async UniTaskVoid SlideToAsync(Vector2 target, float duration, CancellationToken token)
        {
            if (rectTransform == null) return;
            var start = rectTransform.anchoredPosition;
            float timer = 0f;
            while (timer < duration)
            {
                if (token.IsCancellationRequested || rectTransform == null) return;
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                float eased = Mathf.SmoothStep(0f, 1f, t);
                rectTransform.anchoredPosition = Vector2.LerpUnclamped(start, target, eased);
                await UniTask.Yield();
            }

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = target;
            }
        }

        private void OnNumericChange(UnitNumericChange change)
        {
            if (_numericComp == null || change.UnitAttrComp != _numericComp) return;
            // if (NumericWriters.TryGetValue(change.NumericType, out var writer))
            // {
            //     writer?.Invoke(this, change.NumericResult);
            // }
        }

        private UnitAttributesNodeDataBase ResolveUnitData()
        {
            if (_numericComp != null && DataCtrl.DataCtrl.Inst != null)
            {
                var id = _numericComp.UnitModelNodeId;
                var fromId = DataCtrl.DataCtrl.Inst.GetUnitAttributeNodeData(id);
                if (fromId != null) return fromId;
            }

            return null;
        }
        
        private float GetFallbackValue(eNumericType type)
        {
            if (_unitData == null) return 0f;

            return _numericComp[type];
            
        }

        private void SetText(TMP_Text text, float value, string format = "0")
        {
            if (text == null) return;
            text.text = value.ToString(format);
        }

        private void SetText(TMP_Text text, string value, string defaultValue)
        {
            if (text == null) return;
            text.text = string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        private void UpdateAvatar(string avatarPath)
        {
            if (avatarImage == null) return;

            if (string.IsNullOrEmpty(avatarPath))
            {
                avatarImage.sprite = defaultAvatar;
                return;
            }

            var sprite = Resources.Load<Sprite>(avatarPath);
            avatarImage.sprite = sprite != null ? sprite : defaultAvatar;
        }
    }
}
