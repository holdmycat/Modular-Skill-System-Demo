
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.GamePlay
{
    
    
    [System.Serializable]
    public class NP_PrintDebugLog : NP_ClassForStoreAction
    {
        static readonly ILog log = LogManager.GetLogger(typeof(NP_PrintDebugLog));

        [Header("打印内容")]
        public string DebugContent;

        
        public override System.Action GetActionToBeDone()
        {
            this.Action = this.AddBuffGroupToSpecifiedUnit;
            return this.Action;
        }

        void AddBuffGroupToSpecifiedUnit()
        {
            #if UNITY_EDITOR
            if (string.IsNullOrEmpty(DebugContent))
                return;
            log.DebugFormat("打印日志：{0}", this.DebugContent);
            #endif
        }
    }
}
