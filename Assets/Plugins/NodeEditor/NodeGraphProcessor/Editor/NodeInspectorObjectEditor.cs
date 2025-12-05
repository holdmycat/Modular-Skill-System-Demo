using UnityEditor;
using UnityEngine;

namespace GraphProcessor
{
    [CustomEditor(typeof(NodeInspectorObject))]
    public class NodeInspectorObjectEditor : UnityEditor.Editor
    {
        SerializedProperty selectedNodesProp;
        SerializedProperty currentDataProp;

        void OnEnable()
        {
            selectedNodesProp = serializedObject.FindProperty("selectedNodeData");
            currentDataProp = serializedObject.FindProperty("currentData");
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject == null)
            {
                EditorGUILayout.HelpBox("No inspector data.", MessageType.Info);
                return;
            }

            serializedObject.Update();
            try
            {
                selectedNodesProp ??= serializedObject.FindProperty("selectedNodeData");
                currentDataProp ??= serializedObject.FindProperty("currentData");

                if (selectedNodesProp != null)
                    EditorGUILayout.PropertyField(selectedNodesProp, new GUIContent("Selected Nodes"), true);

                EditorGUILayout.Space();

                if (currentDataProp != null)
                    EditorGUILayout.PropertyField(currentDataProp, new GUIContent("Current Data"), true);
            }
            catch (System.Exception ex)
            {
                EditorGUILayout.HelpBox($"Failed to draw inspector: {ex.Message}", MessageType.Error);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
