//------------------------------------------------------------
// File: NP_NodeDataBase.cs
// Created: 2025-12-01
// Purpose: Base class for behavior tree node data and factory helpers.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    [BsonDeserializerRegister]
    public abstract class NP_NodeDataBase
    {
        [Tooltip("Unique node identifier.")]
        [HideInInspector]
        public long id;

        [Tooltip("IDs of nodes linked to this node.")]
        [HideInInspector]
        public List<long> LinkedIds = new List<long>();

        [Tooltip("Description for this node.")]
        [TextArea(2, 2)]
        [BsonIgnore]
        public string NodeDes;

        /// <summary>
        /// Get the runtime node instance.
        /// </summary>
        public abstract Node NP_GetNode();

        /// <summary>
        /// Create a composite node.
        /// </summary>
        public virtual Composite CreateComposite(Node[] nodes)
        {
            return null;
        }

        /// <summary>
        /// Create a decorator node.
        /// </summary>
        public virtual Decorator CreateDecoratorNode(uint unit, NP_RuntimeTree runtimeTree, Node node)
        {
            return null;
        }

      
        
        /// <summary>
        /// Create a task node.
        /// </summary>
        public virtual Task CreateTask(uint unit, NP_RuntimeTree runtimeTree)
        {
            return null;
        }
        
        /// <summary>
        /// Create a decorator node for NG systems.
        /// </summary>
        public virtual Decorator CreateNGDecoratorNode<T>(string unit, T runtimeTree, Clock clock, Node node) where T : MonoBehaviour
        {
            return null;
        }

        /// <summary>
        /// Create a task node for NG systems.
        /// </summary>
        public virtual Task CreateNGTask<T>(string uId, T runtimeTree) where T : MonoBehaviour
        {
            return null;
        }
    }
}
