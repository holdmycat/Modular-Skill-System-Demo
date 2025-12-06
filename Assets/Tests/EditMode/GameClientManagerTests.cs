//------------------------------------------------------------
// File: GameClientManagerTests.cs
// Created: 2025-12-06
// Purpose: Edit mode tests for GameClientManager lifecycle and scene manager handling.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Reflection;
using Ebonor.DataCtrl;
using Ebonor.Manager;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    public class GameClientManagerTests
    {
        private static T CreateComponent<T>(string name = null) where T : MonoBehaviour
        {
            var go = new GameObject(name ?? typeof(T).Name);
            return go.AddComponent<T>();
        }

        [Test]
        public void EnsureDataCtrl_AddsComponent()
        {
            var manager = CreateComponent<GameClientManager>();
            manager.EnsureDataCtrl();
            Assert.IsNotNull(DataCtrl.Inst, "DataCtrl should be created and parented.");
            Object.DestroyImmediate(manager.gameObject);
        }

        [Test]
        public void SwitchSceneManager_EnterExitSequence()
        {
            var manager = CreateComponent<GameClientManager>();
            TestSceneManager.ExitCount = 0;
            var first = manager.SwitchSceneManager<TestSceneManager>();
            Assert.IsTrue(first.Entered, "First scene manager should Enter.");

            var second = manager.SwitchSceneManager<TestSceneManager>();
            Assert.AreEqual(1, TestSceneManager.ExitCount, "First scene manager should Exit when switching.");
            Assert.IsTrue(second.Entered, "Second scene manager should Enter.");

            Object.DestroyImmediate(manager.gameObject);
        }

        [Test]
        public void PauseAndQuit_AreForwarded()
        {
            var manager = CreateComponent<GameClientManager>();
            var sm = manager.SwitchSceneManager<TestSceneManager>();

            var pauseMethod = typeof(GameClientManager).GetMethod("OnApplicationPause", BindingFlags.Instance | BindingFlags.NonPublic);
            pauseMethod.Invoke(manager, new object[] { true });
            pauseMethod.Invoke(manager, new object[] { false });

            var quitMethod = typeof(GameClientManager).GetMethod("OnApplicationQuit", BindingFlags.Instance | BindingFlags.NonPublic);
            quitMethod.Invoke(manager, null);

            Assert.IsTrue(sm.Paused, "Pause should be forwarded.");
            Assert.IsTrue(sm.Resumed, "Resume should be forwarded.");
            Assert.IsTrue(sm.Exited, "Exit should be forwarded on quit.");

            Object.DestroyImmediate(manager.gameObject);
            // sm is destroyed with manager; no separate destroy to avoid MissingReference.
        }
    }

    /// <summary>
    /// Test implementation of SceneManagerBase for lifecycle assertions.
    /// </summary>
    public class TestSceneManager : SceneManagerBase
    {
        public bool Entered;
        public bool Exited;
        public bool Paused;
        public bool Resumed;
        public static int ExitCount;

        public override void Enter() => Entered = true;
        public override void Exit()
        {
            Exited = true;
            ExitCount++;
        }

        public override void Pause(bool paused)
        {
            if (paused) Paused = true;
            else Resumed = true;
        }
    }
}
