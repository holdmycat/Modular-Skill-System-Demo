using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.GamePlay
{
    public class PlayerEntity : CharacterEntity
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PlayerEntity));
        // Player specific logic (Input, Camera, etc.)
        
        
        protected override async UniTask InitializeDataAsync(CharacterRuntimeData data)
        {
            log.DebugFormat("[PlayerEntity] InitializeDataAsync netId={0}", data._netId);
            LoadDataAsync<PlayerActorNumericComponent>(data);
            await UniTask.CompletedTask;
        }
    }
}
