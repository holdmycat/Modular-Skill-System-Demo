using Ebonor.DataCtrl;
using Ebonor.GamePlay;
using GraphProcessor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Plugins.NodeEditor
{
    /// <summary>
    /// Inline editable fields for Squad FSM actions directly on the node.
    /// </summary>
    [NodeCustomEditor(typeof(NP_ChangeSquadStackStateActionNode))]
    public class NP_ChangeSquadStackStateActionNodeView : NP_NodeView
    {
        public override void Enable()
        {
            base.Enable();

            if (nodeTarget is NP_ChangeSquadStackStateActionNode actionNode &&
                actionNode.NP_ActionNodeData?.NpClassForStoreAction is NP_ChangeSquadStackStateAction action)
            {
                var enumField = new EnumField("目标状态", action.TargetState.Value);
                enumField.RegisterValueChangedCallback(evt =>
                {
                    action.TargetState.Value = (eBuffBindAnimStackState)evt.newValue;
                    owner.graph.NotifyNodeChanged(nodeTarget);
                });
                controlsContainer.Add(enumField);
            }
        }
    }

    [NodeCustomEditor(typeof(NP_RemoveSquadStackStateActionNode))]
    public class NP_RemoveSquadStackStateActionNodeView : NP_NodeView
    {
        public override void Enable()
        {
            base.Enable();

            if (nodeTarget is NP_RemoveSquadStackStateActionNode actionNode &&
                actionNode.NP_ActionNodeData?.NpClassForStoreAction is NP_RemoveSquadStackStateAction action)
            {
                var enumField = new EnumField("移除状态", action.StateToRemove.Value);
                enumField.RegisterValueChangedCallback(evt =>
                {
                    action.StateToRemove.Value = (eBuffBindAnimStackState)evt.newValue;
                    owner.graph.NotifyNodeChanged(nodeTarget);
                });
                controlsContainer.Add(enumField);
            }
        }
    }

    [NodeCustomEditor(typeof(NP_WaitActionNode))]
    public class NP_WaitActionNodeView : NP_NodeView
    {
        public override void Enable()
        {
            base.Enable();

            if (nodeTarget is NP_WaitActionNode waitNode &&
                waitNode.NP_ActionNodeData?.NpClassForStoreAction is NP_WaitAction action)
            {
                var field = new FloatField("等待秒数") { value = action.WaitDuration };
                field.RegisterValueChangedCallback(evt =>
                {
                    action.WaitDuration = evt.newValue;
                    owner.graph.NotifyNodeChanged(nodeTarget);
                });
                controlsContainer.Add(field);
            }
        }
    }

    [NodeCustomEditor(typeof(NP_PrintDebugLogNode))]
    public class NP_PrintDebugLogNodeView : NP_NodeView
    {
        public override void Enable()
        {
            base.Enable();

            if (nodeTarget is NP_PrintDebugLogNode logNode &&
                logNode.NP_ActionNodeData?.NpClassForStoreAction is NP_PrintDebugLog action)
            {
                var field = new TextField("日志内容") { value = action.DebugContent };
                field.RegisterValueChangedCallback(evt =>
                {
                    action.DebugContent = evt.newValue;
                    owner.graph.NotifyNodeChanged(nodeTarget);
                });
                controlsContainer.Add(field);
            }
        }
    }

    [NodeCustomEditor(typeof(NP_BlackboardConditionNode))]
    public class NP_BlackboardConditionNodeView : NP_NodeView
    {
        public override void Enable()
        {
            base.Enable();

            if (nodeTarget is NP_BlackboardConditionNode condNode &&
                condNode.NP_BlackboardConditionNodeData != null)
            {
                var data = condNode.NP_BlackboardConditionNodeData;

                var mgr = NP_BlackBoardDataManager.CurrentEditedNP_BlackBoardDataManager;
                var bbKeys = mgr?.BBValues != null ? new List<string>(mgr.BBValues.Keys) : null;

                if (bbKeys != null && bbKeys.Count > 0)
                {
                    // 默认值保证存在
                    if (!bbKeys.Contains(data.NPBalckBoardRelationData.BBKey))
                    {
                        data.NPBalckBoardRelationData.BBKey = bbKeys[0];
                    }

                    var popup = new PopupField<string>("BB Key", bbKeys, data.NPBalckBoardRelationData.BBKey);
                    popup.RegisterValueChangedCallback(evt =>
                    {
                        data.NPBalckBoardRelationData.BBKey = evt.newValue;
                        if (mgr.BBValues.TryGetValue(evt.newValue, out var bbVal) && bbVal != null)
                        {
                            data.NPBalckBoardRelationData.NP_BBValue = bbVal.DeepCopy();
                            data.NPBalckBoardRelationData.NP_BBValueType = data.NPBalckBoardRelationData.NP_BBValue.NP_BBValueType.ToString();
                        }
                        owner.graph.NotifyNodeChanged(nodeTarget);
                    });
                    controlsContainer.Add(popup);
                }
                else
                {
                    // 回落为手动输入
                    var bbKeyField = new TextField("BB Key") { value = data.NPBalckBoardRelationData.BBKey };
                    bbKeyField.RegisterValueChangedCallback(evt =>
                    {
                        data.NPBalckBoardRelationData.BBKey = evt.newValue;
                        owner.graph.NotifyNodeChanged(nodeTarget);
                    });
                    controlsContainer.Add(bbKeyField);
                }

                var opeField = new EnumField("比较符", data.Ope);
                opeField.RegisterValueChangedCallback(evt =>
                {
                    data.Ope = (Operator)evt.newValue;
                    owner.graph.NotifyNodeChanged(nodeTarget);
                });
                controlsContainer.Add(opeField);

                var stopField = new EnumField("Stop", data.Stop);
                stopField.RegisterValueChangedCallback(evt =>
                {
                    data.Stop = (Stops)evt.newValue;
                    owner.graph.NotifyNodeChanged(nodeTarget);
                });
                controlsContainer.Add(stopField);
            }
        }
    }

    [NodeCustomEditor(typeof(NP_SelectorNode))]
    public class NP_SelectorNodeView : NP_NodeView
    {
        public override void Enable()
        {
            base.Enable();
            // 选择器当前无需额外字段，保留描述编辑（来自基类）。
        }
    }
}
