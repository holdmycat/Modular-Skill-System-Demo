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
            serializedObject.Update();

            var inspector = (NodeInspectorObject)target;
            if (inspector.selectedNodeData == null || inspector.selectedNodeData.Count == 0)
            {
                EditorGUILayout.HelpBox("在图中选中结点即可在此查看和编辑结点数据。", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (selectedNodesProp != null)
                EditorGUILayout.PropertyField(selectedNodesProp, new GUIContent("Selected Nodes"), true);

            if (currentDataProp != null)
                EditorGUILayout.PropertyField(currentDataProp, new GUIContent("Current Data"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
