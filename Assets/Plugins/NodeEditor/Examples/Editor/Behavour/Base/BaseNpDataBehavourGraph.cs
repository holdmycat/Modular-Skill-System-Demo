using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using GraphProcessor;
using UnityEditor;
using UnityEngine;

namespace Plugins.NodeEditor
{
    /// <summary>
    /// 对外数据:公有类型
    /// </summary>
    public abstract partial class BaseNpDataBehavourGraph : BaseGraph
    {
        [Header("本Canvas所有数据整理部分 (ReadOnly)")] 
        [Tooltip("保存文件名(技能树ID) - Auto Set")]
        [SerializeField]
        public string _name;
        
        [Tooltip("保存路径 - ReadOnly")]
        [SerializeField]
        protected string _configPath = "Assets/AssetPackages/Res/BattleGraphData/SkillGraphData";
        
        /// <summary>
        /// 黑板数据管理器
        /// </summary>
        protected NP_BlackBoardDataManager _npBlackBoardDataManager = new NP_BlackBoardDataManager();

        public NP_BlackBoardDataManager NpBlackBoardDataManager => _npBlackBoardDataManager;
        
    }

    
    /// <summary>
    /// 私有数据 + 私有接口
    /// </summary>
    public abstract partial class BaseNpDataBehavourGraph : BaseGraph
    {
         //当前Canvas所有NP_Node
        private readonly List<NP_NodeBase> m_AllNodes = new List<NP_NodeBase>();
        
        /// <summary>
        /// 自动配置所有行为树结点
        /// </summary>
        /// <param name="npDataSupportorBase">自定义的继承于NP_DataSupportorBase的数据体</param>
        private void AutoSetNP_NodeData(NP_DataSupportorBase npDataSupportorBase)
        {
            if (npDataSupportorBase == null)
            {
                return;
            }
            
            npDataSupportorBase.NPBehaveTreeDataId = 0;
            npDataSupportorBase.NP_DataSupportorDic.Clear();

            //当前Canvas所有NP_Node
            List<NP_NodeBase> mValidNodes = new List<NP_NodeBase>();

            foreach (var node in m_AllNodes)
            {
                if (node is { } mNode)
                {
                    mValidNodes.Add(mNode);
                }
            }

            if (!long.TryParse(_name, out npDataSupportorBase.NPBehaveTreeDataId))
            {
                Debug.LogError("Name is not long, Name:" + _name);
                return;
            }
            
            if (npDataSupportorBase.NPBehaveTreeDataId == 0)
            {
                //设置为根结点Id
                npDataSupportorBase.NPBehaveTreeDataId = mValidNodes[^1].NP_GetNodeData().id;
                Debug.LogError(
                    $"注意，名为{this._name}的Graph首次导出，或者未在配置表中找到Id为{_name}的数据行，行为树Id被设置为{npDataSupportorBase.NPBehaveTreeDataId}，请前往Excel表中进行添加，然后导出Excel");
            }
            else
            {
                if (mValidNodes.Count <= 0)
                    return;
                mValidNodes[^1].NP_GetNodeData().id = npDataSupportorBase.NPBehaveTreeDataId;
            }

            foreach (var node in mValidNodes)
            {
                //获取结点对应的NPData
                NP_NodeDataBase mNodeData = node.NP_GetNodeData();
                if (mNodeData.LinkedIds == null)
                {
                    mNodeData.LinkedIds = new List<long>();
                }

                mNodeData.LinkedIds.Clear();

                //出结点连接的Nodes
                List<NP_NodeBase> theNodesConnectedToOutNode = new List<NP_NodeBase>();

                foreach (var outputNode in node.GetOutputNodes())
                {
                    if (mValidNodes.Contains(outputNode))
                    {
                        theNodesConnectedToOutNode.Add(outputNode as NP_NodeBase);
                    }
                }

                //对所连接的节点们进行排序
                theNodesConnectedToOutNode.Sort((x, y) => x.position.x.CompareTo(y.position.x));

                //配置连接的Id，运行时实时构建行为树
                foreach (var npNodeBase in theNodesConnectedToOutNode)
                {
                    mNodeData.LinkedIds.Add(npNodeBase.NP_GetNodeData().id);
                }

                //将此结点数据写入字典
                npDataSupportorBase.NP_DataSupportorDic.Add(mNodeData.id, mNodeData);
            }
        }

        
        /// <summary>
        /// 自动配置黑板数据
        /// </summary>
        /// <param name="npDataSupportorBase">自定义的继承于NP_DataSupportorBase的数据体</param>
        private void AutoSetNP_BBDatas(NP_DataSupportorBase npDataSupportorBase)
        {
            npDataSupportorBase.NP_BBValueManager.Clear();
            //设置黑板数据
            foreach (var bbvalues in _npBlackBoardDataManager.BBValues)
            {
                npDataSupportorBase.NP_BBValueManager.Add(bbvalues.Key, bbvalues.Value);
            }
        }
        
