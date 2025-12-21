using Ebonor.Framework;
using UnityEngine;
using UnityEditor;

namespace Ebonor.Editor
{
    public class LogSettingsWindow : EditorWindow
    {
        private const string EnableFileLogKey = "Ebonor_EnableFileLog";
        
        [MenuItem("Custom Windows/Log Settings")]
        public static void ShowWindow()
        {
            GetWindow<LogSettingsWindow>("Log Settings");
        }

        private void OnGUI()
        {
            GUILayout.Label("Log Configuration", EditorStyles.boldLabel);

            bool currentSort = EditorPrefs.GetBool(EnableFileLogKey, false);
            bool enableFileLog = EditorGUILayout.Toggle("Write Log to File", currentSort);

            if (enableFileLog != currentSort)
            {
                EditorPrefs.SetBool(EnableFileLogKey, enableFileLog);
                UpdateLogSettings(enableFileLog);
            }
            
            GUILayout.Space(10);
            GUILayout.Label("Path: ProjectRoot/Logs/Log_yyyy-MM-dd_HH-mm.txt", EditorStyles.helpBox);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnRuntimeLoad()
        {
            bool enableFileLog = EditorPrefs.GetBool(EnableFileLogKey, false);
            UpdateLogSettings(enableFileLog);
        }

        private static void UpdateLogSettings(bool enable)
        {
            // Update the Default Log Factory
            if (LogManager.Default != null)
            {
                LogManager.Default.EnableFileLog = enable;
                Debug.Log($"[LogSettings] File Logging set to: {enable}");
            }
        }
    }
}
