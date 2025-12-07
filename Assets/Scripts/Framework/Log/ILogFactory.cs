//------------------------------------------------------------
// File: ILogFactory.cs
// Created: 2025-11-29
// Purpose: Factory interface for building log instances.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------


using System;

namespace Ebonor.Framework
{
    public interface ILogFactory
    {
        ILog GetLogger<T>();

        ILog GetLogger(Type type);

        ILog GetLogger(string name);

        void SetLogSwitch(bool on);
    }
}