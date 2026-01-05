using System;
using System.IO;
using Ebonor.DataCtrl;
using Ebonor.GamePlay;
using GraphProcessor;
using UnityEditor;
using UnityEngine;

namespace Plugins.NodeEditor
{
    /// <summary>
    /// Simple editor utility to generate paired NP action/node scripts.
    /// </summary>
    public class ActionNodeGeneratorWindow : EditorWindow
    {
        private const string ActionFolderGamePlay = "Assets/Scripts/GamePlay/TheDataContainsAction/Actions";
        private const string ActionFolderDataCtrl = "Assets/Scripts/DataCtrl/NPData/TheDataContainsAction/Actions";
        private const string NodeFolder = "Assets/Plugins/NodeEditor/Examples/Editor/Nodes/Actions";

        [Serializable]
        private class MenuDescriptor
        {
            public TreeType Tree;
            public BehaviorCategory Behavior;
            public ActionCategory Action;
            public MenuDescriptor(TreeType tree, BehaviorCategory behavior, ActionCategory action)
            {
                Tree = tree;
                Behavior = behavior;
                Action = action;
            }
            public override string ToString() => $"{Tree}/{Behavior}/{Action}";
        }

        private enum TreeType 
        { 
            [InspectorName("SLG 小队")]
            SlgSquad 
        }
        
        private enum BehaviorCategory 
        { 
            [InspectorName("任务")]
            Task 
        }
        
        private enum ActionCategory 
        { 
            [InspectorName("NpBehave 核心")]
            NpBehave, 
            [InspectorName("系统")]
            System, 
            [InspectorName("变换 (Transform)")]
            Transform, 
            [InspectorName("相机")]
            Camera, 
            [InspectorName("音频")]
            Audio, 
            [InspectorName("碰撞")]
            Collider, 
            [InspectorName("时间")]
            Time, 
            [InspectorName("Buff")]
            Buff, 
            [InspectorName("Squad 状态机")]
            SquadFsm 
        }

        private enum ActionNamespace
        {
            GamePlay,
            DataCtrl
        }

        private string _baseName = "PrintDebugLog";
        private string _nodeDisplayName = "打印日志";
        private readonly System.Collections.Generic.List<MenuDescriptor> _menus =
            new System.Collections.Generic.List<MenuDescriptor>
            {
                new MenuDescriptor(TreeType.SlgSquad, BehaviorCategory.Task, ActionCategory.System)
            };
        private ActionNamespace _namespaceSelection = ActionNamespace.GamePlay;

        private bool _overrideAction = true;
        private bool _overrideFunc;
        private bool _overrideFunc1;
        private bool _overrideFunc2;
        private bool _overrideInterval;

