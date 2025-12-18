using System.Collections.Generic;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Functional base class for Factions.
    /// Responsible for maintaining its own Teams.
    /// </summary>
    public abstract class FactionBase
    {
        public int FactionId { get; protected set; }
        
        // In a real generic implementation, this might be a generic list or dictionary, 
        // but for now we keep it simple. Subclasses might manage specific Team types.
        
        public FactionBase(int factionId)
        {
            FactionId = factionId;
        }

        public abstract void Tick(int tick);
        public virtual void OnUpdate() { }
    }
}
