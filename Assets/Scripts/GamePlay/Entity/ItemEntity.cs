using UnityEngine;

namespace Ebonor.GamePlay
{
    public class ItemEntity : GameEntity
    {
        public int ItemId { get; set; }
        public int Count { get; set; }

        public override async Cysharp.Threading.Tasks.UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            // Reset item visual state
        }
    }
}
