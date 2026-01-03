using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Per-tree context passed to actions for resolving owner/target and other services.
    /// </summary>
    public class NPRuntimeContext
    {
        public readonly bool IsServer;
        public readonly uint OwnerId;
        public readonly uint StarterId;
        public readonly uint TargetId;
        public readonly ILog Log;
        public readonly INPRuntimeEntityResolver Resolver;

        public NPRuntimeContext(bool isServer, uint ownerId, uint starterId, uint targetId, ILog log, INPRuntimeEntityResolver resolver)
        {
            IsServer = isServer;
            OwnerId = ownerId;
            StarterId = starterId;
            TargetId = targetId;
            Log = log;
            Resolver = resolver;
        }

        public T Resolve<T>(uint id) where T : class
        {
            return Resolver?.Resolve(id, IsServer) as T;
        }
    }
}
