//------------------------------------------------------------
// File: Operator.cs
// Created: 2025-12-05
// Purpose: Comparison operators used by condition decorators.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public enum Operator
    {
        IS_SET,
        IS_NOT_SET,
        IS_EQUAL,
        IS_NOT_EQUAL,
        IS_GREATER_OR_EQUAL,
        IS_GREATER,
        IS_SMALLER_OR_EQUAL,
        IS_SMALLER,
        ALWAYS_TRUE
    }
}