        [MenuItem("Custom Windows/NP Action & Node Generator")]
        public static void Open()
        {
#if UNITY_2020_1_OR_NEWER
            // Dock next to the Scene view by default to avoid floating popup.
            var window = GetWindow<ActionNodeGeneratorWindow>(null, true, new[] { typeof(SceneView) });
#else
            var window = GetWindow<ActionNodeGeneratorWindow>();
#endif
            window.titleContent = new GUIContent("NP Action/Node Generator");
            window.minSize = new Vector2(420, 260);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("生成 NP Action + Node", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _baseName = EditorGUILayout.TextField("脚本名称 (不含前缀)", _baseName);
            _nodeDisplayName = EditorGUILayout.TextField("节点显示名", _nodeDisplayName);
            EditorGUILayout.LabelField("节点菜单（可多选）", EditorStyles.boldLabel);
            for (int i = 0; i < _menus.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                var menu = _menus[i];
                menu.Tree = (TreeType)EditorGUILayout.EnumPopup("树类型", menu.Tree);
                menu.Behavior = (BehaviorCategory)EditorGUILayout.EnumPopup("NPData 节点类型", menu.Behavior);
                menu.Action = (ActionCategory)EditorGUILayout.EnumPopup("Action 分类", menu.Action);
                if (GUILayout.Button("移除此菜单"))
                {
                    _menus.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndVertical();
            }
            if (GUILayout.Button("添加一个菜单条目"))
            {
                _menus.Add(new MenuDescriptor(TreeType.SlgSquad, BehaviorCategory.Task, ActionCategory.System));
            }
            
            _namespaceSelection = (ActionNamespace)EditorGUILayout.EnumPopup("归属命名空间", _namespaceSelection);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("选择需要 override 的方法：", EditorStyles.boldLabel);
            _overrideAction = EditorGUILayout.ToggleLeft("GetActionToBeDone (Action)", _overrideAction);
            _overrideFunc = EditorGUILayout.ToggleLeft("GetFuncToBeDone (Func<bool>)", _overrideFunc);
            _overrideFunc1 = EditorGUILayout.ToggleLeft("GetFunc1ToBeDone (Func<bool>)", _overrideFunc1);
            _overrideFunc2 = EditorGUILayout.ToggleLeft("GetFunc2ToBeDone (Func<Request, Result>)", _overrideFunc2);
            _overrideInterval = EditorGUILayout.ToggleLeft("GetInternal (float)", _overrideInterval);

            EditorGUILayout.Space();
            var targetActionFolder = _namespaceSelection == ActionNamespace.GamePlay
                ? ActionFolderGamePlay
                : ActionFolderDataCtrl;
            EditorGUILayout.HelpBox("Action 将生成到:\n" + targetActionFolder +
                                    "\nNode 将生成到:\n" + NodeFolder, MessageType.Info);

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(_baseName)))
            {
                if (GUILayout.Button("生成 Action 和 Node 脚本"))
                {
                    Generate();
                }
            }
        }

        private void Generate()
        {
            var actionFolder = _namespaceSelection == ActionNamespace.GamePlay
                ? ActionFolderGamePlay
                : ActionFolderDataCtrl;

            Directory.CreateDirectory(actionFolder);
            Directory.CreateDirectory(NodeFolder);

            string actionClassName = $"NP_{_baseName}Action";
            string nodeClassName = $"NP_{_baseName}ActionNode";

            string actionPath = Path.Combine(actionFolder, $"{actionClassName}.cs");
            string nodePath = Path.Combine(NodeFolder, $"{nodeClassName}.cs");

            if (File.Exists(actionPath) || File.Exists(nodePath))
            {
                if (!EditorUtility.DisplayDialog("文件已存在",
                        "目标 Action 或 Node 脚本已存在，是否覆盖？", "覆盖", "取消"))
                {
                    return;
                }
            }

            File.WriteAllText(actionPath, BuildActionContent(actionClassName));
            File.WriteAllText(nodePath, BuildNodeContent(nodeClassName, actionClassName));

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("生成成功", $"已生成:\n{actionPath}\n{nodePath}", "确定");
        }

