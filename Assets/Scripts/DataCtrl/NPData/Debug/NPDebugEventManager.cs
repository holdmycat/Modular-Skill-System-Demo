using System.Collections.Generic;

namespace Ebonor.DataCtrl
{
#if UNITY_EDITOR
    /// <summary>
    /// Editor-only hub for NP debug events.
    /// </summary>
    public static class NPDebugEventManager
    {
        public static INPNodeDebugListener Listener { get; set; }

        public static void Raise(NP_RuntimeTree tree, Node node, NPDebugEventType evt, bool success)
        {
            Listener?.OnNodeEvent(tree, node, evt, success);
        }
    }
#endif
}
