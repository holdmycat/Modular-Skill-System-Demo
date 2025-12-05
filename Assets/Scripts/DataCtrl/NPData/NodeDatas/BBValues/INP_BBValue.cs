//------------------------------------------------------------
// File: INP_BBValue.cs
// Created: 2025-12-01
// Purpose: Interface for blackboard value accessors.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public interface INP_BBValue<T>
    {
        T GetValue();
    }
}
