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

        public virtual void SetNGRuntimeTree<T>(string _uid, T tree) where T : MonoBehaviour
        {
            
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
