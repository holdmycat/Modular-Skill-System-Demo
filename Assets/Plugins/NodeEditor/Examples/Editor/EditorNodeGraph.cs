using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

namespace Plugins.NodeEditor
{
#if UNITY_EDITOR
    using UnityEditor;
    public class EditorNodeGraph : EditorWindow
    {
        
        private const string NodeEditorPath = "Assets/Plugins/NodeEditor/Examples/Saves/";
        
        [MenuItem("Custom Windows/Behavior Trees/Compile All Trees")]
        public static void ExecuteOneKeySetOnBaseGraphAssets()
        {
            // Get all asset guids in the specified path
            string[] assetGuids = AssetDatabase.FindAssets("t:BaseGraph", new[] { EditorNodeGraph.NodeEditorPath });
        
            List<BaseGraph> baseGraphAssets = new List<BaseGraph>();
        
            // Iterate through all asset guids and load the assets
            foreach (string guid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var baseGraph = AssetDatabase.LoadAssetAtPath<BaseGraph>(assetPath);
                if (baseGraph != null)
                {
                    baseGraphAssets.Add(baseGraph);
                }
            }
        
            // Execute OneKeySet on each BaseGraph asset
            foreach (BaseGraph baseGraph in baseGraphAssets)
            {
                Debug.LogFormat("Compiling {0} Begin", baseGraph.name);
                baseGraph.OneKeySet();
                Debug.LogFormat("Compiling {0} Over", baseGraph.name);
            }
        }
       
        
        private const string NodeSkillAttrEditorPath = "Assets/Plugins/NodeEditor/Examples/Saves/SkillAttributesGraphs/";
        private const string NodeActiveSkillEditorPath = "Assets/Plugins/NodeEditor/Examples/Saves/ActiveSkillGraphs/";
        private const string NodeSupportSkillEditorPath = "Assets/Plugins/NodeEditor/Examples/Saves/SupportSkillGraphs/";
        [MenuItem("Custom Windows/Behavior Trees/Compile All Skill Trees")]
        public static void ExecuteSkillOneKeySetOnBaseGraphAssets()
        {
            // skill attr
            string[] assetGuids = AssetDatabase.FindAssets("t:BaseGraph", new[] { EditorNodeGraph.NodeSkillAttrEditorPath });
            ExecuteTheGraphAssets(assetGuids);
            
            //active skill attr
            assetGuids = AssetDatabase.FindAssets("t:BaseGraph", new[] { EditorNodeGraph.NodeActiveSkillEditorPath });
            ExecuteTheGraphAssets(assetGuids);

            //support skill attr
            assetGuids = AssetDatabase.FindAssets("t:BaseGraph", new[] { EditorNodeGraph.NodeSupportSkillEditorPath });
            ExecuteTheGraphAssets(assetGuids);
        }
        
        [MenuItem("Custom Windows/Behavior Trees/Compile All Active Skill Trees")]
        public static void ExecuteActiveSkillOneKeySetOnBaseGraphAssets()
        {
            // skill attr
            string[] assetGuids = AssetDatabase.FindAssets("t:BaseGraph", new[] { EditorNodeGraph.NodeSkillAttrEditorPath });
            ExecuteTheGraphAssets(assetGuids);
            
            //active skill attr
            assetGuids = AssetDatabase.FindAssets("t:BaseGraph", new[] { EditorNodeGraph.NodeActiveSkillEditorPath });
            ExecuteTheGraphAssets(assetGuids);

            // //support skill attr
            // assetGuids = AssetDatabase.FindAssets("t:BaseGraph", new[] { EditorNodeGraph.NodeSupportSkillEditorPath });
            // ExecuteTheGraphAssets(assetGuids);
        }
        
        [MenuItem("Custom Windows/Behavior Trees/Compile All Support Skill Trees")]
        public static void ExecuteSupportSkillOneKeySetOnBaseGraphAssets()
        {
            // skill attr
            string[] assetGuids = AssetDatabase.FindAssets("t:BaseGraph", new[] { EditorNodeGraph.NodeSkillAttrEditorPath });
            ExecuteTheGraphAssets(assetGuids);
            
            // //active skill attr
            // assetGuids = AssetDatabase.FindAssets("t:BaseGraph", new[] { EditorNodeGraph.NodeActiveSkillEditorPath });
            // ExecuteTheGraphAssets(assetGuids);

            //support skill attr
            assetGuids = AssetDatabase.FindAssets("t:BaseGraph", new[] { EditorNodeGraph.NodeSupportSkillEditorPath });
            ExecuteTheGraphAssets(assetGuids);
        }



        private static void ExecuteTheGraphAssets(string[] assetGuids)
        {
            List<BaseGraph> baseGraphAssets = new List<BaseGraph>();
        
            // Iterate through all asset guids and load the assets
            foreach (string guid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var baseGraph = AssetDatabase.LoadAssetAtPath<BaseGraph>(assetPath);
                if (baseGraph != null)
                {
                    baseGraphAssets.Add(baseGraph);
                }
            }
        
            // Execute OneKeySet on each BaseGraph asset
            foreach (BaseGraph baseGraph in baseGraphAssets)
            {
                Debug.LogFormat("Compiling {0} Begin", baseGraph.name);
                baseGraph.OneKeySet();
                Debug.LogFormat("Compiling {0} Over", baseGraph.name);
            }
        }
    }
    
#endif
   
}
