using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ebonor.UI
{

    public partial class UIScene_Loading : UIBase
    {
        protected override async UniTask OnCreateAsync()
        {
            
        }

        protected override async UniTask OnOpenAsync()
        {
            
        }

        protected override async UniTask OnCloseAsync()
        {
           
        }

        protected override async UniTask OnDestroyAsync()
        {
            
        }
    }
    
    public partial class UIScene_Loading : UIBase
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text percentText;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Image rotationImage;
        [SerializeField] private Slider progressSlider;

        [Header("Animation")]
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float fillLerpSpeed = 2f;

        private float _targetPercent;

        /// <summary>Set progress (0–1).</summary>
        public void SetPercent(float percent01)
        {
            _targetPercent = Mathf.Clamp01(percent01);
        }

        /// <summary>Set loading title.</summary>
        public void SetTitle(string title)
        {
            if (titleText != null) titleText.text = title;
        }

        public override void OnUpdate(float deltaTime)
        {
            // Rotate loading icon
            if (rotationImage != null)
            {
                rotationImage.rectTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            }
            
            // Animate slider toward target percent
            if (progressSlider != null)
            {
                float current = progressSlider.value;
                float next = Mathf.MoveTowards(current, _targetPercent, fillLerpSpeed * Time.deltaTime);
                progressSlider.value = next;

                if (percentText != null)
                {
                    percentText.text = $"{Mathf.RoundToInt(next * 100f)}%";
                }
            }
            else if (percentText != null)
            {
                percentText.text = $"{Mathf.RoundToInt(_targetPercent * 100f)}%";
            }
        }
    }
}
