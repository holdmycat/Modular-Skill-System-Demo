using System.IO;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using UnityEngine;

namespace Plugins.NodeEditor
{
    public class SlgSquadBehavourGraph : BaseNpDataBehavourGraph
    {
        [Header("此被动武器技能树数据载体(客户端)")]
        public NP_DataSupportor _squadBehavourGraphdDataSupportor = new NP_DataSupportor();
        
        /// <summary>
        /// 自动配置当前图所有数据（结点，黑板）
        /// </summary>
        [ContextMenu("自动配置所有结点数据(客户端)")]
        public override  void BtnAutoSetCanvasDatas() 
         {
             var supporter = _squadBehavourGraphdDataSupportor;
             
            OnGraphEnable();
            
            PrepareAllNodeData();
            _configPath = ConstData.PATH_SLGSQUADBEHAVOUR;
            if (supporter is { } dataSupportor)
            {
                AutoSetCanvasDatas(dataSupportor.NpDataSupportorBase);
                AutoSetSquadBehavour_NodeData(_squadBehavourGraphdDataSupportor);
            }
         }

        [ContextMenu("保存行为树信息为二进制文件")]
        public override void Save()
        {
            if (string.IsNullOrEmpty(_configPath) || string.IsNullOrEmpty(_configPath) ||
                string.IsNullOrEmpty(_name))
            {
                Debug.LogError($"保存路径或文件名不能为空，请检查配置");
                return;
            }

            var supporter = _squadBehavourGraphdDataSupportor;
            

            using (FileStream file = File.Create($"{_configPath}/{this._name}.bytes"))
            {
                BsonSerializer.Serialize(new BsonBinaryWriter(file), supporter);
            }

            Debug.Log($"保存 {_configPath}/{this._name}.bytes {_configPath}/{this._name}.bytes 成功");
        }

        [ContextMenu("测试反序列化")]
        public override void TestDe()
        {
            try
            {
                var supporter = _squadBehavourGraphdDataSupportor;
                
                byte[] mClientfile = File.ReadAllBytes($"{_configPath}/{this._name}.bytes");
                if (mClientfile.Length == 0) Debug.Log("没有读取到文件");
                
                if (supporter is { } dataSupportor)
                {
                    var activeSkillBase = BsonSerializer.Deserialize<NP_DataSupportor>(mClientfile);
                    Debug.Log($"反序列化主动技能 {_configPath}/{this._name}.bytes 成功");
                }
                
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                throw;
            }
        }
        
        [ContextMenu("一键配置")]
        public override void OneKeySet()
        {
           
            BtnAutoSetCanvasDatas();
            Save();
            TestDe();
        }
        
        private void AutoSetSquadBehavour_NodeData(NP_DataSupportor npDataSupportor)
        {
            var skillNodeId = long.Parse(_name);

            npDataSupportor.NpDataSupportorBase.NPBehaveTreeDataId = skillNodeId;
            
            npDataSupportor.Ids.TryAdd(ConstData.BELONGTOSKILLID, skillNodeId);
            
            _npBlackBoardDataManager.Ids.TryAdd(ConstData.BELONGTOSKILLID, skillNodeId);
            
        }
        
    }
}
