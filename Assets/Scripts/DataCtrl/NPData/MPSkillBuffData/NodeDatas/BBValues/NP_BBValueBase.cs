using UnityEngine;

namespace Ebonor.DataCtrl
{
    [System.Serializable]
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
                Debug.LogError($"{typeof(T)} 拷贝失败，anpBbValue为空或类型非法");
                return;
            }
            this.SetValueFrom((INP_BBValue<T>) anpBbValue);
        }
        
        public virtual void SetValueFrom(INP_BBValue<T> bbValue)
        {
            if (bbValue == null || !(bbValue is NP_BBValueBase<T>) )
            {
                Debug.LogError($"{typeof(T)} 拷贝失败，anpBbValue为空或类型非法");
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
