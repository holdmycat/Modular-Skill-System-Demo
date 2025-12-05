//------------------------------------------------------------
// File: ANP_BBValue.cs
// Created: 2025-12-01
// Purpose: Abstract base for blackboard value types.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;

namespace Ebonor.DataCtrl
{
   
    public abstract class ANP_BBValue
    {
        public abstract Type NP_BBValueType { get; }

        /// <summary>
        ///  anpBbValue 
        /// </summary>
        /// <param name="anpBbValue"></param>
        public abstract void SetValueFrom(ANP_BBValue anpBbValue);
    }
}
