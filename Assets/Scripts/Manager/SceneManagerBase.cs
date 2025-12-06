//------------------------------------------------------------
// File: SceneManagerBase.cs
// Created: 2025-12-06
// Purpose: Base class for scene-specific managers controlled by GameClientManager.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using Ebonor.Framework;
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

        /// <summary>
        /// Called once when the scene manager is created.
        /// </summary>
        public virtual void Init(GameClientManager clientManager)
        {
            ClientManager = clientManager;
        }

        /// <summary>
        /// Called when this manager becomes active.
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Called once per frame by the client manager.
        /// </summary>
        public virtual void Tick(float deltaTime) { }

        /// <summary>
        /// Called when switching away from this scene.
        /// </summary>
        public virtual void Exit() { }

        /// <summary>
        /// Application pause/resume hook for scene-specific logic.
        /// </summary>
        public virtual void Pause(bool paused) { }

        /// <summary>
        /// Reset scene state (UI, entities, data) to initial baseline.
        /// </summary>
        public virtual void ResetScene() { }
    }
}
