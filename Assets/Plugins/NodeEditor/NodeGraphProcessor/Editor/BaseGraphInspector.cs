using UnityEditor;
using UnityEngine;

namespace GraphProcessor
{
    // File: BaseGraphInspector.cs
    // Summary: Replacement inspector to surface common graph actions without Odin.
    [CustomEditor(typeof(BaseGraph), true)]
    public class BaseGraphInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var graph = (BaseGraph)target;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Graph Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Auto configure all nodes"))
            {
                graph.BtnAutoSetCanvasDatas();
            }

            if (GUILayout.Button("Save graph binary"))
            {
                graph.Save();
            }

            if (GUILayout.Button("Test graph deserialization"))
            {
                graph.TestDe();
            }

            if (GUILayout.Button("One-click setup"))
            {
                graph.OneKeySet();
            }
        }
    }
}
