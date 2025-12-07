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
        public override UniTask Enter()
        {
            base.Enter();
            log.Debug("Enter showcase scene.");
            return UniTask.CompletedTask;
        }

        public override UniTask Exit()
        {
            base.Exit();
            log.Debug("Exit showcase scene.");
            return UniTask.CompletedTask;
        }

        public override UniTask ResetScene()
        {
            log.Debug("Reset showcase scene to defaults.");
            return UniTask.CompletedTask;
        }
    }

}
