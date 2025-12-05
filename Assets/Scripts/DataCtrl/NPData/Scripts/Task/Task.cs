//------------------------------------------------------------
// File: Task.cs
// Created: 2025-12-05
// Purpose: Abstract base class for executable behavior tree tasks.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public abstract class Task : Node
    {
        public Task(string name) : base(name)
        {
        }
    }
}