//------------------------------------------------------------
// File: ResourceLoader.cs
// Purpose: Unified resource loading facade supporting Resources and Addressables.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public enum ResourceLoadMode
    {
        Resources,
        Addressables
    }

    /// <summary>
    /// Unified resource loader; inject preferred mode at construction/Init.
    /// </summary>
    public sealed class ResourceLoader
    {
        private readonly ResourceLoadMode _mode;

        public ResourceLoader(ResourceLoadMode mode = ResourceLoadMode.Resources)
        {
            _mode = mode;
        }
        
        /// <summary>
        /// Load GameObject (prefab) asynchronously.
        /// </summary>
        public async UniTask<GameObject> LoadPrefab(string key)
        {
            switch (_mode)
            {
                case ResourceLoadMode.Resources:
                    return await Resources.LoadAsync<GameObject>(key).ToUniTask() as GameObject;
                case ResourceLoadMode.Addressables:
                    return await LoadAddressable<GameObject>(key);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Load AudioClip asynchronously.
        /// </summary>
        public async UniTask<AudioClip> LoadAudioClip(string key)
        {
            switch (_mode)
            {
                case ResourceLoadMode.Resources:
                    return await Resources.LoadAsync<AudioClip>(key).ToUniTask() as AudioClip;
                case ResourceLoadMode.Addressables:
                    return await LoadAddressable<AudioClip>(key);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Load all assets under the mapped Resources folder for the given type.
        /// </summary>
        public async UniTask<IList<T>> LoadAllAssets<T>(ResourceAssetType type) where T : UnityEngine.Object
        {
            switch (_mode)
            {
                case ResourceLoadMode.Resources:
                    var folderPath = GetResourceFolder(type);
                    // Resources.LoadAll is synchronous; wrap in completed task for a consistent async signature.
                    var assets = Resources.LoadAll<T>(folderPath);
                    await UniTask.CompletedTask;
                    return assets;
                case ResourceLoadMode.Addressables:
                    Debug.LogWarning($"LoadAllAssets not implemented for Addressables mode (type {type}).");
                    await UniTask.CompletedTask;
                    return Array.Empty<T>();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Load generic asset asynchronously.
        /// </summary>
        public async UniTask<T> LoadAsset<T>(string assetName, ResourceAssetType type) where T : UnityEngine.Object
        {
            switch (_mode)
            {
                case ResourceLoadMode.Resources:
                    var resPath = BuildResourcePath(type, assetName);
                    return await Resources.LoadAsync<T>(resPath).ToUniTask() as T;
                case ResourceLoadMode.Addressables:
                    return await LoadAddressable<T>(assetName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTask<T> LoadAddressable<T>(string key) where T : UnityEngine.Object
        {
#if ADDRESSABLES_AVAILABLE
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(key);
            return await handle.ToUniTask();
#else
            Debug.LogWarning($"Addressables not enabled; key {key} cannot be loaded in this build.");
            await UniTask.CompletedTask;
            return null;
#endif
        }

        private static string BuildResourcePath(ResourceAssetType type, string assetName)
        {
            // Map asset type to folder; adjust to match Resources layout.
            var folder = GetResourceFolder(type);
            return string.IsNullOrEmpty(folder) ? assetName : $"{folder}/{assetName}";
        }

        private static string GetResourceFolder(ResourceAssetType type)
        {
            switch (type)
            {
                case ResourceAssetType.ScriptableObject:
                    return "ScriptableObject";
                case ResourceAssetType.UiPrefab:
                    return "UI";
                case ResourceAssetType.HeroModelPrefab:
                    return "Models/Heros";
                case ResourceAssetType.AllCharacterData:
                    return "AllCharacterData";
                case ResourceAssetType.SceneStateManager:
                    return "SceneStateManager";
                case ResourceAssetType.UIAtlas:
                    return "UI/UIAtlas";
                case ResourceAssetType.AllSquadBehavour:
                    return "AllSquadBehavour";
                default:
                    return string.Empty;
            }
        }
    }
}
