using UnityEditor;
using GraphProcessor;
using UnityEditor.Callbacks;

namespace Plugins.NodeEditor
{

    // File: GraphAssetCallbacks.cs
    // Summary: Menu items for creating and opening graph assets; routes asset opens to the right graph window.

    //skills
    // public partial class GraphAssetCallbacks
    // {
        
    //     /// <summary>
    //     /// Skill Attributes
    //     /// </summary>
    //     [MenuItem("Assets/Create/AllSkillAttributesData", false, 10)]
    //     public static void CreateGraphPorcessor_AllActiveSkillAttributesData()
    //     {
    //         var graph = ScriptableObject.CreateInstance<SkillAttributesDataGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "AllSkillAttriDataGraph.asset");
    //     }
    //     
    //     /// <summary>
    //     /// Active Skill
    //     /// </summary>
    //     [MenuItem("Assets/Create/GraphProcessor_Skill", false, 10)]
    //     public static void CreateGraphPorcessor_Skill()
    //     {
    //         var path = GlobalHelper.GetSelectedPathOrFallback();
    //         //Debug.Log("CreateGraphPorcessor_Skill, path:" + path);
    //         var graph = ScriptableObject.CreateInstance<SkillGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "SkillGraph.asset");
    //        
    //         graph.OnInitGraph(path);
    //     }
    //
    //     /// <summary>
    //     /// Support Skill
    //     /// </summary>
    //     [MenuItem("Assets/Create/GraphProcessor_SupportSkill", false, 10)]
    //     public static void CreateGraphPorcessor_SupportSkill()
    //     {
    //         var path = GlobalHelper.GetSelectedPathOrFallback();
    //         var graph = ScriptableObject.CreateInstance<SupportSkillGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "SupportSkillGraph.asset");
    //         graph.OnInitGraph(path);
    //     }
    //     
    //     
    //     
    //     /// <summary>
    //     /// Event Skill
    //     /// </summary>
    //     [MenuItem("Assets/Create/GraphProcessor_EventSkill", false, 10)]
    //     public static void CreateGraphPorcessor_EventSkill()
    //     {
    //         var path = GlobalHelper.GetSelectedPathOrFallback();
    //         //Debug.Log("CreateGraphPorcessor_Skill, path:" + path);
    //         var graph = ScriptableObject.CreateInstance<SkillGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "SkillGraph.asset");
    //        
    //         graph.OnInitGraph(path);
    //     }
    //     
    //     
    //     [MenuItem("Assets/Create/All Bullet Attributes Data", false, 10)]
    //     public static void CreateGraphPorcessor_AllBulletAttributesData()
    //     {
    //         var graph = ScriptableObject.CreateInstance<BulletAttributesDataGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "AllBulletAttriDataGraph.asset");
    //     }
    //     
    //     [MenuItem("Assets/Create/All Summon Attributes Data", false, 10)]
    //     public static void CreateGraphPorcessor_AllSummonAttributesData()
    //     {
    //         var graph = ScriptableObject.CreateInstance<SummonAttributesDataGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "AllSummonAttriDataGraph.asset");
    //     }
    // }
    //
    // //passive weapon skills
    // public partial class GraphAssetCallbacks
    // {
    //     [MenuItem("Assets/Create/GraphProcessor_PassiveWeaponSkill", false, 10)]
    //     public static void CreateGraphPorcessor_PassiveWeaponSkill()
    //     {
    //         var graph = ScriptableObject.CreateInstance<PassiveWeaponSkillGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "PassiveWeaponSkill.asset");
    //         graph.OnInitGraph("");
    //     }
    // }
    //
    // //npc behaviours
    // public partial class GraphAssetCallbacks
    // {
    //     [MenuItem("Assets/Create/GraphProcessor_NpcBehaviour", false, 10)]
    //     public static void CreateGraphPorcessor_NpcBehaviour()
    //     {
    //         var graph = ScriptableObject.CreateInstance<NpcBehaviourGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "NpcBehaviour.asset");
    //         graph.OnInitGraph("");
    //     }
    // }
    //
    // //attributes
    // public partial class GraphAssetCallbacks
    // {
    //     [MenuItem("Assets/Create/AllHeroAttributesData", false, 10)]
    //     public static void CreateGraphPorcessor_AllHeroAttributesData()
    //     {
    //         var graph = ScriptableObject.CreateInstance<UnitAttributesDataGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "AllHeroAttriDataGraph.asset");
    //     }
    //     
    //     [MenuItem("Assets/Create/AllWeaponAttributesData", false, 10)]
    //     public static void CreateGraphPorcessor_AllWeaponAttributesData()
    //     {
    //         var graph = ScriptableObject.CreateInstance<WeaponAttributesDataGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "AllWeaponAttriDataGraph.asset");
    //     }
    //     
    //     [MenuItem("Assets/Create/All Drop Attributes Data", false, 10)]
    //     public static void CreateGraphPorcessor_AllDropAttributesData()
    //     {
    //         var graph = ScriptableObject.CreateInstance<DropAttributesDataGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "AllDropAttriDataGraph.asset");
    //     }
    //     
    //     [MenuItem("Assets/Create/MPServerData", false, 10)]
    //     public static void CreateGraphPorcessor_MPServerData()
    //     {
    //         var graph = ScriptableObject.CreateInstance<MPServerDataGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "MPServerDataGraph.asset");
    //     }
    //          
    //     [MenuItem("Assets/Create/AllGameAttributesData", false, 10)]
    //     public static void CreateGraphPorcessor_AllGameAttributesData()
    //     {
    //         var graph = ScriptableObject.CreateInstance<GameAttributesDataGraph>();
    //         ProjectWindowUtil.CreateAsset(graph, "AllGameAttriDataGraph.asset");
    //     }
    // }
    
    
    //system
    public partial class GraphAssetCallbacks
    {
        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            var baseGraph = EditorUtility.InstanceIDToObject(instanceID) as BaseGraph;
            // Only handle node graph assets; let Unity handle everything else so scene double-click works.
            if (baseGraph == null) return false;
            return InitializeGraph(baseGraph);
        }

