using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Signal: UI Unit Selected
    /// Fired when a user selects a unit in the UI or Scene.
    /// </summary>
    public class SignalUI_UnitSelected
    {
        public int NetId;
        public NetworkPrefabType Type;
    }
}
