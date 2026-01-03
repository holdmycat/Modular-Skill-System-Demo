//------------------------------------------------------------
// File: NP_RuntimeTree.cs
// Created: 2025-12-05
// Purpose: Runtime behaviour tree component that stores context IDs, blackboard access, and support data.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    
    public class NP_RuntimeTree : MonoBehaviour
    {
        static readonly ILog log = LogManager.GetLogger(typeof(NP_RuntimeTree));
        
        /// <summary>
        /// NP behaviour tree root node.
        /// </summary>
        Root m_RootNode;
        
        //public long UniqueTreeId = IdGenerater.Instance.GenerateId();
        
        /// <summary>
        /// Owning unit NetId.
        /// </summary>
        [Header("Runtime Tree Identifiers")]
        [Tooltip("NetId of the unit that owns the skill.")]
        public uint BelongToUnit;

        [Tooltip("NetId of the unit that triggers the skill.")]
        public uint StartToUnit;
        
        [Tooltip("NetId of the unit that is targeted by the skill.")]
        public uint TargetToUnit;
        
        /// <summary>
        /// Behaviour tree runtime ID.
        /// </summary>
        public long RunTimeTreeId;

        /// <summary>
        /// Runtime context (owner/target IDs, server/client flag, shared log).
        /// </summary>
        public NPRuntimeContext Context { get; private set; }
        
        /// <summary>
        /// Scene/room identifier.
        /// </summary>
        public string RoomId;
        

        /// <summary>
        /// Event or support skill index.
        /// </summary>
        public int EventSupportSkillIndex;
        
#if UNITY_EDITOR
        /// <summary>
        /// Blackboard key/value snapshot for editor display.
        /// </summary>
        [System.Serializable]
        public struct BlackBordData
        {
            public string key;
            public string value;
        }
        
        [Tooltip("Blackboard entries (editor-only debug view).")]
        public List<BlackBordData> mBordValues = new List<BlackBordData>();
#endif
        
        /// <summary>
        /// Data container this tree belongs to.
        /// </summary>
        public NP_DataSupportor BelongNP_DataSupportor;

        /// <summary>
        /// Skill animation parallel node.
        /// </summary>
        //public DynamicParallel DynamicParallelInst;

        /// <summary>
        /// Dictionary of skill events bound to nodes.
        /// </summary>
        public Dictionary<eSkillEventNode, CDynamicBuffMgr> DicDynamicSkillEventNode = new Dictionary<eSkillEventNode, CDynamicBuffMgr>();
        
        /// <summary>
        /// Currently active skill event node for this tree.
        /// </summary>
        public eSkillEventNode CurSkillEventNode;
        
        // /// <summary>
        // /// Event skill manager hooks.
        // /// </summary>
        // public List<System.Action<bool>> ListTimePauseResumeActions = new List<System.Action<bool>>();
        
        /// <summary>
        /// Skill data reference.
        /// </summary>
        //public ScriptSkillDataBase SkillData;
        
        /// <summary>
        /// Set the root node reference.
        /// </summary>
        /// <param name="rootNode">Root node instance.</param>
        public void SetRootNode(Root rootNode)
        {
            this.m_RootNode = rootNode;
        }


        protected Clock m_clock;
        public Clock Clock => m_clock;
        
        /// <summary>
        /// Get the shared blackboard instance.
        /// </summary>
        public Blackboard GetBlackboard()
        {
            if (m_RootNode == null)
            {
                log.Error($"Behavior tree {this.RunTimeTreeId} root node is null");
            }
            if (m_RootNode.Blackboard == null)
            {
                log.Error($"Behavior tree {this.RunTimeTreeId} blackboard instance is null");
            }
            return this.m_RootNode.Blackboard;
        }

        ///
       
        
        
        /// <summary>
        /// Start running the behaviour tree.
        /// </summary>
        public void Start()
        {
            this.m_RootNode.Start();
        }
        
        private void Awake()
        {
            if (null == DicDynamicSkillEventNode)
            {
                DicDynamicSkillEventNode = new Dictionary<eSkillEventNode, CDynamicBuffMgr>();
            }

            // if (null == ListTimePauseResumeActions)
            // {
            //     ListTimePauseResumeActions = new List<Action<bool>>();
            // }
        }

        private void OnDestroy()
        {
            DicDynamicSkillEventNode.Clear();
            //ListTimePauseResumeActions.Clear();
        }

#if UNITY_EDITOR      
        void Update()
        {
            if (m_RootNode == null) 
                return;
            if (GlobalHelper.CheckDevClient())
            {
                mBordValues = new List<BlackBordData>();
                var mDicBlackBord = GetBlackboard().GetDatas();
                foreach (var variable in mDicBlackBord.Keys)
                {
                    BlackBordData data = new BlackBordData();
                    data.key = variable;
                    var datavalue = mDicBlackBord[variable];
                    if (datavalue is NP_BBValue_String)
                    {
                        data.value = GetBlackboard().Get<string>(variable);
                    }else if (datavalue is NP_BBValue_Bool)
                    {
                        data.value = GetBlackboard().Get<bool>(variable).ToString();
                    }else if (datavalue is NP_BBValue_Int)
                    {
                        data.value = GetBlackboard().Get<int>(variable).ToString();
                    }else if (datavalue is NP_BBValue_Float)
                    {
                        data.value = GetBlackboard().Get<float>(variable).ToString();
                    }else if (datavalue is NP_BBValue_Long)
                    {
                        data.value = GetBlackboard().Get<long>(variable).ToString();
                    }else if (datavalue is NP_BBValue_List_Long)
                    {
                        data.value = GetBlackboard().Get<List<long>>(variable).ToString();
                    }else if (datavalue is NP_BBValue_UInt)
                    {
                        data.value = GetBlackboard().Get<uint>(variable).ToString();
                    }else if (datavalue is NP_BBValue_Vector3)
                    {
                        // data.value = GetBlackboard().Get<Vector3>(VARIABLE).ToString();
                    }
                
                    mBordValues.Add(data);
                }
            }
           
        }
#endif

        /// <summary>
        /// Stop the behaviour tree.
        /// </summary>
        public void Stop()
        {
            if (null != m_RootNode)
            {
                m_RootNode.Stop();
                BelongToUnit = 0;
                m_RootNode = null;
            }
          
        }

        public void OnInitSupportEventRuntimeTree(string _roomoId, uint belongid, uint startId, long _rootId,
            NP_DataSupportor datasupport, Clock _clock, uint targetid = 0, int index = 0)
        {
            EventSupportSkillIndex = index;
            OnInitRuntimeTree(_roomoId, belongid, startId, _rootId, datasupport, _clock, targetid);
        }
        
        public void OnInitRuntimeTree(string _roomoId, uint belongid, uint startId, long _rootId, NP_DataSupportor datasupport, Clock _clock, uint targetid = 0, bool isServer = true, INPRuntimeEntityResolver resolver = null)
        {
            RoomId = _roomoId;
            BelongToUnit = belongid;
            StartToUnit = startId;
            BelongNP_DataSupportor = datasupport;
            m_clock = _clock;
            RunTimeTreeId = _rootId;
            TargetToUnit = targetid;
            Context = new NPRuntimeContext(isServer, belongid, startId, targetid, log, resolver);
        }
        

     
        
        public Dictionary<Type, List<NP_ClassForStoreAction>> DicStoreAction = new Dictionary<Type, List<NP_ClassForStoreAction>>();

        public void AddClassForStoreAction<T>(T inst) where T : NP_ClassForStoreAction
        {
            if (!DicStoreAction.ContainsKey(typeof(T)))
            {
                DicStoreAction.Add(typeof(T), new List<NP_ClassForStoreAction>());
            }
            
            DicStoreAction[typeof(T)].Add(inst);
        }
    }
}
