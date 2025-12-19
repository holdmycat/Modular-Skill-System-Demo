//------------------------------------------------------------
// File: SlgUnitAttributesDataGraph.cs
// Purpose: Graph asset for SLG unit attributes.
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
    public class SlgUnitAttributesDataGraph : BaseGraph
    {
        public string Name = "SlgUnitData";
        public string SavePath;

        public SlgUnitAttributesDataSupportor DataSupportor = new SlgUnitAttributesDataSupportor();

        protected override void OnGraphEnabled()
        {
            base.OnGraphEnabled();
            Debug.LogFormat("SlgUnitAttributesDataGraph-OnGraphEnabled, node count:{0}", nodes.Count);

            foreach (var variable in nodes)
            {
                if (variable is SlgUnitAttributesNodeBase node)
                {
                    var data = node.SlgAttributesData_GetNodeData();
                    data?.GenerateRoleIdFromData();
                }
            }
        }

        public override void BtnAutoSetCanvasDatas()
        {
            DataSupportor.SlgUnitAttributesDic.Clear();
            foreach (var node in nodes)
            {
                if (node is SlgUnitAttributesNodeBase slgNode)
                {
                    var value = slgNode.SlgAttributesData_GetNodeData();
                    if (value == null) continue;
                    value.typeStr = node.GetType().ToString();
                    DataSupportor.SlgUnitAttributesDic[value.UnitDataNodeId] = value;
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
                    BsonSerializer.Serialize(bsonWriter, DataSupportor);
                }
            }

            Debug.Log("SLG Save succeeded");
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
