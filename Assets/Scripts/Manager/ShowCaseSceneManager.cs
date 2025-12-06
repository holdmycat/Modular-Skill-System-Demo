//------------------------------------------------------------
// File: ShowCaseSceneManager.cs
// Created: 2025-12-06
// Purpose: Scene manager for the ShowcaseScene sandbox.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.Manager
{
    
    public class ShowCaseSceneManager : SceneManagerBase
    {
        public override void Enter()
        {
            base.Enter();
            log.Debug("Enter showcase scene.");
        }

        public override void Exit()
        {
            base.Exit();
            log.Debug("Exit showcase scene.");
        }

        public override void ResetScene()
        {
            log.Debug("Reset showcase scene to defaults.");
        }
    }

}
