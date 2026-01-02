using System.Collections.Generic;
using Ebonor.Framework;
using UnityEditor;

namespace Plugins.NodeEditor
{
    public class NPBehaveGraphView : UniversalGraphView
    {
        
        static readonly ILog log = LogManager.GetLogger(typeof(NPBehaveGraphView));
        
        private List<List<NP_NodeView>> m_HashTree = new List<List<NP_NodeView>>();
        public NPBehaveGraphWindow NpBehaveGraphWindow;

        private NP_NodeView m_RootNodeView
        {
            get { return (NP_NodeView) this.nodeViews.Find(x => x.nodeTarget.name == "行为树根节点"); }
        }

        public NPBehaveGraphView(EditorWindow window) : base(window)
        {
            NpBehaveGraphWindow = window as NPBehaveGraphWindow;
        }

        /// <summary>
        /// 自动排序布局
        /// </summary>
        public void AutoSortLayout()
        {
            var rootNodeView = m_RootNodeView;
            if (rootNodeView == null)
            {
                return;
            }

            // 先计算节点之间的联系（配置父节点和子节点）
            CalculateNodeRelationShip(rootNodeView);
            NodeAutoLayouter.Layout(new NPBehaveNodeConvertor().Init(rootNodeView));
        }

        private void CalculateNodeRelationShip(NP_NodeView rootNodeView)
        {
            rootNodeView.Parent = null;
            rootNodeView.Children.Clear();
            var outputPort = rootNodeView.outputPortViews;
            var inputPort = rootNodeView.inputPortViews;

            if (inputPort.Count > 0)
            {
                var inputEdges = inputPort[0].GetEdges();
                if (inputEdges.Count > 0)
                {
                    rootNodeView.Parent = inputEdges[0].output.node as NP_NodeView;
                }
                else
                {
                    log.Error("当前行为树配置有误，请检查是否有节点未正确链接");
                }
            }

            if (outputPort.Count > 0)
            {
                var outputEdges = outputPort[0].GetEdges();
                if (outputEdges.Count > 0)
                {
                    foreach (var outputEdge in outputEdges)
                    {
                        var childNodeView = outputEdge.input.node as NP_NodeView;
                        if(null == childNodeView)
                            continue;
                        CalculateNodeRelationShip(childNodeView);
                        rootNodeView.Children.Add(childNodeView);
                    }

                    // 根据x坐标进行排序
                    rootNodeView.Children.Sort((x, y) => x.GetPosition().x.CompareTo(y.GetPosition().x));
                }
                else
                {
                    log.Error("当前行为树配置有误，请检查是否有节点未正确链接");
                }
            }
        }
    }
}