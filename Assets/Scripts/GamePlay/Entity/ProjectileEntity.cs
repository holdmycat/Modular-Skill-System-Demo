using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ProjectileEntity : GameEntity
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProjectileEntity));
        public float Damage { get; set; }
        public float Speed { get; set; }
        public uint OwnerId { get; set; }
        
        
        protected override async UniTask InitializeDataAsync(CharacterRuntimeData data)
        {
            log.DebugFormat("[ProjectileEntity] InitializeDataAsync netId={0}", data._netId);
            await UniTask.CompletedTask;
        }
    }
}
