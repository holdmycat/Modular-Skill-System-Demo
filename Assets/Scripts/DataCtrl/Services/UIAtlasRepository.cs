using System.Collections.Generic;
using Ebonor.Framework;
using UnityEngine;
using UnityEngine.U2D;

namespace Ebonor.DataCtrl
{
    public class UIAtlasRepository : IUIAtlasRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UIAtlasRepository));
        private readonly Dictionary<string, SpriteAtlas> _atlasDict = new Dictionary<string, SpriteAtlas>();

        public void SaveAtlas(string name, SpriteAtlas atlas)
        {
            if (atlas == null)
            {
                log.Warn($"[UIAtlasRepository] Trying to save null atlas for: {name}");
                return;
            }

            if (!_atlasDict.ContainsKey(name))
            {
                _atlasDict.Add(name, atlas);
                log.Info($"[UIAtlasRepository] Saved Atlas: {name}");
            }
            else
            {
                _atlasDict[name] = atlas;
            }
        }

        public SpriteAtlas GetAtlas(string name)
        {
            if (_atlasDict.TryGetValue(name, out var atlas))
            {
                return atlas;
            }
            log.Warn($"[UIAtlasRepository] Atlas not found: {name}");
            return null;
        }

        public Sprite GetUICharacterAtlas(string spriteName)
        {
            if (_atlasDict.TryGetValue(ConstData.UIATLAS_CHARACTERICON, out var atlas))
            {
                return atlas.GetSprite(spriteName);
            }

            log.Warn($"[UIAtlasRepository] Atlas not found: {ConstData.UIATLAS_CHARACTERICON}");
            
            return null;
        }
    }
}
