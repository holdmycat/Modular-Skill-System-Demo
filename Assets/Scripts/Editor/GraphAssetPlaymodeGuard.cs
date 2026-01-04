using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Snapshot graph assets before entering play mode and restore them on exit,
/// preventing runtime mutations from persisting to .asset files.
/// </summary>
[InitializeOnLoad]
public static class GraphAssetPlaymodeGuard
{
    private static readonly Dictionary<string, byte[]> Snapshots = new Dictionary<string, byte[]>();

    static GraphAssetPlaymodeGuard()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                Snapshot();
                break;
            case PlayModeStateChange.EnteredEditMode:
                Restore();
                break;
        }
    }

    private static void Snapshot()
    {
        Snapshots.Clear();
        var guids = AssetDatabase.FindAssets("t:BaseGraph", new[] { "Assets/Plugins/NodeEditor/Examples/Saves" });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!File.Exists(path)) continue;
            try
            {
                Snapshots[path] = File.ReadAllBytes(path);
            }
            catch (IOException e)
            {
                Debug.LogWarning($"[GraphAssetPlaymodeGuard] Snapshot failed for {path}: {e.Message}");
            }
        }
    }

    private static void Restore()
    {
        foreach (var kvp in Snapshots)
        {
            var path = kvp.Key;
            var bytes = kvp.Value;
            if (bytes == null || bytes.Length == 0) continue;

            try
            {
                File.WriteAllBytes(path, bytes);
                AssetDatabase.ImportAsset(path);
            }
            catch (IOException e)
            {
                Debug.LogWarning($"[GraphAssetPlaymodeGuard] Restore failed for {path}: {e.Message}");
            }
        }

        Snapshots.Clear();
    }
}
