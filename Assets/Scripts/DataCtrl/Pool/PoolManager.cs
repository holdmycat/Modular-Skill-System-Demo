using System;
using System.Collections.Generic;
using Ebonor.Framework;
using UnityEngine;
using Zenject;
using UObject = UnityEngine.Object;

namespace Ebonor.DataCtrl
{
    public class PoolManager : MonoBehaviour, IPoolManager, IInitializable, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PoolManager));

        [Inject]
        private DiContainer _container;

        private Dictionary<ePoolObjectType, PoolCtrlBase> mDicPoolCtrl = new Dictionary<ePoolObjectType, PoolCtrlBase>();
      

        // Static mapping of pool types to controller types
        private static readonly Dictionary<ePoolObjectType, Type> mDicPoolCtrlType = new Dictionary<ePoolObjectType, Type>
        {
            //{ePoolObjectType.eEffect, typeof(PoolEffectCtrl)},
            {ePoolObjectType.eModel, typeof(PoolModelCtrl)},
            //{ePoolObjectType.eFloatingText, typeof(PoolFloatingTextCtrl)},
        };

        public void Initialize()
        {
            log.Info("[PoolManager] Initializing...");
            // Create a root GameObject for pools if it doesn't exist
          
            // Initialize all registered pools
            foreach (var kvp in mDicPoolCtrlType)
            {
                CreatePoolCtrl(kvp.Key, kvp.Value);
            }
        }

        public void Dispose()
        {
            log.Info("[PoolManager] Disposing...");
            foreach (var ctrl in mDicPoolCtrl.Values)
            {
                ctrl.ClearAllPoolItem();
            }
            mDicPoolCtrl.Clear();
            
        }

        private void CreatePoolCtrl(ePoolObjectType type, Type ctrlType)
        {
            if (mDicPoolCtrl.ContainsKey(type)) return;

            // Create a new GameObject for this pool controller
            var go = new GameObject(type.ToString());
            go.transform.SetParent(gameObject.transform);
            
            // Use Zenject to instantiate the component so it gets injected if needed
            // But since we are adding it to a specific GO, we use InstantiateComponent
            var ctrl = _container.InstantiateComponent(ctrlType, go) as PoolCtrlBase;
            
            if (ctrl != null)
            {
                ctrl.transform.localPosition = Vector3.up * 100; // Legacy positioning
                ctrl.InitPool(type, _container); // Pass container to the controller
                mDicPoolCtrl.Add(type, ctrl);
                log.Debug($"[PoolManager] Created pool controller for {type}");
            }
            else
            {
                log.Error($"[PoolManager] Failed to create pool controller for {type}");
            }
        }
        
        public T SpawnItemFromPool<T>(ePoolObjectType type, string name) where T : PoolItemBase
        {
            if (mDicPoolCtrl.TryGetValue(type, out var ctrl))
            {
                return ctrl.SpawnItemFromPool<T>(name);
            }
            
            log.Warn($"[PoolManager] No pool controller found for type {type}");
            return null;
        }

        public void DespawnItemToPool<T>(ePoolObjectType type, T item) where T : PoolItemBase
        {
            if (item == null) return;

            if (mDicPoolCtrl.TryGetValue(type, out var ctrl))
            {
                ctrl.DespawnItemFromPool(item);
            }
            else
            {
                log.Warn($"[PoolManager] No pool controller found for type {type} to despawn {item.name}");
                // Fallback destroy if pool not found
                Destroy(item.gameObject);
            }
        }

        public void InitPoolItem<T>(ePoolObjectType type, string name) where T : PoolItemBase
        {
            if (mDicPoolCtrl.TryGetValue(type, out var ctrl))
            {
                ctrl.InitPoolItem<T>(name);
            }
        }
    }
}
