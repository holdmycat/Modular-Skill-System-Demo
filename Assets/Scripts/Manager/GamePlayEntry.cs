//------------------------------------------------------------
// File: GamePlayEntry.cs
// Created: 2025-12-06
// Purpose: Scene entry point to bootstrap GameClientManager and initial scene manager.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.Manager
{
    public class GamePlayEntry : MonoBehaviour
    {
        [Header("Bootstrap")]
        [SerializeField] private SceneManagerBase initialSceneManagerPrefab;

        static readonly ILog log = LogManager.GetLogger(typeof(GamePlayEntry));
        
        private void Start()
        {
            var clientManager = gameObject.GetComponent<GameClientManager>() ?? gameObject.AddComponent<GameClientManager>();

            // Ensure DataCtrl and default setup are ready.
            clientManager.EnsureDataCtrl();

            // If specified, switch to the provided scene manager; otherwise rely on GameClientManager default.
            if (initialSceneManagerPrefab != null)
            {
                log.Info($"Switching to initial scene manager: {initialSceneManagerPrefab.GetType().Name}");
                clientManager.SwitchSceneManager(initialSceneManagerPrefab);
                return;
            }
            
            log.Warn("initialSceneManagerPrefab is null; GameClientManager will use its default (if any).");
        }
    }
}
