using UnityEngine;
using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    public interface IActorNumericComponentFactory
    {
        T Create<T>(GameObject target) where T : ActorNumericComponentBase;
    }
}
