//------------------------------------------------------------
// File: GameClientManagerTests.cs
// Created: 2025-12-06
// Purpose: Edit mode tests for GameClientManager lifecycle and scene manager handling.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Reflection;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Manager;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
        public async UniTask SwitchSceneManager_EnterExitSequence()
        {
            var manager = CreateComponent<GameClientManager>();
            TestSceneManager.ExitCount = 0;
            var first = await manager.SwitchSceneManager<TestSceneManager>();
            Assert.IsTrue(first.Entered, "First scene manager should Enter.");

            var second = await manager.SwitchSceneManager<TestSceneManager>();
            Assert.AreEqual(1, TestSceneManager.ExitCount, "First scene manager should Exit when switching.");
            Assert.IsTrue(second.Entered, "Second scene manager should Enter.");

            Object.DestroyImmediate(manager.gameObject);
        }

        [UnityTest]
        public System.Collections.IEnumerator PauseAndQuit_AreForwarded()
        {
            var manager = CreateComponent<GameClientManager>();
            var smTask = manager.SwitchSceneManager<TestSceneManager>();
            yield return smTask.ToCoroutine();
            var sm = smTask.GetAwaiter().GetResult();

            var pauseMethod = typeof(GameClientManager).GetMethod("OnApplicationPause", BindingFlags.Instance | BindingFlags.NonPublic);
            if (pauseMethod != null)
            {
                pauseMethod.Invoke(manager, new object[] { true });
                pauseMethod.Invoke(manager, new object[] { false });
            }

            yield return null;

            var quitMethod = typeof(GameClientManager).GetMethod("OnApplicationQuit", BindingFlags.Instance | BindingFlags.NonPublic);
            if (quitMethod != null) quitMethod.Invoke(manager, null);
            yield return null;

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

        protected override UniTask OnEnter()
        {
            Entered = true;
            return UniTask.CompletedTask;
        }
        protected override UniTask OnExit()
        {
            Exited = true;
            ExitCount++;
            return UniTask.CompletedTask;
        }

        protected override UniTask OnPause(bool paused)
        {
            if (paused) Paused = true;
            else Resumed = true;
            return UniTask.CompletedTask;
        }
    }
}
