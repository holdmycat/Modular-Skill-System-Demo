//------------------------------------------------------------
// File: ShowCaseSceneManager.cs
// Created: 2025-12-06
// Purpose: Data-driven scene manager for the ShowcaseScene sandbox.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using Cysharp.Threading.Tasks;

namespace Ebonor.Manager
{
    
    public interface ISceneManager
    {
        UniTask StartupSequence();
    }
    

}
