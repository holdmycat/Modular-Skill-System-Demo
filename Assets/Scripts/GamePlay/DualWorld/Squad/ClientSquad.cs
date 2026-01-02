using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientSquad : BaseSquad
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientSquad));

        private ResourceLoader _resourceLoader;

        [Inject]
        public ClientSquad(
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService, 
            ICharacterDataRepository characterDataRepository,
            ResourceLoader resourceLoader,
            CommanderContextData contextData, ShowcaseContext showcaseContext)
        {
            log.Info($"[ClientSquad] Construction");
            
            _networkBus = networkBus;
            _characterDataRepository = characterDataRepository;
            _dataLoaderService = dataLoaderService;
            _resourceLoader = resourceLoader;
            _showcaseContext = showcaseContext;
            _contextData = contextData;
            _faction = contextData.Faction;
        }
        

        private UnityEngine.GameObject _debugVisual;
        private UnityEngine.Transform _debugRoot;

        public void SetDebugVisualRoot(UnityEngine.Transform root)
        {
            _debugRoot = root;
            if (_debugVisual != null)
            {
                log.Info($"[ClientSquad] Reparenting Visual {_debugVisual.name} to Root {root.name}");
                _debugVisual.transform.SetParent(_debugRoot);
            }
        }

        public override async void InitAsync()
        {
            log.Info($"[ClientSquad] InitAsync");
            
#if UNITY_EDITOR
            if (_globalGameConfig != null && _globalGameConfig.IsDebugVisualsEnabled && _globalGameConfig.ShowSquadVisual)
            {
                var prefab = await UnityEngine.Resources.LoadAsync<UnityEngine.GameObject>("Models/DebugSquad") as UnityEngine.GameObject;
                if (prefab != null)
                {
                   log.Info($"[ClientSquad] Instantiating Debug Visual for NetId {NetId}");
                   _debugVisual = UnityEngine.Object.Instantiate(prefab);
                   if (_debugRoot != null)
                   {
                       _debugVisual.transform.SetParent(_debugRoot);
                   }
                   _debugVisual.transform.position = Position;
                   _debugVisual.transform.rotation = Rotation;
                   
                   string unitName = _unitAttr != null ? _unitAttr.UnitName : "Unknown";
                   _debugVisual.name = $"Squad_{_faction}_{unitName}_{NetId}";
                   log.Info($"[ClientSquad] Debug Visual Created: {_debugVisual.name}");
                }
                else
                {
                    log.Warn("[ClientSquad] Failed to load DebugSquad prefab.");
                }
            }
#endif
            
            //Load Squad Solider
            LoadSoldierVisuals().Forget();
        }

        private UnityEngine.GameObject _visualsRoot;

        private async UniTaskVoid LoadSoldierVisuals()
        {
            if (_unitAttr == null)
            {
                log.Warn($"[ClientSquad] UnitAttr is null, cannot load soldiers.");
                return;
            }

            string avatar = _unitAttr.UnitAvatar;
            log.Info($"[ClientSquad] Loading Soldier Model: {avatar} for Unit: {_unitAttr.UnitName}");

            var avatarName = avatar + "_" + _faction;
            
            var prefab = await _resourceLoader.LoadAsset<UnityEngine.GameObject>(avatarName, ResourceAssetType.HeroModelPrefab);
            if (prefab == null)
            {
                log.Error($"[ClientSquad] Failed to load soldier model: {avatarName}");
                return;
            }

            _visualsRoot = new UnityEngine.GameObject($"Visuals_{_faction}_{_unitAttr.UnitName}_{NetId}");
            _visualsRoot.transform.position = Position;
            _visualsRoot.transform.rotation = Rotation;

            int count = GetInitialCount();
            for (int i = 0; i < count; i++)
            {
                var soldier = UnityEngine.Object.Instantiate(prefab, _visualsRoot.transform);
                soldier.transform.localPosition = GetSoldierLocalPosition(i);
                soldier.transform.localRotation = UnityEngine.Quaternion.identity;
            }
            log.Info($"[ClientSquad] Loaded {count} soldiers.");
        }

        protected override void InitializeNumeric()
        {
            _numericComponent = _numericFactory.CreateSquad(_netId, _squadUnitAttr);
            
            // Register Data to ShowcaseContext (Data Layer)
            if (_showcaseContext != null)
            {
                _showcaseContext.Register(NetId, _numericComponent, Faction);
            }
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info($"[ClientSquad] ShutdownAsync");
            
            if (_debugVisual != null)
            {
                log.Info($"[ClientSquad] Destroying Debug Visual {_debugVisual.name}");
                UnityEngine.Object.Destroy(_debugVisual);
                _debugVisual = null;
            }

            if (_visualsRoot != null)
            {
                UnityEngine.Object.Destroy(_visualsRoot);
                _visualsRoot = null;
            }

            await base.ShutdownAsync();
        }
        
        public override void Tick(int tick)
        {
            
        }
        
        public override void OnUpdate()
        {
             if (_globalGameConfig != null && _globalGameConfig.IsDebugVisualsEnabled && _globalGameConfig.ShowSquadVisual && _debugVisual != null)
             {
                  _debugVisual.transform.position = Position;
                  _debugVisual.transform.rotation = Rotation;
             }
             
             if (_visualsRoot != null)
             {
                 _visualsRoot.transform.position = Position;
                 _visualsRoot.transform.rotation = Rotation;
             }
        }
        
        public class Factory : PlaceholderFactory<ClientSquad> 
        {
            public Factory()
            {
                log.Info($"[ClientSquad.Factory] Construction");
            }
        }
        
    }
}
