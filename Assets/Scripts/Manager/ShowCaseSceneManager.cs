//------------------------------------------------------------
// File: ShowCaseSceneManager.cs
// Created: 2025-12-06
// Purpose: Scene manager for the ShowcaseScene sandbox.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using Cysharp.Threading.Tasks;

namespace Ebonor.Manager
{
    
    public class ShowCaseSceneManager : SceneManagerBase
    {
        protected override UniTask OnEnter()
        {
            log.Debug("Enter showcase scene.");
            return UniTask.CompletedTask;
        }

        protected override UniTask OnExit()
        {
            log.Debug("Exit showcase scene.");
            return UniTask.CompletedTask;
        }

        protected override UniTask OnResetScene()
        {
            log.Debug("Reset showcase scene to defaults.");
            return UniTask.CompletedTask;
        }
    }

}
