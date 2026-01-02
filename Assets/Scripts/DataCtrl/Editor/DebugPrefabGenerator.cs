using UnityEngine;
using UnityEditor;
using System.IO;

public class DebugPrefabGenerator
{
    [MenuItem("Custom Windows/Debug/Generate Debug Prefabs")]
    public static void Generate()
    {
        string path = "Assets/Resources/Models";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // Commander Capsule
        GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        cap.name = "DebugCommander";
        PrefabUtility.SaveAsPrefabAsset(cap, path + "/DebugCommander.prefab");
        GameObject.DestroyImmediate(cap);

        // Squad Cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "DebugSquad";
        PrefabUtility.SaveAsPrefabAsset(cube, path + "/DebugSquad.prefab");
        GameObject.DestroyImmediate(cube);

        AssetDatabase.Refresh();
        Debug.Log("Generated DebugCommander and DebugSquad prefabs in Resources/Models");
    }
}