        public static bool InitializeGraph(BaseGraph baseGraph)
        {
            // if (baseGraph == null) return false;
            //
            // switch (baseGraph)
            // {
            //     case SkillGraph skillGraph:
            //         NodeGraphWindowHelper.GetAndShowNodeGraphWindow<SkillGraphWindow>(skillGraph)
            //             .InitializeGraph(skillGraph);
            //         break;
            //     case SupportSkillGraph skillGraph:
            //         NodeGraphWindowHelper.GetAndShowNodeGraphWindow<SupportSkillGraphWindow>(skillGraph)
            //             .InitializeGraph(skillGraph);
            //         break;
            //     case PassiveWeaponSkillGraph passiveWeaponSkillGraph:
            //     {
            //         NodeGraphWindowHelper.GetAndShowNodeGraphWindow<PassiveWeaponSkillGraphWindow>(passiveWeaponSkillGraph)
            //             .InitializeGraph(passiveWeaponSkillGraph);
            //         break;
            //     }
            //     
            //         
            //     case NpcBehaviourGraph npcBehaviourGraph:
            //     {
            //         NodeGraphWindowHelper.GetAndShowNodeGraphWindow<NpcBehaviourGraphWindow>(npcBehaviourGraph)
            //             .InitializeGraph(npcBehaviourGraph);
            //         break;
            //     }
            //     // case NPBehaveGraph npBehaveGraph:
            //     //     NodeGraphWindowHelper.GetAndShowNodeGraphWindow<NPBehaveGraphWindow>(npBehaveGraph)
            //     //         .InitializeGraph(npBehaveGraph);
            //     //     break;
            //     default:
            //         NodeGraphWindowHelper.GetAndShowNodeGraphWindow<FallbackGraphWindow>(baseGraph)
            //             .InitializeGraph(baseGraph);
            //         break;
            // }

            return true;
        }
    }
}
