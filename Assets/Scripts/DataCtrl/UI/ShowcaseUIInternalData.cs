using UnityEngine;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Blackboard for sharing transient UI state between panels in the Showcase Scene.
    /// Values here are not persistent game data, but UI-specific state (e.g. selection).
    /// </summary>
    public class ShowcaseUIInternalData
    {
        /// <summary>
        /// NetId of the currently selected entity (Legion, Squad, Commander, etc.)
        /// -1 indicates no selection.
        /// </summary>
        public int SelectedEntityNetId { get; set; } = -1;

        /// <summary>
        /// The type of the currently selected entity, or None.
        /// </summary>
        public NetworkPrefabType SelectedEntityType { get; set; } = NetworkPrefabType.None;

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public void ClearSelection()
        {
            SelectedEntityNetId = -1;
            SelectedEntityType = NetworkPrefabType.None;
        }
    }
}
