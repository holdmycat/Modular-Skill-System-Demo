using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.GamePlay
{
    public class ItemEntity : GameEntity
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ItemEntity));
        public int ItemId { get; set; }
        public int Count { get; set; }
        

        protected override async UniTask InitializeDataAsync(CharacterRuntimeData data)
        {
            log.DebugFormat("[ItemEntity] InitializeDataAsync netId={0}", data._netId);
            await UniTask.CompletedTask;
        }
    }
}
