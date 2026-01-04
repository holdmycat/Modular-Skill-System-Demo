using System;
using Ebonor.Framework;
using UnityEngine;


namespace Ebonor.DataCtrl
{
    [System.Serializable]
    [BsonDeserializerRegister]
    public class NP_ClassForStoreAction
    {
        static readonly ILog log = LogManager.GetLogger(typeof(NP_ClassForStoreAction));
        
        
        public NP_ClassForStoreAction()
        {
        
        }

        public virtual NP_ClassForStoreAction Clone()
        {
            // Shallow Copy
            var clone = (NP_ClassForStoreAction)this.MemberwiseClone();
            
            // Clear Runtime Data
            clone.BelongToUnit = 0;
            clone.BelongtoRuntimeTree = null;
            clone.Context = null;
            clone.Action = null;
            clone.Func = null;
            clone.Func1 = null;
            clone.Func2 = null;
            
            return clone;
        }
        
        // 判断逻辑，控制是否显示
        private bool IsTargetOfTypeSpecificClass()
        {
            return GetType() != typeof(NP_ChangeBlackValueAction);
        }
        
        /// <summary>
        /// 归属的UnitID
        /// </summary>
        [HideInInspector] public uint BelongToUnit;

        /// <summary>
        /// 归属的运行时行为树实例
        /// </summary>
        [HideInInspector]
        public NP_RuntimeTree BelongtoRuntimeTree;

        /// <summary>
        /// Per-tree context (owner/target IDs, server/client flag).
        /// </summary>
        [HideInInspector]
        public NPRuntimeContext Context;
        
        public virtual void SetContext(NPRuntimeContext context)
        {
            Context = context;
        }
        
        /// <summary>
        /// 归属的运行时行为树实例
        /// </summary>
        // [HideInEditorMode]
        // public ScriptSkillDataBase SkillDataBaseInst;
        
        [HideInInspector]
        public System.Action Action;

        [HideInInspector]
        public Func<bool> Func;
        
        [HideInInspector]
        public Func<bool> Func1;

        [HideInInspector]
        public Func<Ebonor.DataCtrl.Action.Request, Ebonor.DataCtrl.Action.Result> Func2;

        
        /// <summary>
        /// 获取将要执行的委托函数，也可以在这里面做一些初始化操作
        /// </summary>
        /// <returns></returns>
        public virtual System.Action GetActionToBeDone()
        {
            return null;
        }

        public virtual Func<bool> GetFuncToBeDone()
        {
            return null;
        }

        public virtual Func<bool> GetFunc1ToBeDone()
        {
            return null;
        }
        
        public virtual Func<Action.Request, Action.Result> GetFunc2ToBeDone()
        {
            return null;
        }

        public virtual float GetInternal()
        {
            return 0f;
        }

        public Action _CreateNPBehaveAction()
        {
            GetActionToBeDone();
            if (this.Action != null)
            {
                return new Ebonor.DataCtrl.Action(Action);
            }

            GetFuncToBeDone();
            if (this.Func != null)
            {
                return new Ebonor.DataCtrl.Action(this.Func);
            }
            
            GetFunc1ToBeDone();
            if (this.Func != null)
            {
                return new Ebonor.DataCtrl.Action(this.Func1);
            }

            GetFunc2ToBeDone();
            if (this.Func2 != null)
            {
                return new Ebonor.DataCtrl.Action(this.Func2);
            }

            //log.Info($"{this.GetType()} _CreateNPBehaveAction失败，因为没有找到可以绑定的委托");
            return null;
        }
    }
}
