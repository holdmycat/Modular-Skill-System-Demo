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
            if (bbValueProp != null)
            {
                EditorGUI.PropertyField(line, bbValueProp, new GUIContent("BB Value"), true);
            }

            EditorGUI.EndProperty();
        }

        private void DrawBBKeyDropdown(Rect line, SerializedProperty bbKeyProp, SerializedProperty typeProp, SerializedProperty bbValueProp)
        {
            var keys = GetBBKeys().ToList();
            int currentIndex = Mathf.Max(0, keys.IndexOf(bbKeyProp.stringValue));
            int newIndex = EditorGUI.Popup(line, "BB Key", currentIndex, keys.ToArray());
            if (newIndex != currentIndex && newIndex >= 0 && newIndex < keys.Count)
            {
                var newKey = keys[newIndex];
                bbKeyProp.stringValue = newKey;
                // Copy value from manager if available
                var value = GetBBValue(newKey);
                if (value != null && bbValueProp != null)
                {
                    bbValueProp.managedReferenceValue = value.DeepCopy();
                    typeProp.stringValue = value.NP_BBValueType.ToString();
                }
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
    }
}
#endif
