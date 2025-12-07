//------------------------------------------------------------
// File: ShowCaseSceneManager.cs
// Created: 2025-12-06
// Purpose: Data-driven scene manager for the ShowcaseScene sandbox.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using UnityEngine;
using Ebonor.Framework;
using Ebonor.GamePlay;

namespace Ebonor.Manager
{
    
    public class ShowCaseSceneManager : SceneManagerBase
    {
        private SceneLoadConfig _sceneConfig;
        private Transform _roomRoot;
        private Transform _characterRoot;
        private Transform _uiRoot;
        private Transform _audioRoot;
        private Transform _logicRoot;
        private ResourceLoader _loader;
        
        private readonly Dictionary<string, GameObject> _characterInstances = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, GameObject> _uiInstances = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, AudioSource> _audioSources = new Dictionary<string, AudioSource>();
        private readonly Dictionary<string, GameObject> _logicInstances = new Dictionary<string, GameObject>();

        protected override async UniTask OnEnter()
        {
            
            GlobalServices.ResourceLoader.LoadAsset<>()
            
            log.Info("Enter showcase scene: loading content (data-driven).");
            if (_sceneConfig == null)
            {
                log.Error("Scene config is missing; skipping content load.");
                return;
            }

            _loader = GlobalServices.ResourceLoader ?? new ResourceLoader(ResourceLoadMode.Resources);

            await LoadRoom();
            // await LoadCharacters();
            // await LoadUi();
            // await LoadAudio();
            // await StartLogic();
        }

        protected override async UniTask OnExit()
        {
            log.Info("Exit showcase scene: cleaning up content.");
            // await UnloadLogic();
            // await UnloadAudio();
            // await UnloadUi();
            // await UnloadCharacters();
            await UnloadRoom();
        }

        protected override UniTask OnResetScene()
        {
            log.Debug("Reset showcase scene to defaults.");
            return UniTask.CompletedTask;
        }

        private async UniTask LoadRoom()
        {
            if (null != _roomInstance)
            {
                log.Error("Fatal error, _roomInstance not null");
                return;
            }
            _roomInstance = gameObject.AddComponent<GamePlayRoomManager>();
            _roomRoot = _roomInstance.transform;
            _characterRoot = _characterRoot ?? new GameObject("Characters").transform;
            _characterRoot.SetParent(transform);
            _uiRoot = _uiRoot ?? new GameObject("UI").transform;
            _uiRoot.SetParent(transform);
            _audioRoot = _audioRoot ?? new GameObject("Audio").transform;
            _audioRoot.SetParent(transform);
            _logicRoot = _logicRoot ?? new GameObject("Logic").transform;
            _logicRoot.SetParent(transform);
        }

        private async UniTask UnloadRoom()
        {
            Destroy(_roomInstance);
            _roomInstance = null;
            await UniTask.CompletedTask;
        }

        private async UniTask LoadCharacters()
        {
            _characterInstances.Clear();
            foreach (var entry in _sceneConfig.characters)
            {
                if (entry == null || string.IsNullOrEmpty(entry.assetKey)) continue;
                var prefab = await _loader.LoadPrefab(entry.assetKey);
                if (prefab == null) continue;

                var go = Instantiate(prefab, _characterRoot ?? transform);
                go.name = string.IsNullOrEmpty(entry.id) ? prefab.name : entry.id;
                go.transform.SetPositionAndRotation(entry.position, Quaternion.Euler(entry.rotation));
                _characterInstances[go.name] = go;
            }
        }

        private async UniTask UnloadCharacters()
        {
            foreach (var kvp in _characterInstances)
            {
                DestroySafe(kvp.Value);
                await UniTask.Yield();
            }
            _characterInstances.Clear();
        }

        private async UniTask LoadUi()
        {
            _uiInstances.Clear();
            foreach (var entry in _sceneConfig.ui)
            {
                if (entry == null || string.IsNullOrEmpty(entry.assetKey)) continue;
                var prefab = await _loader.LoadPrefab(entry.assetKey);
                if (prefab == null) continue;

                var go = Instantiate(prefab, _uiRoot ?? transform);
                go.name = string.IsNullOrEmpty(entry.id) ? prefab.name : entry.id;
                _uiInstances[go.name] = go;
            }
        }

        private async UniTask UnloadUi()
        {
            foreach (var kvp in _uiInstances)
            {
                DestroySafe(kvp.Value);
                await UniTask.Yield();
            }
            _uiInstances.Clear();
        }

        private async UniTask LoadAudio()
        {
            _audioSources.Clear();
            foreach (var entry in _sceneConfig.audio)
            {
                if (entry == null || string.IsNullOrEmpty(entry.assetKey)) continue;
                var clip = await _loader.LoadAudioClip(entry.assetKey);
                if (clip == null) continue;

                var go = new GameObject(string.IsNullOrEmpty(entry.id) ? clip.name : entry.id);
                go.transform.SetParent(_audioRoot ?? transform);
                var source = go.AddComponent<AudioSource>();
                source.clip = clip;
                source.loop = entry.loop;
                source.playOnAwake = false;
                source.volume = entry.volume;
                source.Play();
                _audioSources[go.name] = source;
            }
        }

        private async UniTask UnloadAudio()
        {
            foreach (var kvp in _audioSources)
            {
                if (kvp.Value != null) kvp.Value.Stop();
                DestroySafe(kvp.Value?.gameObject);
                await UniTask.Yield();
            }
            _audioSources.Clear();
        }

        private async UniTask StartLogic()
        {
            _logicInstances.Clear();
            foreach (var entry in _sceneConfig.logic)
            {
                if (entry == null || string.IsNullOrEmpty(entry.assetKey)) continue;
                var prefab = await _loader.LoadPrefab(entry.assetKey);
                if (prefab == null) continue;

                var go = Instantiate(prefab, _logicRoot ?? transform);
                go.name = string.IsNullOrEmpty(entry.id) ? prefab.name : entry.id;
                _logicInstances[go.name] = go;
            }
        }

        private async UniTask UnloadLogic()
        {
            foreach (var kvp in _logicInstances)
            {
                DestroySafe(kvp.Value);
                await UniTask.Yield();
            }
            _logicInstances.Clear();
        }

        private void DestroySafe(GameObject go)
        {
            if (go == null) return;
            if (Application.isPlaying) Destroy(go);
            else DestroyImmediate(go);
        }
    }

}
