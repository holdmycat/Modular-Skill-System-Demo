using UnityEngine;
using UnityEditor;
using System.IO;
namespace Ebonor.Manager
{
    public class ActionNodeGeneratorWindow : EditorWindow
    {
        // Enums from Design
        public enum ActionNamespace { GamePlay, DataCtrl }
        public enum OverrideType { GetActionToBeDone, GetFuncToBeDone, GetFunc1ToBeDone, GetFunc2ToBeDone }
        public enum TreeType { SlgSquad }
        public enum NodeType { Task }
        public enum ActionCategory { NpBehave, System, Transform, Camera, Audio, Collider, Time, Buff }

        // Input Fields
        private string _baseName = "NewAction";
        private string _description = "New Action Description";
        private ActionNamespace _namespace = ActionNamespace.GamePlay;
        private OverrideType _overrideType = OverrideType.GetActionToBeDone;
        private TreeType _treeType = TreeType.SlgSquad;
        private NodeType _nodeType = NodeType.Task;
        private ActionCategory _category = ActionCategory.System;

        [MenuItem("Tools/Action Node Generator")]
        public static void ShowWindow()
        {
            GetWindow<ActionNodeGeneratorWindow>("Action Node Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Action Node Generator", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            GUILayout.Label("Basic Information", EditorStyles.boldLabel);
            _baseName = EditorGUILayout.TextField("Base Name", _baseName);
            _description = EditorGUILayout.TextField("Description", _description);

            EditorGUILayout.Space();
            GUILayout.Label("Configuration", EditorStyles.boldLabel);
            _namespace = (ActionNamespace)EditorGUILayout.EnumPopup("Namespace", _namespace);
            _overrideType = (OverrideType)EditorGUILayout.EnumPopup("Override Method", _overrideType);

            EditorGUILayout.Space();
            GUILayout.Label("Menu Hierarchy", EditorStyles.boldLabel);
            _treeType = (TreeType)EditorGUILayout.EnumPopup("Tree Type", _treeType);
            _nodeType = (NodeType)EditorGUILayout.EnumPopup("Node Type", _nodeType);
            _category = (ActionCategory)EditorGUILayout.EnumPopup("Category", _category);

            EditorGUILayout.Space();
            if (GUILayout.Button("Generate Scripts"))
            {
                GenerateScripts();
            }
        }

        private void GenerateScripts()
        {
            if (string.IsNullOrEmpty(_baseName))
            {
                EditorUtility.DisplayDialog("Error", "Base Name cannot be empty.", "OK");
                return;
            }
            
            // 1. Generate Action Script
            GenerateActionScript();

            // 2. Generate Node Script
            GenerateNodeScript();

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", $"Action and Node scripts for '{_baseName}' generated successfully!", "OK");
        }

        private void GenerateActionScript()
        {
            string className = $"NP_{_baseName}Action";
            string folderPath = GetActionFolderPath();
            string filePath = Path.Combine(folderPath, $"{className}.cs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string template = GetActionTemplate();
            string content = template
                .Replace("CLASS_NAME", className)
                .Replace("NAMESPACE", _namespace == ActionNamespace.GamePlay ? "GamePlay" : "DataCtrl")
                .Replace("NAMESPACE_IMPORT", _namespace == ActionNamespace.GamePlay ? "using Ebonor.GamePlay;" : "") // Adjusted import logic
                .Replace("OVERRIDE_METHOD", GetOverrideMethodCode(_overrideType));

            File.WriteAllText(filePath, content);
            Debug.Log($"Generated Action Script: {filePath}");
        }

        private void GenerateNodeScript()
        {
            string actionClassName = $"NP_{_baseName}Action";
            string nodeClassName = $"NP_{_baseName}ActionNode";
            string folderPath = GetNodeFolderPath();
            string filePath = Path.Combine(folderPath, $"{nodeClassName}.cs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string graphType = "SlgSquadBehavourGraph"; // Hardcoded for now based on context, can be mapped from TreeType if needed
            string menuPath = $"{_treeType}/{_nodeType}/{_category}/{_description}";
            
            // Map TreeType to GraphType if needed. For now assuming SlgSquad -> SlgSquadBehavourGraph
            if (_treeType == TreeType.SlgSquad) graphType = "SlgSquadBehavourGraph";

            string template = GetNodeTemplate();
            string content = template
                .Replace("CLASS_NAME_NODE", nodeClassName)
                .Replace("CLASS_NAME", actionClassName)
                .Replace("MENU_PATH", menuPath)
                .Replace("GRAPH_TYPE", graphType)
                .Replace("NODE_DESC", _description)
                .Replace("NAMESPACE_IMPORT", _namespace == ActionNamespace.GamePlay ? "using Ebonor.GamePlay;" : "using Ebonor.DataCtrl;");

            File.WriteAllText(filePath, content);
            Debug.Log($"Generated Node Script: {filePath}");
        }

        private string GetActionFolderPath()
        {
            string root = Application.dataPath;
            if (_namespace == ActionNamespace.GamePlay)
            {
                return Path.Combine(root, "Scripts/GamePlay/TheDataContainsAction", _category.ToString());
            }
            else
            {
                return Path.Combine(root, "Scripts/DataCtrl/NPData/TheDataContainsAction", _category.ToString());
            }
        }

        private string GetNodeFolderPath()
        {
            string root = Application.dataPath;
            // Assuming strict structure: Assets/Plugins/NodeEditor/Examples/Editor/Nodes/Actions/{Category}/
            // Wait, prompt said: Assets/Plugins/NodeEditor/Examples/Editor/Nodes/Actions/
            // But also "菜单路径分了3个层级...这三个分别对应3类枚举类型"
            // Let's stick to the prompt's implied structure or similar to existing.
            // Existing example: Assets/Plugins/NodeEditor/Examples/Editor/Nodes/Skill/Task/System/NP_PrintDebugLogNode.cs
            // Let's try to match existing as much as possible, or use a simplified new path.
            // Prompt: "node统一放到Assets/Plugins/NodeEditor/Examples/Editor/Nodes/Actions/，菜单路径分了3个层级..."
            // Let's use: Assets/Plugins/NodeEditor/Examples/Editor/Nodes/Actions/{Category}
            return Path.Combine(root, "Plugins/NodeEditor/Examples/Editor/Nodes/Actions", _category.ToString());
        }

        private string GetOverrideMethodCode(OverrideType type)
        {
            switch (type)
            {
                case OverrideType.GetActionToBeDone:
                    return @"public override System.Action GetActionToBeDone()
        {
            this.Action = () => 
            {
                // TODO: Implement Action logic here
            };
            return this.Action;
        }";
                case OverrideType.GetFuncToBeDone:
                    return @"public override System.Func<bool> GetFuncToBeDone()
        {
            this.Func = () => 
            {
                // TODO: Implement Func logic here
                return true;
            };
            return this.Func;
        }";
                case OverrideType.GetFunc1ToBeDone:
                    return @"public override System.Func<bool> GetFunc1ToBeDone()
        {
            this.Func = () => 
            {
                // TODO: Implement Func1 logic here
                return true;
            };
            return this.Func;
        }";
                case OverrideType.GetFunc2ToBeDone:
                     return @"public override System.Func<Ebonor.DataCtrl.Action.Request, Ebonor.DataCtrl.Action.Result> GetFunc2ToBeDone()
        {
            this.Func2 = (request) => 
            {
                // TODO: Implement Func2 logic here
                return Ebonor.DataCtrl.Action.Result.Success;
            };
            return this.Func2;
        }";
                default:
                    return "";
            }
        }

        private string GetActionTemplate()
        {
            return @"using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
NAMESPACE_IMPORT

namespace Ebonor.NAMESPACE
{
    [System.Serializable]
    public class CLASS_NAME : NP_ClassForStoreAction
    {
        // OVERRIDE_METHOD
    }
}";
        }

        private string GetNodeTemplate()
        {
            return @"using GraphProcessor;
using Ebonor.DataCtrl;
using Ebonor.GamePlay;
NAMESPACE_IMPORT

namespace Plugins.NodeEditor
{
    [NodeMenuItem(""MENU_PATH"", typeof(GRAPH_TYPE))]
    public class CLASS_NAME_NODE : NP_TaskNodeBase
    {
        public override string name => ""NODE_DESC"";
        
        public NP_ActionNodeData NP_ActionNodeData =
            new NP_ActionNodeData()
            {
                NpClassForStoreAction = new CLASS_NAME(),
                NodeDes = ""NODE_DESC""
            };
        
        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ActionNodeData;
        }
    }
}";
        }
    }
}
