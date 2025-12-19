//------------------------------------------------------------
// File: SlgUnitSquadAttributesDataGraph.cs
// Purpose: Graph asset for SLG squad attributes (separate from unit/兵种属性表).
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
    public class SlgUnitSquadAttributesDataGraph : BaseGraph
    {
        public string Name = "SlgSquadData";
        public string SavePath;

        public SlgUnitSquadAttributesDataSupportor DataSupportor = new SlgUnitSquadAttributesDataSupportor();

        protected override void OnGraphEnabled()
        {
            base.OnGraphEnabled();
            Debug.LogFormat("SlgUnitSquadAttributesDataGraph-OnGraphEnabled, node count:{0}", nodes.Count);

            foreach (var variable in nodes)
            {
                if (variable is SlgUnitSquadAttributesNodeBase node)
                {
                    var data = node.SlgSquadAttributesData_GetNodeData();
                    data?.GenerateRoleIdFromData();
                }
            }
        }

        public override void BtnAutoSetCanvasDatas()
        {
            DataSupportor.SquadAttributesDic.Clear();
            foreach (var node in nodes)
            {
                if (node is SlgUnitSquadAttributesNodeBase slgNode)
                {
                    var value = slgNode.SlgSquadAttributesData_GetNodeData();
                    if (value == null) continue;
                    value.typeStr = node.GetType().ToString();
                    DataSupportor.SquadAttributesDic[value.SquadDataNodeId] = value;
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

            Debug.Log("SLG Squad Save succeeded");
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
