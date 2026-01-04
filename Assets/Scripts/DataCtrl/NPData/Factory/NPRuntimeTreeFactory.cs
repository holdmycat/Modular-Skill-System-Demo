using System;
using System.Collections.Generic;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.DataCtrl
{
    public struct NPRuntimeTreeRequest
    {
        public uint BelongToUnit;
        public uint StartToUnit;
        public uint TargetToUnit;
        public long RootId;
        public RuntimeTreeType TreeType;
        public eMPNetPosition NetPosition;
    }

    public interface INPRuntimeTreeFactory
    {
        NP_RuntimeTree Create(NPRuntimeTreeRequest request);
    }

    /// <summary>
    /// Builds NP_RuntimeTree instances and wires their nodes/blackboard using provided data.
    /// </summary>
    public class NPRuntimeTreeFactory : INPRuntimeTreeFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NPRuntimeTreeFactory));

        private readonly INPRuntimeTreeDataProvider _dataProvider;
        private readonly Clock _serverClock;
        private readonly Clock _clientClock;
        private readonly IInstantiator _instantiator;
        private readonly INPRuntimeEntityResolver _resolver;

        private enum NodeType
        {
            Task,
            Decorator,
            Composite
        }

        private static readonly Dictionary<Type, NodeType> NodeRegistry = new Dictionary<Type, NodeType>
        {
            {typeof(NP_RootNodeData), NodeType.Decorator},
            {typeof(NP_ParallelNodeData), NodeType.Composite},
            {typeof(NP_DynamicParallelNodeData), NodeType.Composite},
            {typeof(NP_SelectorNodeData), NodeType.Composite},
            {typeof(NP_SequenceNodeData), NodeType.Composite},
            {typeof(NP_BlackboardConditionNodeData), NodeType.Decorator},
            {typeof(NP_RepeaterNodeData), NodeType.Decorator},
            {typeof(NP_TimeRepeaterNodeData), NodeType.Decorator},
            {typeof(NP_ServiceNodeData), NodeType.Decorator},
            {typeof(NP_ActionNodeData), NodeType.Task},
            {typeof(NP_WaitNodeData), NodeType.Task},
            {typeof(NP_WaitUntilStoppedData), NodeType.Task},
        };

        [Inject]
        public NPRuntimeTreeFactory(
            INPRuntimeTreeDataProvider dataProvider,
            [Inject(Id = ClockIds.Server)] Clock serverClock,
            [Inject(Id = ClockIds.Client)] Clock clientClock,
            INPRuntimeEntityResolver resolver,
            IInstantiator instantiator)
        {
            _dataProvider = dataProvider;
            _serverClock = serverClock;
            _clientClock = clientClock;
            _resolver = resolver;
            _instantiator = instantiator;
        }

        public NP_RuntimeTree Create(NPRuntimeTreeRequest request)
        {
            var data = _dataProvider.GetData(request.RootId, request.TreeType);
            if (data == null)
            {
                throw new InvalidOperationException($"[NPRuntimeTreeFactory] No NP_DataSupportor for rootId:{request.RootId}, type:{request.TreeType}");
            }

            var runtimeTree = _instantiator.Instantiate<NP_RuntimeTree>();

            var clock = request.NetPosition == eMPNetPosition.eServerOnly ? _serverClock : _clientClock;
            
            runtimeTree.OnInitRuntimeTree( 
                request.BelongToUnit, 
                request.StartToUnit, 
                request.RootId, 
                data, 
                clock, 
                request.TargetToUnit, 
                request.NetPosition,
                _resolver);

            BuildTree(runtimeTree, data);

            return runtimeTree;
        }

        private static void BuildTree(NP_RuntimeTree runtimeTree, NP_DataSupportor supportor)
        {
            var data = supportor.NpDataSupportorBase;
            var netId = runtimeTree.BelongToUnit;
            var rootId = data.NPBehaveTreeDataId;

            foreach (var kvp in data.NP_DataSupportorDic)
            {
                var nodeData = kvp.Value;
                if (!NodeRegistry.TryGetValue(nodeData.GetType(), out var nodeType))
                {
                    Log.Warn($"[NPRuntimeTreeFactory] Unregistered node type: {nodeData.GetType().Name}");
                    continue;
                }

                try
                {
                    switch (nodeType)
                    {
                        case NodeType.Task:
                            nodeData.CreateTask(netId, runtimeTree);
                            if (nodeData is NP_ActionNodeData actionNodeData)
                            {
                                actionNodeData.NpClassForStoreAction.SetContext(runtimeTree.Context);
                            }
                            break;
                        case NodeType.Decorator:
                            nodeData.CreateDecoratorNode(netId, runtimeTree,
                                data.NP_DataSupportorDic[nodeData.LinkedIds[0]].NP_GetNode());
                            break;
                        case NodeType.Composite:
                            var children = new List<Node>();
                            foreach (var linkedId in nodeData.LinkedIds)
                            {
                                children.Add(data.NP_DataSupportorDic[linkedId].NP_GetNode());
                            }
                            nodeData.CreateComposite(children.ToArray());
                            break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"{e} ----- {nodeData.NodeDes}");
                    throw;
                }
            }

            runtimeTree.SetRootNode(data.NP_DataSupportorDic[rootId].NP_GetNode() as Root);

            var bbValues = runtimeTree.GetBlackboard().GetDatas();
            foreach (var kvp in data.NP_BBValueManager)
            {
                bbValues[kvp.Key] = kvp.Value;
            }
        }
    }
}
