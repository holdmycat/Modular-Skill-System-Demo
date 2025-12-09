//------------------------------------------------------------
// File: UIPanel_CharacterProperty.cs
// Purpose: Display basic character properties and live numeric updates.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
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

        [Header("Numeric")]
        [SerializeField] private TMP_Text powerText;
        [SerializeField] private TMP_Text agilityText;
        [SerializeField] private TMP_Text vitalityText;
        [SerializeField] private TMP_Text movementSpeedText;
        [SerializeField] private TMP_Text rotationSpeedText;
        [SerializeField] private TMP_Text lifeText;

        private ActorInstanceBase _actorInstance;
        private ActorNumericComponentBase _numericComp;
        private UnitAttributesNodeDataBase _unitData;
        private bool _subscribed;

        private static readonly Dictionary<eNumericType, Action<UIPanel_CharacterProperty, float>> NumericWriters =
            new Dictionary<eNumericType, Action<UIPanel_CharacterProperty, float>>
            {
                { eNumericType.Power, (p, v) => p.SetText(p.powerText, v) },
                { eNumericType.Agility, (p, v) => p.SetText(p.agilityText, v) },
                { eNumericType.Vitality, (p, v) => p.SetText(p.vitalityText, v) },
                { eNumericType.MovementSpeed, (p, v) => p.SetText(p.movementSpeedText, v) },
                { eNumericType.RotationSpeed, (p, v) => p.SetText(p.rotationSpeedText, v) },
                { eNumericType.Life, (p, v) => p.SetText(p.lifeText, v) },
                { eNumericType.MaxLife, (p, v) => p.SetText(p.lifeText, v) }
            };

        /// <summary>Bind this panel to the provided actor instance.</summary>
        public void Bind(ActorInstanceBase actorInstance)
        {
            Unbind();
            _actorInstance = actorInstance;
            _numericComp = actorInstance != null ? actorInstance.ActorNumericComponentBase : null;
            _unitData = ResolveUnitData();

            RefreshStaticInfo();
            RefreshAllNumeric();
            Subscribe();
        }

        /// <summary>Detach from the current actor and stop listening for updates.</summary>
        public void Unbind()
        {
            if (_subscribed)
            {
                DataEventManager.OnDetach<UnitNumericChange>(OnNumericChange);
                _subscribed = false;
            }

            _actorInstance = null;
            _numericComp = null;
            _unitData = null;
        }

        protected override async UniTask OnShowAsync()
        {
            await UniTask.CompletedTask;
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

        private void RefreshStaticInfo()
        {
            if (_unitData != null)
            {
                SetText(nameText, _unitData.UnitName, string.Empty);
                SetText(professionText, _unitData.HeroProfession.ToString(), string.Empty);
                SetText(levelText, 1f);
                UpdateAvatar(_unitData.UnitAvatar);
            }
            else
            {
                SetText(nameText, "Unknown", string.Empty);
                SetText(professionText, "--", string.Empty);
                SetText(levelText, 0f);
                UpdateAvatar(null);
            }
        }

        private void RefreshAllNumeric()
        {
            foreach (var kvp in NumericWriters)
            {
                var type = kvp.Key;
                float fallback = GetFallbackValue(type);
                float value = GetNumericValue(type, fallback);
                kvp.Value?.Invoke(this, value);
            }
        }

        private void OnNumericChange(UnitNumericChange change)
        {
            if (_numericComp == null || change.UnitAttrComp != _numericComp) return;
            if (NumericWriters.TryGetValue(change.NumericType, out var writer))
            {
                writer?.Invoke(this, change.NumericResult);
            }
        }

        private UnitAttributesNodeDataBase ResolveUnitData()
        {
            if (_numericComp != null && DataCtrl.Inst != null)
            {
                var id = _numericComp.UnitModelNodeId;
                var fromId = DataCtrl.Inst.GetUnitAttributeNodeData(id);
                if (fromId != null) return fromId;
            }

            if (_actorInstance != null && DataCtrl.Inst != null)
            {
                var fromName = DataCtrl.Inst.GetUnitAttributeNodeDataByUnitName(_actorInstance.name);
                if (fromName != null) return fromName;
            }

            return null;
        }

        private float GetNumericValue(eNumericType type, float fallback = 0f)
        {
            if (_numericComp?.NumericDic != null && _numericComp.NumericDic.TryGetValue((int)type, out var value))
            {
                return value;
            }

            return fallback;
        }

        private float GetFallbackValue(eNumericType type)
        {
            if (_unitData == null) return 0f;

            return type switch
            {
                eNumericType.Power => _unitData.Power,
                eNumericType.Agility => _unitData.Agility,
                eNumericType.Vitality => _unitData.Vitality,
                eNumericType.MovementSpeed => _unitData.MovementSpeed,
                eNumericType.RotationSpeed => _unitData.RotationSpeed,
                eNumericType.Life => 0f,
                eNumericType.MaxLife => 0f,
                _ => 0f
            };
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
