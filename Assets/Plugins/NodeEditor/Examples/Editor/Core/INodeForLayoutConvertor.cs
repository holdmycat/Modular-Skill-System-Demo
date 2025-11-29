namespace Plugins.NodeEditor
{
    public interface INodeForLayoutConvertor
    {
        /// <summary>
        /// Distance between sibling nodes.
        /// </summary>
        float SiblingDistance { get; }

        object PrimRootNode { get; }
        NodeAutoLayouter.TreeNode LayoutRootNode { get; }

        INodeForLayoutConvertor Init(object primRootNode);
        NodeAutoLayouter.TreeNode PrimNode2LayoutNode();
        void LayoutNode2PrimNode();
    }
}
