using UnityEngine;

namespace Ebonor.GamePlay
{
    public class PlayerEntity : CharacterEntity
    {
        // Player specific logic (Input, Camera, etc.)
        
        public override async Cysharp.Threading.Tasks.UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            log.Info($"[PlayerEntity] Initialized: {name}");
        }
    }
}
