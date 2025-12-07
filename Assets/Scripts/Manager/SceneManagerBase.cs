//------------------------------------------------------------
// File: SceneManagerBase.cs
// Created: 2025-12-06
// Purpose: Base class for scene-specific managers controlled by GameClientManager.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using Ebonor.GamePlay;
using UnityEngine;

namespace Ebonor.Manager
{
    /// <summary>
    /// Base class for scene-specific managers controlled by GameClientManager.
    /// </summary>
    public abstract class SceneManagerBase : MonoBehaviour
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(SceneManagerBase));
        
        protected GameClientManager ClientManager { get; private set; }

        protected GamePlayRoomManager _roomInstance;
        
        /// <summary>
        /// Called once when the scene manager is created.
        /// </summary>
        public virtual UniTask Init(GameClientManager clientManager)
        {
            ClientManager = clientManager;
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Called when this manager becomes active. Input is auto-blocked during execution.
        /// </summary>
        public  UniTask Enter()
        {
            return RunWithInputBlock(OnEnter);
        }

        /// <summary>
        /// Called once per frame by the client manager.
        /// </summary>
        public  UniTask Tick(float deltaTime) => OnTick(deltaTime);

        /// <summary>
        /// Called when switching away from this scene. Input is auto-blocked during execution.
        /// </summary>
        public  UniTask Exit()
        {
            return RunWithInputBlock(OnExit);
        }

        /// <summary>
        /// Application pause/resume hook for scene-specific logic. Input is auto-blocked during execution.
        /// </summary>
        public  UniTask Pause(bool paused)
        {
            return RunWithInputBlock(() => OnPause(paused));
        }

        /// <summary>
        /// Reset scene state (UI, entities, data) to initial baseline. Input is auto-blocked during execution.
        /// </summary>
        public  UniTask ResetScene()
        {
            return RunWithInputBlock(OnResetScene);
        }

        /// <summary>
        /// Override for entering logic (executed under input block).
        /// </summary>
        protected virtual UniTask OnEnter() => UniTask.CompletedTask;

        /// <summary>
        /// Override for per-frame logic (not blocked).
        /// </summary>
        protected virtual UniTask OnTick(float deltaTime) => UniTask.CompletedTask;

        /// <summary>
        /// Override for exit logic (executed under input block).
        /// </summary>
        protected virtual UniTask OnExit() => UniTask.CompletedTask;

        /// <summary>
        /// Override for pause/resume logic (executed under input block).
        /// </summary>
        protected virtual UniTask OnPause(bool paused) => UniTask.CompletedTask;

        /// <summary>
        /// Override for reset logic (executed under input block).
        /// </summary>
        protected virtual UniTask OnResetScene() => UniTask.CompletedTask;

        private static async UniTask RunWithInputBlock(Func<UniTask> body)
        {
            using (InputBlocker.AcquireAll())
            {
                await body();
            }
        }
    }
}
