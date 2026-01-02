
using GraphProcessor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Plugins.NodeEditor
{
    public class NPBehaveGraphWindow : UniversalGraphWindow
    {
        protected override void InitializeWindow(BaseGraph graph)
        {
            graphView = new NPBehaveGraphView(this);

            m_MiniMap = new MiniMap() {anchored = true};
            graphView.Add(m_MiniMap);
        
            m_ToolbarView = new NPBehaveToolbarView(graphView, m_MiniMap, graph);
            graphView.Add(m_ToolbarView);
            
            titleContent = new GUIContent(graph.name,
                AssetDatabase.LoadAssetAtPath<Texture2D>($"{NodeGraphProcessorPathPrefix}/Editor/Icon_Dark.png"));
            
            //NP_BlackBoardHelper.SetCurrentBlackBoardDataManager(this.graph as NPBehaveGraph);
        }
        
        private void OnFocus()
        {
            //NP_BlackBoardHelper.SetCurrentBlackBoardDataManager(this.graph as NPBehaveGraph);
        }
    }
}
