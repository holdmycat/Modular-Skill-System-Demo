#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    [CustomPropertyDrawer(typeof(NP_BlackBoardRelationData))]
    public class NP_BlackBoardRelationDataDrawer : PropertyDrawer
    {
        private static readonly System.Type[] _bbValueTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => SafeGetTypes(a))
            .Where(t => typeof(ANP_BBValue).IsAssignableFrom(t) && !t.IsAbstract)
            .OrderBy(t => t.Name)
            .ToArray();
        private static readonly string[] _bbValueTypeNames = _bbValueTypes.Select(t => t.Name).ToArray();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Calculate height based on children
            float height = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 3;
            var bbValueProp = property.FindPropertyRelative("NP_BBValue");
            if (bbValueProp != null && bbValueProp.hasChildren)
            {
                height += EditorGUI.GetPropertyHeight(bbValueProp, true);
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var bbKeyProp = property.FindPropertyRelative("BBKey");
            var bbValueTypeProp = property.FindPropertyRelative("NP_BBValueType");
            var writeCompareProp = property.FindPropertyRelative("WriteOrCompareToBB");
            var bbValueProp = property.FindPropertyRelative("NP_BBValue");

            var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(line, "Blackboard Relation", EditorStyles.boldLabel);

            line.y += line.height + EditorGUIUtility.standardVerticalSpacing;
            DrawBBKeyDropdown(line, bbKeyProp, bbValueTypeProp, bbValueProp);

            line.y += line.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(line, writeCompareProp, new GUIContent("Write/Compare"));

            line.y += line.height + EditorGUIUtility.standardVerticalSpacing;
            DrawBbValueWithFallback(line, bbValueProp, bbValueTypeProp, bbKeyProp != null ? bbKeyProp.stringValue : string.Empty);

            EditorGUI.EndProperty();
        }

        private void DrawBBKeyDropdown(Rect line, SerializedProperty bbKeyProp, SerializedProperty typeProp, SerializedProperty bbValueProp)
        {
            var keys = GetBBKeys().ToList();
            int currentIndex = Mathf.Max(0, keys.IndexOf(bbKeyProp.stringValue));
            // Split rect: popup + refresh button
            var popupRect = new Rect(line.x, line.y, line.width - 70, line.height);
            var buttonRect = new Rect(line.x + line.width - 65, line.y, 65, line.height);

            int newIndex = EditorGUI.Popup(popupRect, "BB Key", currentIndex, keys.ToArray());
            bool forceRefresh = GUI.Button(buttonRect, "刷新");

            if (newIndex >= 0 && newIndex < keys.Count)
            {
                var newKey = keys[newIndex];
                bbKeyProp.stringValue = newKey;
                // Refresh only when key changed, value is null, or user clicked refresh
                if (forceRefresh || newIndex != currentIndex || (bbValueProp != null && bbValueProp.managedReferenceValue == null))
                {
                    var value = GetBBValue(newKey);
                    if (value != null && bbValueProp != null)
                    {
                        bbValueProp.managedReferenceValue = value.DeepCopy();
                        typeProp.stringValue = value.NP_BBValueType.ToString();
                    }
                }
            }
        }

        private void DrawBbValueWithFallback(Rect line, SerializedProperty bbValueProp, SerializedProperty typeProp, string selectedKey)
        {
            if (bbValueProp == null)
            {
                EditorGUI.LabelField(line, "BB Value", "n/a");
                return;
            }

            if (bbValueProp.managedReferenceValue == null)
            {
                // Try to hydrate from current blackboard manager using the selected key.
                var value = GetBBValue(selectedKey);
                if (value != null)
                {
                    bbValueProp.managedReferenceValue = value.DeepCopy();
                    typeProp.stringValue = value.NP_BBValueType.ToString();
                }
                else
                {
                    // Allow user to pick a value type even when CurrentEditedNP_BlackBoardDataManager is null or key not found.
                    int currentIndex = 0;
                    int newIndex = EditorGUI.Popup(line, "BB Value Type", currentIndex, _bbValueTypeNames);
                    if (newIndex >= 0 && newIndex < _bbValueTypes.Length)
                    {
                        var chosenType = _bbValueTypes[newIndex];
                        bbValueProp.managedReferenceValue = System.Activator.CreateInstance(chosenType);
                        typeProp.stringValue = chosenType.ToString();
                    }
                }
            }
            else
            {
                EditorGUI.PropertyField(line, bbValueProp, new GUIContent("BB Value"), true);
            }
        }

        private IEnumerable<string> GetBBKeys()
        {
            if (NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager != null)
            {
                return NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager.BBValues.Keys;
            }

            return new List<string> { "<None>" };
        }

        private ANP_BBValue GetBBValue(string key)
        {
            if (NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager != null &&
                NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager.BBValues.TryGetValue(key, out var val))
            {
                return val;
            }
            return null;
        }

        private static IEnumerable<System.Type> SafeGetTypes(System.Reflection.Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return new System.Type[] { };
            }
        }
    }
}
#endif
