namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Resolves runtime entities (commander/squad/etc.) from ids for NP actions.
    /// </summary>
    public interface INPRuntimeEntityResolver
    {
        object Resolve(uint id, eMPNetPosition isServer);
    }

    /// <summary>
    /// No-op resolver; always returns null.
    /// </summary>
    public sealed class NullNPRuntimeEntityResolver : INPRuntimeEntityResolver
    {
        public object Resolve(uint id, eMPNetPosition isServer)
        {
            return null;
        }
    }
}
