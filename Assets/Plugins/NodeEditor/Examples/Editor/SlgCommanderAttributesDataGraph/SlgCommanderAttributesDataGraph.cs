//------------------------------------------------------------
// File: SlgCommanderAttributesDataGraph.cs
// Purpose: Graph asset for SLG Commander attributes.
//------------------------------------------------------------
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
    public class SlgCommanderAttributesDataGraph : BaseGraph
    {
        public string Name = "SlgCommanderData";
        public string SavePath;

        public SlgCommanderAttributesDataSupportor DataSupportor = new SlgCommanderAttributesDataSupportor();
        
        protected override void OnGraphEnabled()
        {
            base.OnGraphEnabled();
            Debug.LogFormat("SlgCommanderAttributesDataGraph-OnGraphEnabled, node count:{0}", nodes.Count);

            foreach (var variable in nodes)
            {
                if (variable is SlgCommanderAttributesNodeBase node)
                {
                    var data = node.CommanderAttributesData_GetNodeData();
                    data?.GenerateRoleIdFromData();
                }
            }
        }

        public override void BtnAutoSetCanvasDatas()
        {
            DataSupportor.CommanderAttributesDic.Clear();
            foreach (var node in nodes)
            {
                if (node is SlgCommanderAttributesNodeBase slgNode)
                {
                    var value = slgNode.CommanderAttributesData_GetNodeData();
                    if (value == null) continue;
                    value.typeStr = node.GetType().ToString();
                    DataSupportor.CommanderAttributesDic[value.UnitDataNodeId] = value;
                }
            }
        }

        public override void Save()
        {
            // Ensure ClassMaps registered
            AttributesNodeDataSerializerRegister.RegisterClassMaps();
            using (FileStream file = File.Create($"{SavePath}/{this.Name}.bytes"))
            {
                using (var bsonWriter = new BsonBinaryWriter(file))
                {
                    BsonSerializer.Serialize(bsonWriter, DataSupportor);
                }
            }

            Debug.Log("SLG Commander Save succeeded");
#if UNITY_EDITOR
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
