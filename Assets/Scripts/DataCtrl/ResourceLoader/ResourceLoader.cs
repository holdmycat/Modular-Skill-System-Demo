//------------------------------------------------------------
// File: ResourceLoader.cs
// Purpose: Unified resource loading facade supporting Resources and Addressables.
//------------------------------------------------------------
using System;
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
            switch (type)
            {
                case ResourceAssetType.ScriptableObject:
                    return $"ScriptableObject/{assetName}";
                case ResourceAssetType.UiPrefab:
                    return $"UI/{assetName}";
                case ResourceAssetType.HeroModelPrefab:
                    return $"Models/Hero/{assetName}";
                case ResourceAssetType.AllCharacterData:
                    return $"AllCharacterData/{assetName}";
                default:
                    return assetName;
            }
        }
    }
}
