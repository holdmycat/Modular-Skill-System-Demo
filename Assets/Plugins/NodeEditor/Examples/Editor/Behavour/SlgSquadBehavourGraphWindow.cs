using GraphProcessor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Plugins.NodeEditor
{
    public class SlgSquadBehavourGraphWindow: UniversalGraphWindow
    {
        protected override void InitializeWindow(BaseGraph graph)
        {
            graphView = new NPBehaveGraphView(this);

            m_MiniMap = new MiniMap() { anchored = true };
            graphView.Add(m_MiniMap);

            m_ToolbarView = new NpcBehaviourToolbarView(graphView, m_MiniMap, graph);
            graphView.Add(m_ToolbarView);

            titleContent = new GUIContent(graph.name,
                AssetDatabase.LoadAssetAtPath<Texture2D>($"{NodeGraphProcessorPathPrefix}/Editor/Icon_Dark.png"));
            
            SetCurrentBlackBoardDataManager();
        }

        private void OnFocus()
        {
            //SetCurrentBlackBoardDataManager();
        }

        private void SetCurrentBlackBoardDataManager()
        {
            // NpcBehaviourGraph npBehaveGraph = (this.graph as NpcBehaviourGraph);
            // if (npBehaveGraph == null)
            // {
            //     //因为OnFocus执行时机比较诡异，在OnEnable后，或者执行一些操作后都会执行，但这时Graph可能为空，所以做判断
            //     return;
            // }
            // NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager = (this.graph as NpcBehaviourGraph)?.NpBlackBoardDataManager;
        }
    }
}