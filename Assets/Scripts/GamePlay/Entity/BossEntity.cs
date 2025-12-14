using UnityEngine;

namespace Ebonor.GamePlay
{
    public class BossEntity : NpcEntity
    {
        // Boss specific logic (Phases, HUD)
        
        public override async Cysharp.Threading.Tasks.UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            log.Info($"[BossEntity] Initialized Boss: {name}");
        }
    }
}
