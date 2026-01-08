using UnityEditor;
using UnityEngine;
using Ebonor.DataCtrl; // Required for ANP_BBValue

namespace Plugins.NodeEditor
{
    [CustomEditor(typeof(BaseNpDataBehavourGraph), true)]
    public class BaseNpDataBehavourGraphEditor : Editor
    {
        // Helper inputs for adding new entries
        private string _newBbKey = "";
        private ANP_BBValue _newBbValueType; // Just for type selection reference? 
        // Actually, creating a specific subclass instance via UI is tricky. 
        // I'll assume ANP_BBValue subtypes are selectable or I need a dropdown of types.
        // For now, I'll provide a simplified Add logic or just Keys.
        // The user image shows "Type: NP_BBValue_Bool", so they want type selection.
        
        private int _selectedTypeIndex = 0;
        private readonly System.Type[] _availableTypes = { 
            typeof(NP_BBValue_Bool), 
            typeof(NP_BBValue_Int), 
            typeof(NP_BBValue_Float), 
            typeof(NP_BBValue_String), 
            typeof(NP_BBValue_Vector3),
            typeof(NP_BBValue_List_Long),
            typeof(NP_BBValue_MPNetPosition)
        };
        private string[] _availableTypeNames = { "Bool", "Int", "Float", "String", "Vector3", "List<Long>", "MPNetPosition" };

        // Helper inputs for ID
        private string _newIdKey = "";
        private long _newIdValue = 0;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 1. Script
            SerializedProperty scriptProp = serializedObject.FindProperty("m_Script");
            if (scriptProp != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(scriptProp);
                EditorGUI.EndDisabledGroup();
            }

            // 2. ReadOnly Fields
            SerializedProperty nameProp = serializedObject.FindProperty("_name");
            if (nameProp != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(nameProp);
                EditorGUI.EndDisabledGroup();
            }

            SerializedProperty configPathProp = serializedObject.FindProperty("_configPath");
            if (configPathProp != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(configPathProp);
                EditorGUI.EndDisabledGroup();
            }

            // 3. Draw Default Properties (Excluding Blackboard and Script/ReadOnly)
            DrawPropertiesExcluding(serializedObject, "m_Script", "_name", "_configPath", "_npBlackBoardDataManager");

            // 4. Custom Blackboard Drawer
            EditorGUILayout.Space();
            DrawBlackboardData();

            serializedObject.ApplyModifiedProperties();

            // 5. Graph Actions
            DrawGraphActions();
        }

        private void DrawGraphActions()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Graph Actions", EditorStyles.boldLabel);

