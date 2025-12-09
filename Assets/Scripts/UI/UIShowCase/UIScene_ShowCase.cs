//------------------------------------------------------------
// File: UIScene_ShowCase.cs
// Purpose: Showcase scene root UI; wires subpanels such as character properties.
//------------------------------------------------------------
using Cysharp.Threading.Tasks;
using Ebonor.GamePlay;
using UnityEngine;

namespace Ebonor.UI
{
    /// <summary>Showcase scene root UI that owns character property panel and scene bindings.</summary>
    public partial class UIScene_ShowCase : UIBase
    {
        [Header("Panels")]
        [SerializeField] private UIPanel_CharacterProperty characterPropertyPanel;

        private GamePlayRoomManager _roomManager;
        
        /// <summary>Find required scene references and cache panel components.</summary>
        protected override async UniTask OnCreateAsync()
        {
            if (characterPropertyPanel == null)
            {
                characterPropertyPanel = GetComponentInChildren<UIPanel_CharacterProperty>(true);
            }

            _roomManager = FindObjectOfType<GamePlayRoomManager>();
            await UniTask.CompletedTask;
        }
        
        protected override async UniTask OnDestroyAsync()
        {
            characterPropertyPanel?.Unbind();
            await UniTask.CompletedTask;
        }
        
        /// <summary>Open UI and bind to the active player instance if available.</summary>
        protected override async UniTask OnOpenAsync()
        {
            var player = _roomManager != null ? _roomManager.PlayerActorInstance : null;
            characterPropertyPanel?.Bind(player);
            if (characterPropertyPanel != null)
            {
                await characterPropertyPanel.ShowAsync();
            }
        }

        protected override async UniTask OnCloseAsync()
        {
            if (characterPropertyPanel != null)
            {
                await characterPropertyPanel.HideAsync();
                characterPropertyPanel.Unbind();
            }
        }
    }
}
