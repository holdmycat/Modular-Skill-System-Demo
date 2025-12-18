using System.Collections.Generic;
using UnityEngine;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ClientTeam : MonoBehaviour
    {
        public int TeamId;
        public List<ClientSoldier> Soldiers = new List<ClientSoldier>();

        public void Initialize(int teamId)
        {
            TeamId = teamId;
            name = $"Team_{TeamId}";
        }

        public void OnUpdate()
        {
            foreach (var soldier in Soldiers)
            {
                soldier.OnUpdate();
            }
        }
    }
}
