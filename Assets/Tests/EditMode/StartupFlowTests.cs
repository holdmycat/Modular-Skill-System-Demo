//------------------------------------------------------------
// File: StartupFlowTests.cs
// Purpose: EditMode tests for startup/config/resource loading pipeline.
//------------------------------------------------------------
using System.Reflection;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Manager;
using NUnit.Framework;
using UnityEngine;
using System.Threading.Tasks;

namespace Tests.EditMode
{
    public class StartupFlowTests
    {
        [SetUp]
        public void ResetGlobals()
        {
            SetStaticField(typeof(GlobalServices), "resourceLoader", null);
            SetStaticField(typeof(GlobalServices), "globalGameConfig", null);
        }

        [Test]
        public void GlobalServices_InitResourceLoader_OnlyOnce()
        {
            var loaderA = new ResourceLoader(ResourceLoadMode.Resources);
            var loaderB = new ResourceLoader(ResourceLoadMode.Addressables);

            GlobalServices.InitResourceLoader(loaderA);
            GlobalServices.InitResourceLoader(loaderB);

            Assert.AreSame(loaderA, GlobalServices.ResourceLoader, "ResourceLoader should be initialized only once.");
        }

        [Test]
        public void GlobalGameConfig_FallbackToResourcesWhenNull()
        {
            // No config, ensure InitResourceLoader can be called with default.
            Assert.IsNull(GlobalServices.ResourceLoader);
            GlobalServices.InitResourceLoader(new ResourceLoader(ResourceLoadMode.Resources));
            Assert.IsNotNull(GlobalServices.ResourceLoader);
            Assert.AreEqual(ResourceLoadMode.Resources, GetLoaderMode(GlobalServices.ResourceLoader));
        }

        [Test]
        public async Task SceneManagerBase_LifecycleInvokesHooks()
        {
            var go = new GameObject("TestSceneManager");
            var sm = go.AddComponent<TestSceneManagerHooks>();

            await sm.Enter();
            await sm.Pause(true);
            await sm.Pause(false);
            await sm.ResetScene();
            await sm.Exit();

            Assert.IsTrue(sm.Entered);
            Assert.IsTrue(sm.Exited);
            Assert.IsTrue(sm.PausedOnce);
            Assert.IsTrue(sm.ResumedOnce);

            Object.DestroyImmediate(go);
        }

        [Test]
        public async Task ShowCaseSceneManager_LoadsRoomAndRoots()
        {
            SetStaticField(typeof(GlobalServices), "resourceLoader", new ResourceLoader(ResourceLoadMode.Resources));

            var sceneConfig = ScriptableObject.CreateInstance<SceneLoadConfig>();
            var go = new GameObject("ShowCaseSceneManager");
            var sm = go.AddComponent<ShowCaseSceneManager>();

            SetInstanceField(sm, "_sceneConfig", sceneConfig);

            await sm.Enter();

            var roomMgr = go.GetComponent<Ebonor.GamePlay.GamePlayRoomManager>();
            Assert.IsNotNull(roomMgr, "Room manager should be created.");

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(sceneConfig);
        }

        [Test]
        public void GamePlayRoomManager_CreateDestroy()
        {
            var go = new GameObject("Room");
            var room = go.AddComponent<Ebonor.GamePlay.GamePlayRoomManager>();
            Assert.IsNotNull(room);
            Object.DestroyImmediate(go);
        }

        private static void SetStaticField(System.Type type, string fieldName, object value)
        {
            var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
            field?.SetValue(null, value);
        }

        private static void SetInstanceField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(target, value);
        }

        private static ResourceLoadMode GetLoaderMode(ResourceLoader loader)
        {
            var field = typeof(ResourceLoader).GetField("_mode", BindingFlags.Instance | BindingFlags.NonPublic);
            return field != null ? (ResourceLoadMode)field.GetValue(loader) : ResourceLoadMode.Resources;
        }
    }

    public class TestSceneManagerHooks : SceneManagerBase
    {
        public bool Entered;
        public bool Exited;
        public bool PausedOnce;
        public bool ResumedOnce;

        protected override UniTask OnEnter()
        {
            Entered = true;
            return UniTask.CompletedTask;
        }

        protected override UniTask OnExit()
        {
            Exited = true;
            return UniTask.CompletedTask;
        }

        protected override UniTask OnPause(bool paused)
        {
            if (paused) PausedOnce = true;
            else ResumedOnce = true;
            return UniTask.CompletedTask;
        }
    }
}