        #region system :OnGraphEnabled/OnGraphDisabled
        protected override void OnGraphEnabled()
        {
            base.OnGraphEnabled();
            _name = name;
            Debug.LogFormat("NPBehaveGraph-OnGraphEnabled, node count:{0}", nodes.Count);
            onGraphChanges += GraphChangesCallback;
            // Ensure the static reference is set before any default injection.
            NP_BlackBoardHelper.SetCurrentBlackBoardDataManager(this);
            AddNetPosBb();
        }
        
        protected override void OnGraphDisabled()
        {
            Debug.LogFormat("NPBehaveGraph-OnGraphDisabled, node count:{0}", nodes.Count);
            //SkillAttributesNodeDataBase.OnDisableGlobalSkillAttributes();
            onGraphChanges -= GraphChangesCallback;
        }
        
        protected virtual void GraphChangesCallback(GraphChanges changes)
        {
          
        }
        #endregion


        [ContextMenu("一键配置")]
        public override void OneKeySet()
        {
#if UNITY_EDITOR
            RefreshExportedBinary();
#endif
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Force Unity to re-import the generated .bytes file so it is immediately usable without
        /// having to change focus away from the editor.
        /// </summary>
        private void RefreshExportedBinary()
        {
            if (string.IsNullOrEmpty(_configPath) || string.IsNullOrEmpty(_name)) return;

            // Ensure we only try to import assets that live under the project Assets folder.
            if (_configPath.StartsWith("Assets"))
            {
                var assetPath = $"{_configPath}/{_name}.bytes";
                AssetDatabase.ImportAsset(assetPath);
                Debug.Log($"反序列化主动技能 {_configPath}/{this._name}.bytes 刷新成功");
            }

            AssetDatabase.Refresh();
        }
#endif
        
    }
    
    
    /// <summary>
    /// 子类接口:保护类型
    /// </summary>
    public abstract partial class BaseNpDataBehavourGraph : BaseGraph
    {
        
        protected readonly StringBuilder mStrBld = new StringBuilder();
        
        // 准备所有节点的数据
        protected void PrepareAllNodeData()
        {
            m_AllNodes.Clear();
            
            foreach (var node in this.nodes)
            {
                if (node is NP_NodeBase mNode)
                {
                    m_AllNodes.Add(mNode);
                }
            }
            
            //排序
            m_AllNodes.Sort((x, y) => -x.position.y.CompareTo(y.position.y));

            //配置每个节点Id
            foreach (var node in m_AllNodes)
            {
                node.NP_GetNodeData().id = IdGenerater.Instance.GenerateId();
            }
        }


        protected virtual NP_BlackBoardDataManager AddNetPosBb()
        {
            
            return _npBlackBoardDataManager;
        }
        
        
        /// <summary>
        /// 连接树节点，设置树黑板数据
        /// </summary>
        /// <param name="isServer"></param>
        protected void AutoSetCanvasDatas(NP_DataSupportorBase supporterBase)
        {
            //AddNetPosBb();
            this.AutoSetNP_NodeData(supporterBase);
            this.AutoSetNP_BBDatas(supporterBase);
        }
    }
}
