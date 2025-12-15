using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class NpcEntity : CharacterEntity
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NpcEntity));
        // NPC specific logic (AI, Behavior Tree)
        
        protected override async UniTask InitializeDataAsync(CharacterRuntimeData data)
        {
            log.DebugFormat("[NpcEntity] InitializeDataAsync netId={0}", data._netId);

            await UniTask.CompletedTask;
        }
    }
}
