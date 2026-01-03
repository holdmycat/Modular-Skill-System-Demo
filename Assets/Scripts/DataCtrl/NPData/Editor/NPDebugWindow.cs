#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Default editor listener that tracks last node event per tree and optional UI window.
    /// </summary>
    internal class NPDebugLogListener : INPNodeDebugListener
    {
        internal class State
        {
            public string NodeName;
            public NPDebugEventType EventType;
            public bool Success;
            public double Time;
            public uint OwnerId;
            public bool IsServer;
        }

        private readonly Dictionary<long, State> _states = new Dictionary<long, State>();

        public IReadOnlyDictionary<long, State> States => _states;

        public void OnNodeEvent(NP_RuntimeTree tree, Node node, NPDebugEventType evt, bool success)
        {
            if (tree == null || node == null) return;
            _states[tree.RunTimeTreeId] = new State
            {
                NodeName = node.Name,
                EventType = evt,
                Success = success,
                Time = EditorApplication.timeSinceStartup,
                OwnerId = tree.BelongToUnit,
                IsServer = tree.Context?.IsServer ?? true
            };
        }
    }

    [InitializeOnLoad]
    internal static class NPDebugListenerBootstrap
    {
        internal static readonly NPDebugLogListener Listener;

        static NPDebugListenerBootstrap()
        {
            Listener = new NPDebugLogListener();
            NPDebugEventManager.Listener = Listener;
        }
    }

    public class NPDebugWindow : EditorWindow
    {
        [MenuItem("Window/NP Debugger")]
        public static void ShowWindow()
        {
            GetWindow<NPDebugWindow>("NP Debugger");
        }

        private void OnGUI()
        {
            var listener = NPDebugListenerBootstrap.Listener;
            EditorGUILayout.LabelField("Active Trees", EditorStyles.boldLabel);
            foreach (var kvp in listener.States)
            {
                var state = kvp.Value;
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"TreeId: {kvp.Key} ({(state.IsServer ? "Server" : "Client")}) Owner:{state.OwnerId}");
                EditorGUILayout.LabelField($"Node: {state.NodeName}");
                EditorGUILayout.LabelField($"Event: {state.EventType} {(state.EventType == NPDebugEventType.Exit ? $"Success:{state.Success}" : string.Empty)}");
                EditorGUILayout.LabelField($"Time: {state.Time:F2}");
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif
