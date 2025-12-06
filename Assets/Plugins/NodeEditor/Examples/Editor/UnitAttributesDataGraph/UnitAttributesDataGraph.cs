using System.IO;
using GraphProcessor;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Ebonor.DataCtrl;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Plugins.NodeEditor
{
    public class UnitAttributesDataGraph: BaseGraph
    {
        public string Name = "HeroData";
        
        public string SavePath;
        
        
        protected override void OnGraphEnabled()
        {
            base.OnGraphEnabled();
            Debug.LogFormat("UnitAttributesDataGraph-OnGraphEnabled, node count:{0}", nodes.Count);
           
            foreach (var variable in nodes)
            {
                if (variable is UnitAttributesNodeBase node)
                {
                    var skillData = node.UnitAttributesData_GetNodeData();
                    skillData.OnInit();
                }
            }
            
        }
        
        /// <summary>
        /// Node data container used to collect all data in this ScriptableObject.
        /// </summary>
        public UnitAttributesDataSupportor m_TestDic = new UnitAttributesDataSupportor();
        
        public override void BtnAutoSetCanvasDatas()
        {
            m_TestDic.UnitAttributesDataSupportorDic.Clear();
            foreach (var node in nodes)
            {
                if (node is UnitAttributesNodeBase unitAttributesNodeBase)
                {
                    var value = unitAttributesNodeBase.UnitAttributesData_GetNodeData();
                    value.typeStr = node.GetType().ToString();
                    m_TestDic.UnitAttributesDataSupportorDic.Add(unitAttributesNodeBase.UnitAttributesData_GetNodeData().UnitDataNodeId,
                        value);
                }
            }
        }
        
        public override void Save()
        {
            AttributesNodeDataSerializerRegister.RegisterClassMaps();
            using (FileStream file = File.Create($"{SavePath}/{this.Name}.bytes"))
            {
                using (var bsonWriter = new BsonBinaryWriter(file))
                {
                    BsonSerializer.Serialize(bsonWriter, m_TestDic);
                }
            }
            
            Debug.Log("Save succeeded");
#if UNITY_EDITOR
            // Persist graph asset changes so edits survive script recompiles.
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        
        public override void OneKeySet()
        {
            BtnAutoSetCanvasDatas();
            Save();
        }
        
    }
}
