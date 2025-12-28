using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public interface INumericComponent 
    {
        bool IsServer { get; }
        
        /// <summary>
        /// 构造初始化：分配字典内存，加载初始配置
        /// </summary>
        void Initialize();

        /// <summary>
        /// 升级：当 Entity 等级变化时调用，重新计算成长数值
        /// </summary>
        void LevelUp();

        /// <summary>
        /// 重置：将所有运行时数值回滚到初始状态（通常用于战斗结束后重置状态，或者对象池回收前）
        /// </summary>
        void Reset();

        /// <summary>
        /// 析构：清理内存，解绑事件
        /// </summary>
        void Dispose();
        
         uint NetId { get; }
    }
}
