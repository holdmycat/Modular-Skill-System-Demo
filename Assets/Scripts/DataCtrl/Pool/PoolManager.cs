using System;
using System.Collections.Generic;
using System.Reflection;
using Ebonor.Framework;
using UnityEngine;
using UObject = UnityEngine.Object;
namespace Ebonor.DataCtrl
{
    //对外接口
    public partial class PoolManager : MonoBehaviour
    {
        
        /// <summary>
        /// 创建对象池
        /// </summary>
        public static void CreatePoolManager()
        {
            if (null != mInst)
            {
                log.WarnFormat("CreatePoolManager called multiple times, and that is wrong");
                return;
            }
            var go = new GameObject("PoolManager");
            go.AddComponent<PoolManager>();
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(go);
            }
            GOHelper.ResetGameObject(go);
        }
        
        /// <summary>
        /// 预加载对象
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="_name">对象名称</param>
        /// <typeparam name="T">对象管理对象类型</typeparam>
        public static void InitPoolItem<T>(ePoolObjectType type, string _name) where T : PoolItemBase
        {
            mInst.mDicPoolCtrl[type].InitPoolItem<T>(_name);
        }
        
        public static T SpawnItemFromPool<T>(ePoolObjectType type, string _name) where T : PoolItemBase
        {
            if (mInst == null || mInst.Equals(null))
            {
                log.Warn("SpawnItemFromPool called before PoolManager is created; returning null.");
                return null;
            }

            if (mInst.mDicPoolCtrl.TryGetValue(type, out var inst))
            {
                return inst.SpawnItemFromPool<T>(_name);
            }

            return null;
        }
        
        public static void DespawnItemToPool<T>(ePoolObjectType type, T t) where T : PoolItemBase
        {
            if (mInst == null || mInst.Equals(null))
            {
                log.Warn("DespawnItemToPool called before PoolManager is created; ignoring.");
                return;
            }

            if (mInst.mDicPoolCtrl.TryGetValue(type, out var inst))
            {
                inst.DespawnItemFromPool<T>(t);
            }
        }
        
        /// <summary>
        /// 自回收逻辑，每帧判断是否使用完毕(客户端特效自回收)
        /// </summary>
        public void OnUpdate()
        {
            foreach (var variable in mDicPoolCtrl)
            {
                variable.Value.OnUpdate(this);
            }
        }

        public void DoBeforeLeavingScene()
        {
            foreach (var variable in mInst.mDicPoolCtrl)
            {
                variable.Value.ClearAllPoolItem();
            }
            mDicPoolCtrl.Clear();
            mGOSceneInst = null;
        }

        public void DoBeforeEnteringScene(string _sceneName)
        {

            if(mDicPoolCtrl.Count > 0)
                return;
            mGOSceneInst = new GameObject("PoolManager_" + _sceneName);
            
            GOHelper.ResetGameObject(mGOSceneInst);

            //mResourcePoolConfig = DataCtrl.Inst.GetResourcePoolConfig();
            
            for (var i = ePoolObjectType.eEffect; i < ePoolObjectType.ePoolSize; i++)
            {
                if(!mDicPoolCtrlType.ContainsKey(i))
                    continue;
                Type type = mDicPoolCtrlType[i];
                MethodInfo methodInfo = typeof(PoolManager).GetMethod(nameof(CreatePoolCtrl)).MakeGenericMethod(type);  
                var result = methodInfo.Invoke(this, new object[]{i});
                mDicPoolCtrl.Add(i, result as PoolCtrlBase);
            }
        }
        
        public static PoolCtrlBase CreatePoolCtrl<T>(ePoolObjectType type) where T : PoolCtrlBase
        {
            var go = new GameObject(type.ToString());
            var ctrl = go.AddComponent<T>();
            GOHelper.ResetLocalGameObject(mInst.mGOSceneInst, go, true, 1);
            ctrl.transform.localPosition = Vector3.up * 100; 
            ctrl.InitPool(type);
            return ctrl;
        }

        public static void OnPauseAndResumePoolManager(bool isPause)
        {
            foreach (var variable in mInst.mDicPoolCtrl)
            {
                variable.Value.OnPauseResumeGame(mInst, isPause);
            }
        }
        
        
    }
    
    //系统接口和非静态私有数据
    public partial class PoolManager : MonoBehaviour
    {
        Dictionary<ePoolObjectType, PoolCtrlBase> mDicPoolCtrl = new Dictionary<ePoolObjectType, PoolCtrlBase>();

        GameObject mGOSceneInst;

        private ResourcePoolConfig mResourcePoolConfig;
        public ResourcePoolConfig ResourcePoolConfig => mResourcePoolConfig;
        
        void Awake()
        {
            mInst = this;
        }
    }

    //系统静态数据和接口
    public partial class PoolManager : MonoBehaviour
    {
        static Dictionary<ePoolObjectType, Type> mDicPoolCtrlType = new Dictionary<ePoolObjectType, Type>
        {
            //{ePoolObjectType.eEffect, typeof(PoolEffectCtrl)},
            {ePoolObjectType.eModel, typeof(PoolModelCtrl)},
            //{ePoolObjectType.eFloatingText, typeof(PoolFloatingTextCtrl)},
        };
        
        static readonly ILog log = LogManager.GetLogger(typeof(PoolManager));

        static PoolManager mInst;
        public static PoolManager Inst => mInst;
        
        
    }
}
