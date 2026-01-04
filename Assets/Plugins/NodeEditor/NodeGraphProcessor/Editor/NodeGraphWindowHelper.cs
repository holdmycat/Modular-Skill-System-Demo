using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GraphProcessor
{
    public static class NodeGraphWindowHelper
    {
        private static Dictionary<string, BaseGraphWindow> AllNodeGraphWindows =
            new Dictionary<string, BaseGraphWindow>();

        private static T GetAndShowNodeGraphWindow<T>(string path) where T : BaseGraphWindow
        {
            if (AllNodeGraphWindows.TryGetValue(path, out var universalGraphWindow))
            {
                universalGraphWindow.Focus();
                return universalGraphWindow as T;
            }

#if UNITY_2020_1_OR_NEWER
            // Create a fresh window (instead of reusing GetWindow) so each asset opens in its own tab,
            // and dock it next to the Scene view by default.
            T resultWindow = EditorWindow.CreateWindow<T>(new[] { typeof(SceneView) });
            resultWindow.titleContent = new GUIContent(typeof(T).Name);
            resultWindow.Focus();
#else
            T resultWindow = EditorWindow.GetWindow<T>();
#endif
            AllNodeGraphWindows[path] = resultWindow;
            return resultWindow;
        }

        public static T GetAndShowNodeGraphWindow<T>(BaseGraph owner) where T : BaseGraphWindow
        {
            return GetAndShowNodeGraphWindow<T>(AssetDatabase.GetAssetPath(owner));
        }

        public static void AddNodeGraphWindow(BaseGraph owner, BaseGraphWindow universalGraphWindow)
        {
            AllNodeGraphWindows[AssetDatabase.GetAssetPath(owner)] = universalGraphWindow;
        }
        
        public static void RemoveNodeGraphWindow(string path)
        {
            AllNodeGraphWindows.Remove(path);
        }
        
        public static void RemoveNodeGraphWindow(BaseGraph owner)
        {
            AllNodeGraphWindows.Remove(AssetDatabase.GetAssetPath(owner));
        }
    }
}
