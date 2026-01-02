using System;

namespace Ebonor.DataCtrl
{
    public abstract class ANP_BBValue
    {
        public abstract Type NP_BBValueType { get; }

        /// <summary>
        /// 从另一个anpBbValue设置数据
        /// </summary>
        /// <param name="anpBbValue"></param>
        public abstract void SetValueFrom(ANP_BBValue anpBbValue);
    }
}
