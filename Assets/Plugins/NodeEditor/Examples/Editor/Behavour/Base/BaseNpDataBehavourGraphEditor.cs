using UnityEditor;
using UnityEngine;

namespace Plugins.NodeEditor
{
    [CustomEditor(typeof(BaseNpDataBehavourGraph), true)]
    public class BaseNpDataBehavourGraphEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 1. Draw Script field (standard Unity practice)
            SerializedProperty scriptProp = serializedObject.FindProperty("m_Script");
            if (scriptProp != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(scriptProp);
                EditorGUI.EndDisabledGroup();
            }

            // 2. Draw _name as ReadOnly
            SerializedProperty nameProp = serializedObject.FindProperty("_name");
            if (nameProp != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(nameProp);
                EditorGUI.EndDisabledGroup();
            }

            // 3. Draw _configPath as ReadOnly
            SerializedProperty configPathProp = serializedObject.FindProperty("_configPath");
            if (configPathProp != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(configPathProp);
                EditorGUI.EndDisabledGroup();
            }

            // 4. Draw remaining properties
            DrawPropertiesExcluding(serializedObject, "m_Script", "_name", "_configPath");

            serializedObject.ApplyModifiedProperties();

            // 5. Graph action buttons (restore the original BaseGraphInspector buttons)
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Graph Actions", EditorStyles.boldLabel);

            var graph = (BaseNpDataBehavourGraph)target;
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