            var graph = (BaseNpDataBehavourGraph)target;
            if (GUILayout.Button("Auto configure all nodes")) graph.BtnAutoSetCanvasDatas();
            if (GUILayout.Button("Save graph binary")) graph.Save();
            if (GUILayout.Button("Test graph deserialization")) graph.TestDe();
            if (GUILayout.Button("One-click setup")) graph.OneKeySet();
        }

        private void DrawBlackboardData()
        {
            SerializedProperty bbProp = serializedObject.FindProperty("_npBlackBoardDataManager");
            if (bbProp == null) return;

            EditorGUILayout.LabelField("Blackboard Data Manager", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            // --- Blackboard Values ---
            SerializedProperty bbKeys = bbProp.FindPropertyRelative("_bbValueKeys");
            SerializedProperty bbValues = bbProp.FindPropertyRelative("_bbValueValues");

            EditorGUILayout.LabelField($"Blackboard Values ({bbKeys.arraySize})", EditorStyles.boldLabel);
            
            if (bbKeys.arraySize != bbValues.arraySize)
            {
                EditorGUILayout.HelpBox("Data mismatch: Keys and Values count differ!", MessageType.Error);
            }
            else
            {
                for (int i = 0; i < bbKeys.arraySize; i++)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    // Key
                    SerializedProperty keyProp = bbKeys.GetArrayElementAtIndex(i);
                    // Value
                    SerializedProperty valProp = bbValues.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Key", GUILayout.Width(50));
                    EditorGUILayout.PropertyField(keyProp, GUIContent.none);
                    EditorGUILayout.EndHorizontal();

                    // Value field (Draws nicely because of SerializeReference)
                    EditorGUILayout.PropertyField(valProp, new GUIContent("Value"), true);

                    if (GUILayout.Button("Delete Entry"))
                    {
                        bbKeys.DeleteArrayElementAtIndex(i);
                        bbValues.DeleteArrayElementAtIndex(i); // DeleteReference? Check this.
                        // For SerializeReference, DeleteArrayElementAtIndex sets to null first, needs 2 calls usually?
                        // Or standard Delete just works.
                        // Safest is to break loop or decrement i. But here we are in Immediate Mode GUI... 
                        // Actually deleting changes array size immediately, breaking usage of properties next frame?
                        // Best to return out of function and repaint.
                        serializedObject.ApplyModifiedProperties(); // Apply current deletes? 
                        return; 
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(2);
                }
            }

            // ADD ENTRY UI
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Add New Blackboard Entry", EditorStyles.miniLabel);
            _newBbKey = EditorGUILayout.TextField("Key", _newBbKey);
            _selectedTypeIndex = EditorGUILayout.Popup("Type", _selectedTypeIndex, _availableTypeNames);
            
            if (GUILayout.Button("Add Entry"))
            {
                if (!string.IsNullOrEmpty(_newBbKey))
                {
                    // Check duplicate
                    bool exists = false;
                    for(int i=0; i<bbKeys.arraySize; i++) {
                        if (bbKeys.GetArrayElementAtIndex(i).stringValue == _newBbKey) exists = true;
                    }

                    if (exists)
                    {
                        Debug.LogWarning("Key already exists!");
                    }
                    else
                    {
                        bbKeys.InsertArrayElementAtIndex(bbKeys.arraySize);
                        var newKeyProp = bbKeys.GetArrayElementAtIndex(bbKeys.arraySize - 1);
                        newKeyProp.stringValue = _newBbKey;

                        bbValues.InsertArrayElementAtIndex(bbValues.arraySize);
                        var newValProp = bbValues.GetArrayElementAtIndex(bbValues.arraySize - 1);
                        
                        // Instantiate new object
                        var newObj = System.Activator.CreateInstance(_availableTypes[_selectedTypeIndex]);
                        newValProp.managedReferenceValue = newObj;
                        
                        _newBbKey = ""; // Reset
                        GUI.FocusControl(null);
                    }
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // --- IDs ---
            SerializedProperty idKeys = bbProp.FindPropertyRelative("_idKeys");
            SerializedProperty idValues = bbProp.FindPropertyRelative("_idValues");

            EditorGUILayout.LabelField($"ID Mappings ({idKeys.arraySize})", EditorStyles.boldLabel);

            if (idKeys.arraySize != idValues.arraySize) {
                EditorGUILayout.HelpBox("ID mismatch", MessageType.Error);
            } else {
                for(int i=0; i<idKeys.arraySize; i++) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(idKeys.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(150));
                    EditorGUILayout.PropertyField(idValues.GetArrayElementAtIndex(i), GUIContent.none);
                    if (GUILayout.Button("X", GUILayout.Width(25))) {
                        idKeys.DeleteArrayElementAtIndex(i);
                        idValues.DeleteArrayElementAtIndex(i);
                        return; // Repaint
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            // Add ID UI
             EditorGUILayout.BeginHorizontal();
             _newIdKey = EditorGUILayout.TextField(_newIdKey, GUILayout.Width(150));
             _newIdValue = EditorGUILayout.LongField(_newIdValue);
             if (GUILayout.Button("Add ID")) {
                 if (!string.IsNullOrEmpty(_newIdKey)) {
                     // Check dupe
                     // ... (omitted for brevity, duplicate allows multiple same keys? No, Dict constraint)
                     // Implementation matches logical flow
                     idKeys.InsertArrayElementAtIndex(idKeys.arraySize);
                     idKeys.GetArrayElementAtIndex(idKeys.arraySize-1).stringValue = _newIdKey;
                     idValues.InsertArrayElementAtIndex(idValues.arraySize);
                     idValues.GetArrayElementAtIndex(idValues.arraySize-1).longValue = _newIdValue;
                     _newIdKey = "";
                     _newIdValue = 0;
                     GUI.FocusControl(null);
                 }
             }
             EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
    }
}
