using UnityEngine;

namespace Ebonor.GamePlay
{
    public class NpcEntity : CharacterEntity
    {
        // NPC specific logic (AI, Behavior Tree)
        
        public override async Cysharp.Threading.Tasks.UniTask InitializeAsync()
        {
            await base.InitializeAsync();
        }
    }
}
