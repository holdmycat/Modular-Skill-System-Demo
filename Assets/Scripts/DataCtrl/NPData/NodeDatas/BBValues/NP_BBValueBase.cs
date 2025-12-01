//------------------------------------------------------------
// File: NP_BBValueBase.cs
// Created: 2025-12-01
// Purpose: Base blackboard value wrapper with generic storage.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------

using UnityEngine;

namespace Ebonor.DataCtrl
{
  
    public abstract class NP_BBValueBase<T> : ANP_BBValue, INP_BBValue<T>
    {
        public T Value;

        public T GetValue()
        {
            return Value;
        }
        
        public override void SetValueFrom(ANP_BBValue anpBbValue)
        {
            if (anpBbValue == null || !(anpBbValue is NP_BBValueBase<T>))
            {
                Debug.LogError($"{typeof(T)} copy failed: value is null or invalid type.");
                return;
            }
            this.SetValueFrom((INP_BBValue<T>) anpBbValue);
        }
        
        public virtual void SetValueFrom(INP_BBValue<T> bbValue)
        {
            if (bbValue == null || !(bbValue is NP_BBValueBase<T>) )
            {
                Debug.LogError($"{typeof(T)} copy failed: value is null or invalid type.");
                return;
            }
            
            this.SetValueFrom(bbValue.GetValue());
        }
        
        public virtual void SetValueFrom(T bbValue)
        {
            Value = bbValue;
        }
    }
}
