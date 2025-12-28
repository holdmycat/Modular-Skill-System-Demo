using Zenject;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Runtime context for client-specific state (e.g. Selection, UI Filters, Camera State).
    /// This differs from ShowcaseContext in that it stores transient local state, not server synced data.
    /// </summary>
    public class ClientRuntimeContext : IContext
    {
        // Example: Commonly used 'Selected Unit'
        public uint SelectedNetId { get; set; }

        public ClientRuntimeContext()
        {
        }
    }
}
