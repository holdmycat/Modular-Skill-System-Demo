//------------------------------------------------------------
// File: Stops.cs
// Created: 2025-12-05
// Purpose: Stop propagation policies for decorator and composite nodes.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public enum Stops
    {
        NONE,
        SELF,
        LOWER_PRIORITY,
        BOTH,
        IMMEDIATE_RESTART,
        LOWER_PRIORITY_IMMEDIATE_RESTART
    }
}