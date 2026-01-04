using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ServerSquad : BaseSquad
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerSquad));
        
        [Inject]
        public ServerSquad(
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService, 
            ICharacterDataRepository characterDataRepository,
            INPRuntimeTreeFactory npRuntimeTreeFactory,
            SquadStackFsm.Factory stackFsmFactory,
            GlobalGameConfig globalGameConfig,
            CommanderContextData contextData)
        {
            log.Info($"[ServerSquad] Construction");
            
            _characterDataRepository = characterDataRepository;
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
            _npRuntimeTreeFactory = npRuntimeTreeFactory;
            _faction = contextData.Faction;
            _globalGameConfig = globalGameConfig;
            _stackFsmFactory = stackFsmFactory;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ServerSquad] InitAsync");
            
            if (EnsureStackFsm())
            {
                // Birth -> Idle
                //_stackFsm.BootstrapBirthThenIdle();
            }
        }
        
        private bool EnsureStackFsm()
        {
            if (!TryInitStackFsm())
            {
                return false;
            }

            _stackFsm.OnStateChanged -= OnServerStackStateChanged;
            _stackFsm.OnStateChanged += OnServerStackStateChanged;
            return true;
        }

        private void OnServerStackStateChanged(eBuffBindAnimStackState state)
        {
            if (_networkBus == null || _stackFsm == null) return;

            _networkBus.SendRpc(NetId, new RpcSquadStackStateChanged
            {
                NetId = NetId,
                ClassType = _stackFsm.ClassType,
                State = state
            });
        }

        /// <summary>
        /// Server-only entry to drive stack state changes.
        /// </summary>
        public void SetStackStateOnServer(eBuffBindAnimStackState state, bool force = false)
        {
            if (!EnsureStackFsm()) return;
            _stackFsm.SetState(state, force);
        }
        
        protected override void InitializeNumeric()
        {
            _numericComponent = _numericFactory.CreateSquad(_netId, _squadUnitAttr);
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerSquad] ShutdownAsync");
            await base.ShutdownAsync();
            _networkBus.UnRegisterSpawns(_netId, this);
        }
        
        public class Factory : PlaceholderFactory<ServerSquad> 
        {
            public Factory()
            {
                log.Info($"[ServerSquad.Factory] Construction");
            }
        }
    }
}
