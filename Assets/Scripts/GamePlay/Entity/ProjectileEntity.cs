using UnityEngine;

namespace Ebonor.GamePlay
{
    public class ProjectileEntity : GameEntity
    {
        public float Damage { get; set; }
        public float Speed { get; set; }
        public uint OwnerId { get; set; }

        public override async Cysharp.Threading.Tasks.UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            // Reset physics, trail renderers, etc.
        }
    }
}
