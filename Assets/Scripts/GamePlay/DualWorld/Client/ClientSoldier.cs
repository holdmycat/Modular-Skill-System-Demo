using UnityEngine;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ClientSoldier : MonoBehaviour
    {
        public int NetId;

        public void Initialize(int netId)
        {
            NetId = netId;
            name = $"Soldier_{NetId}";
        }

        public void OnUpdate()
        {
            // Visual updates
        }
    }
}
