//------------------------------------------------------------
// File: Exception.cs
// Created: 2025-12-05
// Purpose: Custom behavior tree exception wrapper.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;

namespace Ebonor.DataCtrl
{
    public class Exception : System.Exception
    {
        public Exception(string message) : base(message)
        {
        }
    }
}