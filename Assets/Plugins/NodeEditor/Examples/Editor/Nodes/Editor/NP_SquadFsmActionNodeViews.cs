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

                    var bbValueContainer = new VisualElement();
                    bbValueContainer.style.marginTop = 2;
                    RebuildBbValueUI(bbValueContainer, data.NPBalckBoardRelationData.NP_BBValue, () =>
                    {
                        owner.graph.NotifyNodeChanged(nodeTarget);
                    });

                    var popup = new PopupField<string>("BB Key", bbKeys, data.NPBalckBoardRelationData.BBKey);
                    popup.RegisterValueChangedCallback(evt =>
                    {
                        data.NPBalckBoardRelationData.BBKey = evt.newValue;
                        if (mgr.BBValues.TryGetValue(evt.newValue, out var bbVal) && bbVal != null)
                        {
                            data.NPBalckBoardRelationData.NP_BBValue = bbVal.DeepCopy();
                            data.NPBalckBoardRelationData.NP_BBValueType = data.NPBalckBoardRelationData.NP_BBValue.NP_BBValueType.ToString();
                            RebuildBbValueUI(bbValueContainer, data.NPBalckBoardRelationData.NP_BBValue, () =>
                            {
                                owner.graph.NotifyNodeChanged(nodeTarget);
                            });
                        }
                        owner.graph.NotifyNodeChanged(nodeTarget);
                    });
                    controlsContainer.Add(popup);

                    controlsContainer.Add(bbValueContainer);
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

        private static void RebuildBbValueUI(VisualElement container, ANP_BBValue bbValue, System.Action onChanged)
        {
            container.Clear();
            if (bbValue == null)
            {
                container.Add(new Label("BB Value: (null)"));
                return;
            }

            var valueField = bbValue.GetType().GetField("Value", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (valueField == null)
            {
                container.Add(new Label("BB Value: (uneditable)"));
                return;
            }

            object cur = valueField.GetValue(bbValue);
            var fieldType = valueField.FieldType;

            if (fieldType == typeof(string))
            {
                var field = new TextField("BB Value") { value = cur as string ?? string.Empty };
                field.RegisterValueChangedCallback(evt =>
                {
                    valueField.SetValue(bbValue, evt.newValue);
                    onChanged?.Invoke();
                });
                container.Add(field);
                return;
            }

            if (fieldType == typeof(int))
            {
                var field = new IntegerField("BB Value") { value = cur is int i ? i : 0 };
                field.RegisterValueChangedCallback(evt =>
                {
                    valueField.SetValue(bbValue, evt.newValue);
                    onChanged?.Invoke();
                });
                container.Add(field);
                return;
            }

            if (fieldType == typeof(long))
            {
                var field = new LongField("BB Value") { value = cur is long l ? l : 0L };
                field.RegisterValueChangedCallback(evt =>
                {
                    valueField.SetValue(bbValue, evt.newValue);
                    onChanged?.Invoke();
                });
                container.Add(field);
                return;
            }

            if (fieldType == typeof(uint))
            {
                var field = new LongField("BB Value") { value = cur is uint ui ? ui : 0 };
                field.RegisterValueChangedCallback(evt =>
                {
                    long val = evt.newValue;
                    if (val < 0) val = 0;
                    if (val > uint.MaxValue) val = uint.MaxValue;
                    valueField.SetValue(bbValue, (uint)val);
                    onChanged?.Invoke();
                });
                container.Add(field);
                return;
            }

            if (fieldType == typeof(float))
            {
                var field = new FloatField("BB Value") { value = cur is float f ? f : 0f };
                field.RegisterValueChangedCallback(evt =>
                {
                    valueField.SetValue(bbValue, evt.newValue);
                    onChanged?.Invoke();
                });
                container.Add(field);
                return;
            }

            if (fieldType == typeof(bool))
            {
                var field = new Toggle("BB Value") { value = cur is bool b && b };
                field.RegisterValueChangedCallback(evt =>
                {
                    valueField.SetValue(bbValue, evt.newValue);
                    onChanged?.Invoke();
                });
                container.Add(field);
                return;
            }

            if (fieldType == typeof(UnityEngine.Vector3))
            {
                var field = new Vector3Field("BB Value") { value = cur is UnityEngine.Vector3 v ? v : UnityEngine.Vector3.zero };
                field.RegisterValueChangedCallback(evt =>
                {
                    valueField.SetValue(bbValue, evt.newValue);
                    onChanged?.Invoke();
                });
                container.Add(field);
                return;
            }

            if (fieldType == typeof(eBuffBindAnimStackState))
            {
                var field = new EnumField("BB Value", cur is eBuffBindAnimStackState e ? e : eBuffBindAnimStackState.NullStateID);
                field.RegisterValueChangedCallback(evt =>
                {
                    valueField.SetValue(bbValue, (eBuffBindAnimStackState)evt.newValue);
                    onChanged?.Invoke();
                });
                container.Add(field);
                return;
            }

            if (fieldType == typeof(eMPNetPosition))
            {
                var field = new EnumField("BB Value", cur is eMPNetPosition e ? e : eMPNetPosition.eNULL);
                field.RegisterValueChangedCallback(evt =>
                {
                    valueField.SetValue(bbValue, (eMPNetPosition)evt.newValue);
                    onChanged?.Invoke();
                });
                container.Add(field);
                return;
            }

            container.Add(new Label($"BB Value: (unsupported type: {fieldType.Name})"));
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
