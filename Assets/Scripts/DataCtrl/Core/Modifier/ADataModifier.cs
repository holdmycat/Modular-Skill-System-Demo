using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    public abstract class ADataModifier : IReference
    {
        /// <summary>
        /// 修改器类型
        /// </summary>
        public abstract ModifierType ModifierType { get; }

        /// <summary>
        /// 目标属性名称 (Debugging/Logging)
        /// </summary>
        public string TargetAttributeName;

        /// <summary>
        /// 获取修改值
        /// </summary>
        /// <returns></returns>
        public abstract float GetModifierValue();

        public abstract void Clear();
    }
}
