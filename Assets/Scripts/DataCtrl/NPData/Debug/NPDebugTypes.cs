namespace Ebonor.DataCtrl
{
#if UNITY_EDITOR
    public enum NPDebugEventType
    {
        Enter,
        Exit
    }

    public interface INPNodeDebugListener
    {
        void OnNodeEvent(NP_RuntimeTree tree, Node node, NPDebugEventType evt, bool success);
    }
#endif
}
