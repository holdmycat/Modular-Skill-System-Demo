//------------------------------------------------------------
// File: InputRouterIntegrationTests.cs
// Purpose: Validate input router initialization with the PlayerInput prefab.
//------------------------------------------------------------
using System.Collections;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.Manager;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode
{
    public class InputRouterIntegrationTests
    {
        [UnityTest]
        public IEnumerator CreateInputSource_LoadsInputSystemPrefab_WhenConfigured()
        {
            // Arrange global services for ResourceLoader and config.
            if (GlobalServices.ResourceLoader == null)
            {
                GlobalServices.InitResourceLoader(new ResourceLoader(ResourceLoadMode.Resources));
            }

            var config = ScriptableObject.CreateInstance<GlobalGameConfig>();
            config.playerInputPrefabPath = ConstData.UI_PLAYERACTION;
            GlobalServices.SetGlobalGameConfig(config);

            var go = new GameObject("TestGameClientManager");
            var manager = go.AddComponent<GameClientManager>();

            // Act & Assert
            yield return UniTask.ToCoroutine(async () =>
            {
                var createInputSource = typeof(GameClientManager).GetMethod(
                    "CreateInputSource",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                var task = (UniTask<IPlayerInputSource>)createInputSource.Invoke(manager, null);
                var source = await task;

                Assert.IsNotNull(source, "Input source should be created from prefab.");
                Assert.IsInstanceOf<InputSystemPlayerInputSource>(source, "InputSystem source should be used when prefab exists.");
            });

            Object.DestroyImmediate(go);
        }
    }
}
