using UnityEngine;
using UnityEngine.U2D;

namespace Ebonor.DataCtrl
{
    public interface IUIAtlasRepository
    {
        void SaveAtlas(string name, SpriteAtlas atlas);
        SpriteAtlas GetAtlas(string name);
        
        Sprite GetUICharacterAtlas(string name);
    }
}
