// File: GraphCreateAndSaveHelper.cs
// Summary: Helper utilities to create and save graph assets via editor dialogs.
// Note: Adjust default paths if your project structure differs.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GraphProcessor
{
    public static class GraphCreateAndSaveHelper
    {
        public static BaseGraph CreateGraph(Type graphType)
        {
            BaseGraph baseGraph = ScriptableObject.CreateInstance(graphType) as BaseGraph;
            string panelPath = "Assets/Plugins/NodeEditor/Examples/Saves/";
            Directory.CreateDirectory(panelPath);
            string panelFileName = "Graph";
            string path = EditorUtility.SaveFilePanelInProject("Save Graph Asset", panelFileName, "asset", "", panelPath);
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("Create graph canceled.");
                return null;
            }
            AssetDatabase.CreateAsset(baseGraph, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return baseGraph;
        }
        
        public static void SaveGraphToDisk(BaseGraph baseGraphToSave)
        {
            EditorUtility.SetDirty(baseGraphToSave);
            AssetDatabase.SaveAssets();
        }
    }
}
