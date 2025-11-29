//------------------------------------------------------------
// Purpose: Toolbar UI shared by graph editor windows.
// Author: Unknown contributor
// Mail: 1778139321@qq.com
// Date: 2021-05-31 19:15:32
//------------------------------------------------------------

using GraphProcessor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.NodeEditor
{
    public class UniversalToolbarView : ToolbarView
    {
        protected readonly MiniMap m_MiniMap;
        protected readonly BaseGraph m_BaseGraph;
        protected readonly BaseGraphView m_BaseGraphView;

        private readonly Texture2D m_CreateNewToggleIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            $"{UniversalGraphWindow.NodeGraphProcessorPathPrefix}/Editor/CreateNew.png");

        private readonly Texture2D m_MiniMapToggleIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            $"{UniversalGraphWindow.NodeGraphProcessorPathPrefix}/Editor/MiniMap.png");

        private readonly Texture2D m_ConditionalToggleIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            $"{UniversalGraphWindow.NodeGraphProcessorPathPrefix}/Editor/Run.png");

        private readonly Texture2D m_ExposedParamsToggleIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            $"{UniversalGraphWindow.NodeGraphProcessorPathPrefix}/Editor/Blackboard.png");

        private readonly Texture2D m_GotoFileButtonIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            $"{UniversalGraphWindow.NodeGraphProcessorPathPrefix}/Editor/GotoFile.png");

        private readonly Texture2D m_OneKeySetFileButtonIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            $"{UniversalGraphWindow.NodeGraphProcessorPathPrefix}/Editor/Icon_Key_Line0.png");
        
        
        
        public UniversalToolbarView(BaseGraphView graphView, MiniMap miniMap, BaseGraph baseGraph) : base(graphView)
        {
            m_MiniMap = miniMap;
            // Hide mini map by default to reduce lag when graphs are large.
            m_MiniMap.visible = false;

            m_BaseGraph = baseGraph;
            m_BaseGraphView = graphView;
        }

        protected override void AddButtons()
        {
            AddButton(new GUIContent("", m_CreateNewToggleIcon, "Create a new graph asset"), () =>
            {
                GenericMenu genericMenu = new GenericMenu();
                foreach (var graphType in TypeCache.GetTypesDerivedFrom<BaseGraph>())
                {
                    genericMenu.AddItem(new GUIContent($"{graphType.Name}"), false,
                        data =>
                        {
                            BaseGraph baseGraph = GraphCreateAndSaveHelper.CreateGraph(data as System.Type);
                            GraphAssetCallbacks.InitializeGraph(baseGraph);
                        }, graphType);
                }

                genericMenu.ShowAsContext();
            }, true);

            //AddSeparator(5);

            AddToggle(new GUIContent("", m_ExposedParamsToggleIcon, "Toggle parameter panel"),
                graphView.GetPinnedElementStatus<ExposedParameterView>() != DropdownMenuAction.Status.Hidden,
                (v) => graphView.ToggleView<ExposedParameterView>());

            //AddSeparator(5);

            AddToggle(new GUIContent("", m_MiniMapToggleIcon, "Toggle mini map"), m_MiniMap.visible,
                (v) => m_MiniMap.visible = v);

            //AddSeparator(5);

            // AddToggle(new GUIContent(m_ConditionalToggleIcon, "Toggle runtime panel"),
            //     graphView.GetPinnedElementStatus<ConditionalProcessorView>() !=
            //     DropdownMenuAction.Status.Hidden, (v) => graphView.ToggleView<ConditionalProcessorView>());

            AddButton(new GUIContent("", m_GotoFileButtonIcon, "Ping the graph asset"),
                () =>
                {
                    EditorGUIUtility.PingObject(graphView.graph);
                    Selection.activeObject = graphView.graph;
                });
            
            AddButton(new GUIContent("", m_OneKeySetFileButtonIcon, "One-click setup"),
                () =>
                {
                    EditorGUIUtility.PingObject(graphView.graph);
                    Selection.activeObject = graphView.graph;
                    graphView.graph.OneKeySet();
                });
            
            AddSeparator(5);

            AddCustom(() =>
            {
                GUI.color = new Color(128 / 255f, 128 / 255f, 128 / 255f);
                GUILayout.Label($"{m_BaseGraph.GetType().Name}-{m_BaseGraph.name}",
                    EditorGUIStyleHelper.GetGUIStyleByName(nameof(EditorStyles.toolbarButton)));
                GUI.color = Color.white;
            });
        }
    }
}