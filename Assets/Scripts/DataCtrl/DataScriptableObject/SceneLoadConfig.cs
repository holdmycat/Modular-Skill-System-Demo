//------------------------------------------------------------
// File: SceneLoadConfig.cs
// Purpose: ScriptableObject describing scene content via data (addresses/ids) to drive loading without drag-drop.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    [CreateAssetMenu(menuName = "Ebonor/Scenes/Scene Load Config", fileName = "SceneLoadConfig")]
    public class SceneLoadConfig : ScriptableObject
    {
        public enum AssetLoadType
        {
            ResourcesPath,
            AddressableKey
        }

       

        [Serializable]
        public class CharacterEntry
        {
            public string id;
            public string assetKey;
            public AssetLoadType loadType = AssetLoadType.ResourcesPath;
            public Vector3 position = Vector3.zero;
            public Vector3 rotation = Vector3.zero;
        }

        [Serializable]
        public class UiEntry
        {
            public string id;
            public string assetKey;
            public AssetLoadType loadType = AssetLoadType.ResourcesPath;
        }

        [Serializable]
        public class AudioEntry
        {
            public string id;
            public string assetKey;
            public AssetLoadType loadType = AssetLoadType.ResourcesPath;
            public bool loop = true;
            [Range(0f, 1f)] public float volume = 1f;
        }

        [Serializable]
        public class LogicEntry
        {
            public string id;
            public string assetKey;
            public AssetLoadType loadType = AssetLoadType.ResourcesPath;
        }
        

        [Header("Characters (ordered)")]
        public List<CharacterEntry> characters = new List<CharacterEntry>();

        [Header("UI (ordered)")]
        public List<UiEntry> ui = new List<UiEntry>();

        [Header("Audio (preload)")]
        public List<AudioEntry> audio = new List<AudioEntry>();

        [Header("Startup Logic")]
        public List<LogicEntry> logic = new List<LogicEntry>();
    }
}