        private string BuildActionContent(string actionClassName)
        {
            var writer = new System.Text.StringBuilder();
            writer.AppendLine("using System;");
            writer.AppendLine("using Ebonor.DataCtrl;");
            writer.AppendLine("using Ebonor.Framework;");
            writer.AppendLine();
            string ns = _namespaceSelection == ActionNamespace.GamePlay ? "Ebonor.GamePlay" : "Ebonor.DataCtrl";
            writer.AppendLine($"namespace {ns}");
            writer.AppendLine("{");
            writer.AppendLine("    /// <summary>");
            writer.AppendLine($"    /// Auto-generated action for menus:");
            foreach (var menu in _menus)
                writer.AppendLine($"    /// - {menu}/{_baseName}");
            writer.AppendLine("    /// </summary>");
            writer.AppendLine($"    [Serializable]");
            writer.AppendLine($"    public class {actionClassName} : NP_ClassForStoreAction");
            writer.AppendLine("    {");
            writer.AppendLine("        static readonly ILog log = LogManager.GetLogger(typeof(" + actionClassName + "));");
            writer.AppendLine();

            if (_overrideAction)
            {
                writer.AppendLine("        public override Action GetActionToBeDone()");
                writer.AppendLine("        {");
                writer.AppendLine("            Action = OnAction;"); 
                writer.AppendLine("            return Action;");
                writer.AppendLine("        }");
                writer.AppendLine();
                writer.AppendLine("        private void OnAction()");
                writer.AppendLine("        {");
                writer.AppendLine("            // TODO: implement action logic");
                writer.AppendLine("        }");
                writer.AppendLine();
            }

            if (_overrideFunc)
            {
                writer.AppendLine("        public override Func<bool> GetFuncToBeDone()");
                writer.AppendLine("        {");
                writer.AppendLine("            Func = OnFunc;");
                writer.AppendLine("            return Func;");
                writer.AppendLine("        }");
                writer.AppendLine();
                writer.AppendLine("        private bool OnFunc()");
                writer.AppendLine("        {");
                writer.AppendLine("            // TODO: return true when succeeded");
                writer.AppendLine("            return true;");
                writer.AppendLine("        }");
                writer.AppendLine();
            }

            if (_overrideFunc1)
            {
                writer.AppendLine("        public override Func<bool> GetFunc1ToBeDone()");
                writer.AppendLine("        {");
                writer.AppendLine("            Func1 = OnFunc1;");
                writer.AppendLine("            return Func1;");
                writer.AppendLine("        }");
                writer.AppendLine();
                writer.AppendLine("        private bool OnFunc1()");
                writer.AppendLine("        {");
                writer.AppendLine("            // TODO: return true when succeeded");
                writer.AppendLine("            return true;");
                writer.AppendLine("        }");
                writer.AppendLine();
            }

            if (_overrideFunc2)
            {
                writer.AppendLine("        public override Func<Action.Request, Action.Result> GetFunc2ToBeDone()");
                writer.AppendLine("        {");
                writer.AppendLine("            Func2 = OnFunc2;");
                writer.AppendLine("            return Func2;");
                writer.AppendLine("        }");
                writer.AppendLine();
                writer.AppendLine("        private Action.Result OnFunc2(Action.Request request)");
                writer.AppendLine("        {");
                writer.AppendLine("            // TODO: construct and return Action.Result");
                writer.AppendLine("            return new Action.Result(true);");
                writer.AppendLine("        }");
                writer.AppendLine();
            }

            if (_overrideInterval)
            {
                writer.AppendLine("        public override float GetInternal()");
                writer.AppendLine("        {");
                writer.AppendLine("            // TODO: return interval seconds if needed");
                writer.AppendLine("            return 0f;");
                writer.AppendLine("        }");
                writer.AppendLine();
            }

            writer.AppendLine("    }");
            writer.AppendLine("}");
            return writer.ToString();
        }

        private string BuildNodeContent(string nodeClassName, string actionClassName)
        {
            var writer = new System.Text.StringBuilder();
            writer.AppendLine("using GraphProcessor;");
            writer.AppendLine("using Ebonor.DataCtrl;");
            writer.AppendLine("using Ebonor.GamePlay;");
            writer.AppendLine();
            writer.AppendLine("namespace Plugins.NodeEditor");
            writer.AppendLine("{");
            foreach (var menu in _menus)
            {
                string menuItem = $"{menu}/{_nodeDisplayName}";
                writer.AppendLine(
                    $"    [NodeMenuItem(\"{menuItem}\", typeof(SlgSquadBehavourGraph))]");
            }
            writer.AppendLine($"    public class {nodeClassName} : NP_TaskNodeBase");
            writer.AppendLine("    {");
            writer.AppendLine($"        public override string name => \"{_nodeDisplayName}\";");
            writer.AppendLine();
            writer.AppendLine("        public NP_ActionNodeData NP_ActionNodeData = new NP_ActionNodeData");
            writer.AppendLine("        {");
            writer.AppendLine($"            NpClassForStoreAction = new {actionClassName}(),");
            writer.AppendLine($"            NodeDes = \"{_nodeDisplayName}\"");
            writer.AppendLine("        };");
            writer.AppendLine();
            writer.AppendLine("        public override NP_NodeDataBase NP_GetNodeData()");
            writer.AppendLine("        {");
            writer.AppendLine("            return NP_ActionNodeData;");
            writer.AppendLine("        }");
            writer.AppendLine("    }");
            writer.AppendLine("}");
            return writer.ToString();
        }
    }
}
