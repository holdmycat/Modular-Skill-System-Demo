// File: EditorGUIStyleHelper.cs
// Summary: Cached helpers to clone and tweak GUIStyle instances by name.

using System.Collections.Generic;
using UnityEngine;

namespace GraphProcessor
{
    public static class EditorGUIStyleHelper
    {
        private static Dictionary<string, GUIStyle> s_Styles = new Dictionary<string, GUIStyle>();

        static EditorGUIStyleHelper()
        {
            s_Styles.Clear();
        }

        /// <summary>
        /// Get a cached deep copy of a GUIStyle by name (e.g., EditorGUIStyleHelper.GetGUIStyleByName(nameof(EditorStyles.toolbarButton))).
        /// </summary>
        public static GUIStyle GetGUIStyleByName(string styleName)
        {
            if (s_Styles.TryGetValue(styleName, out var guiStyle))
            {
                return guiStyle;
            }

            GUIStyle finalResult = new GUIStyle(styleName);
            s_Styles[styleName] = finalResult;
            return finalResult;
        }

        /// <summary>
        /// Set padding on a cached GUIStyle copy by name.
        /// </summary>
        public static GUIStyle SetGUIStylePadding(string styleName, RectOffset rectOffset)
        {
            GUIStyle guiStyle = GetGUIStyleByName(styleName);
            guiStyle.padding = rectOffset;
            return guiStyle;
        }
    }
}
