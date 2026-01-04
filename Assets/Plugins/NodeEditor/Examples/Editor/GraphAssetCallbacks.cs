using UnityEditor;
using GraphProcessor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Plugins.NodeEditor
{
    //attributes
    public partial class GraphAssetCallbacks
    {
        
        [MenuItem("Assets/Create/AllSlgUnitAttributesData", false, 11)]
        public static void CreateGraphPorcessor_AllSlgUnitAttributesData()
        {
            var graph = ScriptableObject.CreateInstance<SlgUnitAttributesDataGraph>();
            ProjectWindowUtil.CreateAsset(graph, "AllSlgUnitAttrDataGraph.asset");
        }

        [MenuItem("Assets/Create/AllSlgSquadAttributesData", false, 12)]
        public static void CreateGraphProcessor_AllSlgSquadAttributesData()
        {
            var graph = ScriptableObject.CreateInstance<SlgUnitSquadAttributesDataGraph>();
            ProjectWindowUtil.CreateAsset(graph, "AllSlgSquadAttrDataGraph.asset");
        }
      
        [MenuItem("Assets/Create/AllSlgCommanderAttributesData", false, 12)]
        public static void CreateGraphProcessor_AllSlgCommanderAttributesData()
        {
            var graph = ScriptableObject.CreateInstance<SlgCommanderAttributesDataGraph>();
            ProjectWindowUtil.CreateAsset(graph, "AllSlgCommanderAttrDataGraph.asset");
        }
        
        [MenuItem("Assets/Create/Behavour/SlgSquadBehavourGraph", false, 10)]
        public static void CreateGraphPorcessor_NpcBehaviour()
        {
            var graph = ScriptableObject.CreateInstance<SlgSquadBehavourGraph>();
            ProjectWindowUtil.CreateAsset(graph, "SlgSquadBehavourGraph.asset");
        }
        
        
        // [MenuItem("Assets/Create/AllWeaponAttributesData", false, 10)]
        // public static void CreateGraphPorcessor_AllWeaponAttributesData()
        // {
        //     var graph = ScriptableObject.CreateInstance<WeaponAttributesDataGraph>();
        //     ProjectWindowUtil.CreateAsset(graph, "AllWeaponAttriDataGraph.asset");
        // }
        //
        // [MenuItem("Assets/Create/All Drop Attributes Data", false, 10)]
        // public static void CreateGraphPorcessor_AllDropAttributesData()
        // {
        //     var graph = ScriptableObject.CreateInstance<DropAttributesDataGraph>();
        //     ProjectWindowUtil.CreateAsset(graph, "AllDropAttriDataGraph.asset");
        // }
        //
        // [MenuItem("Assets/Create/MPServerData", false, 10)]
        // public static void CreateGraphPorcessor_MPServerData()
        // {
        //     var graph = ScriptableObject.CreateInstance<MPServerDataGraph>();
        //     ProjectWindowUtil.CreateAsset(graph, "MPServerDataGraph.asset");
        // }
        //      
        // [MenuItem("Assets/Create/AllGameAttributesData", false, 10)]
        // public static void CreateGraphPorcessor_AllGameAttributesData()
        // {
        //     var graph = ScriptableObject.CreateInstance<GameAttributesDataGraph>();
        //     ProjectWindowUtil.CreateAsset(graph, "AllGameAttriDataGraph.asset");
        // }
    }
    
    //system
    public partial class GraphAssetCallbacks
    {
        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            // Avoid touching graph assets while in play mode to prevent runtime data from resetting editor assets.
            if (Application.isPlaying) return false;

            var baseGraph = EditorUtility.InstanceIDToObject(instanceID) as BaseGraph;
            // Only handle node graph assets; let Unity handle everything else so scene double-click works.
            if (baseGraph == null) return false;
            return InitializeGraph(baseGraph);
        }

        public static bool InitializeGraph(BaseGraph baseGraph)
        {
            if (baseGraph == null) return false;

            switch (baseGraph)
            {
                case SlgUnitAttributesDataGraph slgGraph:
                    NodeGraphWindowHelper.GetAndShowNodeGraphWindow<SlgUnitAttributesDataGraphWindow>(slgGraph)
                        .InitializeGraph(slgGraph);
                    return true;
                case SlgUnitSquadAttributesDataGraph slgSquadGraph:
                    NodeGraphWindowHelper.GetAndShowNodeGraphWindow<SlgUnitSquadAttributesDataGraphWindow>(slgSquadGraph)
                        .InitializeGraph(slgSquadGraph);
                    return true;
                case SlgCommanderAttributesDataGraph slgCommanderGraph:
                    NodeGraphWindowHelper.GetAndShowNodeGraphWindow<SlgCommanderAttributesDataGraphWindow>(slgCommanderGraph)
                        .InitializeGraph(slgCommanderGraph);
                    return true;
                
                case SlgSquadBehavourGraph slgSquadBehavourGraph:
                    NodeGraphWindowHelper.GetAndShowNodeGraphWindow<SlgSquadBehavourGraphWindow>(slgSquadBehavourGraph)
                        .InitializeGraph(slgSquadBehavourGraph);
                    return true;
                
                default:
                    NodeGraphWindowHelper.GetAndShowNodeGraphWindow<FallbackGraphWindow>(baseGraph)
                        .InitializeGraph(baseGraph);
                    return true;
            }
        }
    }
}
