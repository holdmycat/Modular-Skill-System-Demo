using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

using UnityEngine;

namespace Ebonor.DataCtrl
{
[System.Serializable]
[BsonDeserializerRegister]
[BsonDiscriminator(RootClass = true)]
public abstract class NP_NodeDataBase
    {
        /// <summary>
        /// 此结点ID
        /// </summary>
        public long id;

        /// <summary>
        /// 与此结点相连的ID
        /// </summary>
        public List<long> LinkedIds = new List<long>();

        [TextArea(2, 2)]
        [BsonIgnore]
        public string NodeDes;

        /// <summary>
        /// 获取结点
        /// </summary>
        /// <returns></returns>
        public abstract Node NP_GetNode();

        /// <summary>
        /// 创建组合结点
        /// </summary>
        /// <returns></returns>
        public virtual Composite CreateComposite(Node[] nodes)
        {
            return null;
        }

        /// <summary>
        /// 创建装饰结点
        /// </summary>
        /// <param name="unitId">行为树归属的Unit</param>
        /// <param name="runtimeTree">运行时归属的行为树</param>
        /// <param name="node">所装饰的结点</param>
        /// <returns></returns>
        public virtual Decorator CreateDecoratorNode(uint unit, NP_RuntimeTree runtimeTree, Node node)
        {
            return null;
        }

      
        
        /// <summary>
        /// 创建任务节点
        /// </summary>
        /// <param name="unitId">行为树归属的Unit</param>
        /// <param name="runtimeTree">运行时归属的行为树</param>
        /// <returns></returns>
        public virtual Task CreateTask(uint unit, NP_RuntimeTree runtimeTree)
        {
            return null;
        }
        
        /// <summary>
        /// 创建装饰结点
        /// </summary>
        /// <param name="unitId">行为树归属的Unit</param>
        /// <param name="runtimeTree">运行时归属的行为树</param>
        /// <param name="node">所装饰的结点</param>
        /// <returns></returns>
        public virtual Decorator CreateNGDecoratorNode<T>(string unit, T runtimeTree, Clock clock, Node node) where T : MonoBehaviour
        {
            return null;
        }

        /// <summary>
        /// 创建任务节点
        /// </summary>
        /// <param name="unitId">行为树归属的Unit</param>
        /// <param name="runtimeTree">运行时归属的行为树</param>
        /// <returns></returns>
        public virtual Task CreateNGTask<T>(string uId, T runtimeTree) where T : MonoBehaviour
        {
            return null;
        }
    }
}
