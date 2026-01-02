//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2021年5月31日 19:15:32
//------------------------------------------------------------


using Ebonor.DataCtrl;
using GraphProcessor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Plugins.NodeEditor
{
    public class NPBehaveToolbarView: UniversalToolbarView
    {
        public class BlackboardInspectorViewer: ScriptableObject
        {
            public NP_BlackBoardDataManager NpBlackBoardDataManager;
        }

        private static BlackboardInspectorViewer _BlackboardInspectorViewer;

        public static BlackboardInspectorViewer BlackboardInspector
        {
            get
            {
                if (_BlackboardInspectorViewer == null)
                {
                    _BlackboardInspectorViewer = ScriptableObject.CreateInstance<BlackboardInspectorViewer>();
                }

                return _BlackboardInspectorViewer;
            }
        }

        public NPBehaveToolbarView(BaseGraphView graphView, MiniMap miniMap, BaseGraph baseGraph): base(graphView,
            miniMap, baseGraph)
        {
            
        }

        protected override void AddButtons()
        {
            base.AddButtons();
            
            AddButton(new GUIContent("AutoLayout", "自动优化布局"),
                () =>
                {
                    (this.graphView as NPBehaveGraphView).AutoSortLayout();
                }, false);
        
            AddButton(new GUIContent("Blackboard", "打开Blackboard数据面板"),
                () =>
                {
                    NPBehaveToolbarView.BlackboardInspector.NpBlackBoardDataManager =
                            (this.m_BaseGraph as BaseNpDataBehavourGraph).NpBlackBoardDataManager;
                    Selection.activeObject = BlackboardInspector;
                }, false);
            
        }
    }
}