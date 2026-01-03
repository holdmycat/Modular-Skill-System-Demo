using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Resolves runtime entities via INetworkBus registration.
    /// </summary>
    public sealed class NetworkBusRuntimeEntityResolver : INPRuntimeEntityResolver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NetworkBusRuntimeEntityResolver));
        private readonly INetworkBus _networkBus;

        public NetworkBusRuntimeEntityResolver(INetworkBus networkBus)
        {
            _networkBus = networkBus;
        }

        public object Resolve(uint id, bool isServer)
        {
            try
            {
                return _networkBus.GetSpawnedOrNull(id, isServer);
            }
            catch (System.Exception ex)
            {
                Log.Warn($"[NetworkBusRuntimeEntityResolver] Resolve failed for id:{id}, isServer:{isServer}. {ex.Message}");
                return null;
            }
        }
    }
}
