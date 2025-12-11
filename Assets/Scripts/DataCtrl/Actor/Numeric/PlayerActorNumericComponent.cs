//------------------------------------------------------------
// File: PlayerActorNumericComponent.cs
// Purpose: Player-specific numeric component (extend with player-only stats if needed).
//------------------------------------------------------------

using UnityEngine;

namespace Ebonor.DataCtrl
{
    
    public class PlayerActorNumericComponent : ActorNumericComponentBase
    {
        // Add player-only numeric fields or initialization overrides here.
        
        public  override float GetByKey(int key)
        {
            NumericDic.TryGetValue(key, out float value);
            
            return value;
        }
    }
}
