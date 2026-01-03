using UnityEngine;
using Zenject;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Bridges an NPData Clock into Zenject's fixed tick loop.
    /// </summary>
    public class NPClockDriver : IFixedTickable
    {
        protected readonly Clock Clock;

        public NPClockDriver(Clock clock)
        {
            Clock = clock;
        }

        public void FixedTick()
        {
            Clock.OnFixedUpdate(Time.fixedDeltaTime);
        }
    }

    public sealed class ServerClockDriver : NPClockDriver
    {
        public ServerClockDriver([Inject(Id = ClockIds.Server)] Clock clock) : base(clock) { }
    }

    public sealed class ClientClockDriver : NPClockDriver
    {
        public ClientClockDriver([Inject(Id = ClockIds.Client)] Clock clock) : base(clock) { }
    }
}
