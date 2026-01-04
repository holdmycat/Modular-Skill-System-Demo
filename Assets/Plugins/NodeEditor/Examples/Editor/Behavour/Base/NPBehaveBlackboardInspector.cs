using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Ebonor.DataCtrl;
using UnityEditor;
using UnityEngine;

namespace Plugins.NodeEditor
{
    /// <summary>
    /// Editable inspector for the transient Blackboard viewer ScriptableObject.
    /// Lets designers add / remove / edit Blackboard values, events, and ids like normal serialized data.
    /// </summary>
    [CustomEditor(typeof(NPBehaveToolbarView.BlackboardInspectorViewer))]
    public class NPBehaveBlackboardInspector : Editor
    {
        private static string _newKey = string.Empty;
        private static int _newTypeIndex;
        private static bool _showAddValueForm;
        private static readonly Type[] _bbValueTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => SafeGetTypes(a))
            .Where(IsAssignableBbValue)
            .OrderBy(t => t.Name)
            .ToArray();
        private static readonly string[] _bbValueTypeNames = _bbValueTypes.Select(t => t.Name).ToArray();

        public override void OnInspectorGUI()
        {
            var viewer = (NPBehaveToolbarView.BlackboardInspectorViewer)target;
            NP_BlackBoardDataManager manager = viewer.NpBlackBoardDataManager;

            if (manager == null)
            {
                EditorGUILayout.HelpBox("No Blackboard data assigned. Click the Blackboard button from an open graph.", MessageType.Info);
                return;
            }

            manager.BBValues ??= new Dictionary<string, ANP_BBValue>();
            manager.EventValues ??= new List<string>();
            manager.Ids ??= new Dictionary<string, long>();

            DrawBbValues(manager);
            EditorGUILayout.Space(8);
            DrawEventValues(manager);
            EditorGUILayout.Space(8);
            DrawIds(manager);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(viewer);
            }
        }

        #region Blackboard Values

        private void DrawBbValues(NP_BlackBoardDataManager manager)
        {
            EditorGUILayout.LabelField("Blackboard Values", EditorStyles.boldLabel);

            var keys = manager.BBValues.Keys.ToList();
            foreach (var key in keys)
            {
                var value = manager.BBValues[key];
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Key", GUILayout.Width(30));
                string newKey = EditorGUILayout.TextField(key, GUILayout.MinWidth(200), GUILayout.ExpandWidth(true));
                var currentType = value?.GetType();
                int currentIndex = Array.IndexOf(_bbValueTypes, currentType);
                int selected = EditorGUILayout.Popup(Mathf.Max(currentIndex, 0), _bbValueTypeNames, GUILayout.Width(140));
                var selectedType = _bbValueTypes.Length > 0 ? _bbValueTypes[selected] : null;

                if (selectedType != null && currentType != selectedType)
                {
                    value = (ANP_BBValue)Activator.CreateInstance(selectedType);
                    manager.BBValues[key] = value;
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Delete", GUILayout.Width(70)))
                {
                    manager.BBValues.Remove(key);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }

                if (newKey != key && !string.IsNullOrEmpty(newKey))
                {
                    manager.BBValues.Remove(key);
                    manager.BBValues[newKey] = value;
                }
                EditorGUILayout.EndHorizontal();

                if (value != null)
                {
                    DrawValueField(value);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            }

            EditorGUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(28)))
            {
                _showAddValueForm = true;
                _newKey = string.Empty;
                _newTypeIndex = 0;
            }
            EditorGUILayout.LabelField("Click + to add a new Blackboard Value", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();

            if (_showAddValueForm)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("New Blackboard Value", EditorStyles.miniBoldLabel);
                _newKey = EditorGUILayout.TextField("Key", _newKey);
                _newTypeIndex = EditorGUILayout.Popup("Type", _newTypeIndex, _bbValueTypeNames);

                EditorGUILayout.BeginHorizontal();
                using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(_newKey) || _bbValueTypes.Length == 0))
                {
                    if (GUILayout.Button("Create"))
                    {
                        var type = _bbValueTypes[_newTypeIndex];
                        manager.BBValues[_newKey] = (ANP_BBValue)Activator.CreateInstance(type);
                        _newKey = string.Empty;
                        _showAddValueForm = false;
                    }
                }

                if (GUILayout.Button("Cancel"))
                {
                    _showAddValueForm = false;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawValueField(ANP_BBValue value)
        {
            var valueField = value.GetType().GetField("Value", BindingFlags.Public | BindingFlags.Instance);
            if (valueField == null)
            {
                EditorGUILayout.LabelField("Value", "(uneditable)");
                return;
            }

            object cur = valueField.GetValue(value);
            Type fieldType = valueField.FieldType;

            object newVal = cur;
            if (fieldType == typeof(string))
                newVal = EditorGUILayout.TextField("Value", cur as string ?? string.Empty);
            else if (fieldType == typeof(int))
                newVal = EditorGUILayout.IntField("Value", cur is int i ? i : 0);
            else if (fieldType == typeof(long))
                newVal = EditorGUILayout.LongField("Value", cur is long l ? l : 0);
            else if (fieldType == typeof(uint))
            {
                long temp = EditorGUILayout.LongField("Value", cur is uint ui ? ui : 0);
                if (temp < 0) temp = 0;
                if (temp > uint.MaxValue) temp = uint.MaxValue;
                newVal = (uint)temp;
            }
            else if (fieldType == typeof(float))
                newVal = EditorGUILayout.FloatField("Value", cur is float f ? f : 0f);
            else if (fieldType == typeof(bool))
                newVal = EditorGUILayout.Toggle("Value", cur is bool b && b);
            else if (fieldType == typeof(Vector3))
                newVal = EditorGUILayout.Vector3Field("Value", cur is Vector3 v ? v : Vector3.zero);
            else if (typeof(IList).IsAssignableFrom(fieldType))
                newVal = DrawListField(fieldType, cur as IList);
            else
                EditorGUILayout.LabelField("Value", $"(unsupported type: {fieldType.Name})");

            if (!Equals(newVal, cur))
            {
                valueField.SetValue(value, newVal);
            }
        }

        private IList DrawListField(Type listType, IList list)
        {
            var elementType = listType.IsGenericType ? listType.GetGenericArguments()[0] : typeof(object);
            if (list == null)
            {
                list = (IList)Activator.CreateInstance(listType);
            }

            int size = EditorGUILayout.IntField("Size", list.Count);
            size = Mathf.Max(0, size);
            while (list.Count < size) list.Add(GetDefault(elementType));
            while (list.Count > size) list.RemoveAt(list.Count - 1);

            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                object elem = list[i];
                if (elementType == typeof(long))
                {
                    elem = EditorGUILayout.LongField($"Element {i}", elem is long l ? l : 0);
                }
                else if (elementType == typeof(int))
                {
                    elem = EditorGUILayout.IntField($"Element {i}", elem is int iv ? iv : 0);
                }
                else if (elementType == typeof(byte))
                {
                    int val = EditorGUILayout.IntField($"Element {i}", elem is byte b ? b : 0);
                    val = Mathf.Clamp(val, 0, 255);
                    elem = (byte)val;
                }
                else
                {
                    EditorGUILayout.LabelField($"Element {i}", $"(unsupported type: {elementType.Name})");
                }
                list[i] = elem;
            }
            EditorGUI.indentLevel--;
            return list;
        }

        private object GetDefault(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;

        private static bool IsAssignableBbValue(Type type)
        {
            try
            {
                return typeof(ANP_BBValue).IsAssignableFrom(type) && !type.IsAbstract;
            }
            catch (System.Exception ex) when (ex is TypeLoadException || ex is FileNotFoundException)
            {
                // Skip types that cannot be loaded because of missing dependencies.
                return false;
            }
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly asm)
        {
            try { return asm.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null); }
            catch (TypeLoadException) { return Enumerable.Empty<Type>(); }
        }

        #endregion

        #region Events & IDs

        private void DrawEventValues(NP_BlackBoardDataManager manager)
        {
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            var list = manager.EventValues;
            int size = EditorGUILayout.IntField("Size", list.Count);
            size = Mathf.Max(0, size);
            while (list.Count < size) list.Add(string.Empty);
            while (list.Count > size) list.RemoveAt(list.Count - 1);

            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = EditorGUILayout.TextField($"Event {i}", list[i]);
            }
            EditorGUI.indentLevel--;
        }

        private void DrawIds(NP_BlackBoardDataManager manager)
        {
            EditorGUILayout.LabelField("IDs", EditorStyles.boldLabel);
            var keys = manager.Ids.Keys.ToList();
            foreach (var key in keys)
            {
                EditorGUILayout.BeginHorizontal();
                string newKey = EditorGUILayout.TextField(key, GUILayout.Width(EditorGUIUtility.labelWidth));
                long val = EditorGUILayout.LongField(manager.Ids[key]);

                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    manager.Ids.Remove(key);
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                else
                {
                    if (newKey != key && !string.IsNullOrEmpty(newKey))
                    {
                        manager.Ids.Remove(key);
                        manager.Ids[newKey] = val;
                    }
                    else
                    {
                        manager.Ids[key] = val;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Add ID", GUILayout.Width(60));
            string addKey = EditorGUILayout.TextField(string.Empty);
            long addVal = EditorGUILayout.LongField(0);
            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(addKey)))
            {
                if (GUILayout.Button("Add / Replace", GUILayout.Width(100)))
                {
                    manager.Ids[addKey] = addVal;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}
