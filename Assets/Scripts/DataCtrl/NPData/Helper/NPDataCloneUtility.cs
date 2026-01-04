using System.Collections.Generic;
using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Manual clone helpers for NP data structures to avoid mutating cached source data.
    /// </summary>
    public static class NPDataCloneUtility
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NPDataCloneUtility));

        public static NP_DataSupportor CloneSupportor(NP_DataSupportor source)
        {
            if (source == null) return null;

            var clone = new NP_DataSupportor
            {
                Ids = CopyIds(source.Ids),
                NpDataSupportorBase = CloneSupportorBase(source.NpDataSupportorBase)
            };

            return clone;
        }

        private static NP_DataSupportorBase CloneSupportorBase(NP_DataSupportorBase source)
        {
            if (source == null) return null;

            var clone = new NP_DataSupportorBase
            {
                NPBehaveTreeDataId = source.NPBehaveTreeDataId,
                NP_DataSupportorDic = new Dictionary<long, NP_NodeDataBase>(source.NP_DataSupportorDic.Count),
                NP_BBValueManager = new Dictionary<string, ANP_BBValue>(source.NP_BBValueManager.Count)
            };

            foreach (var kvp in source.NP_DataSupportorDic)
            {
                var nodeCopy = CloneNode(kvp.Value);
                if (nodeCopy != null)
                {
                    // Preserve original id as key
                    clone.NP_DataSupportorDic[kvp.Key] = nodeCopy;
                }
            }

            foreach (var kvp in source.NP_BBValueManager)
            {
                var bbCopy = CloneBbValue(kvp.Value);
                if (bbCopy != null)
                {
                    clone.NP_BBValueManager[kvp.Key] = bbCopy;
                }
            }

            return clone;
        }

        private static Dictionary<string, long> CopyIds(Dictionary<string, long> ids)
        {
            if (ids == null) return new Dictionary<string, long>();
            return new Dictionary<string, long>(ids);
        }

        private static NP_NodeDataBase CloneNode(NP_NodeDataBase node)
        {
            if (node == null) return null;

            // Prioritize manual Clone if implemented (which all should eventually)
            // Fallback to DeepCopyFast only if necessary (though current task implies we move fully to Clone)
            var copy = node.Clone();
            
            // Fallback for safety if Clone() returns shallow referring to same instance (shouldn't happen with proper override)
            if (ReferenceEquals(copy, node))
            {
                log.Warn($"[NPDataCloneUtility] {node.GetType().Name} Clone() returned same instance; please implement deep clone.");
                copy = node.DeepCopyFast() ?? node.DeepCopy();
            }

            return copy;
        }

        private static ANP_BBValue CloneBbValue(ANP_BBValue value)
        {
            if (value == null) return null;
            return value.Clone();
        }
    }
}
