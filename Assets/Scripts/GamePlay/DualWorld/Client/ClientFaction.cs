using System.Collections.Generic;
using UnityEngine;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Pure C# Wrapper for the Client Faction. 
    /// Inherits FactionBase to share "Management" logic identity.
    /// Manages the visual GameObject internally.
    /// </summary>
    public class ClientFaction : FactionBase
    {
        private readonly GameObject _viewRoot;
        public List<ClientTeam> Teams { get; private set; } = new List<ClientTeam>();

        public ClientFaction(int factionId, Transform parentRoot) : base(factionId)
        {
            _viewRoot = new GameObject($"Faction_{factionId}");
            Ebonor.Framework.GOHelper.ResetLocalGameObject(parentRoot.gameObject, _viewRoot, true);
        }

        public void CreateTeam(int teamId)
        {
            // Create Client Team Logic/View
            // For now, ClientTeam is still a MonoBehaviour? 
            // Better to keep the pattern: ClientTeam is also a wrapper? 
            // Let's assume ClientTeam is a MonoBehaviour for now to save time, or match user request.
            // User: "维护好自己的军队".
            
            var teamGo = new GameObject($"Team_{teamId}");
            Ebonor.Framework.GOHelper.ResetLocalGameObject(_viewRoot, teamGo, true);
            var team = teamGo.AddComponent<ClientTeam>();
            team.Initialize(teamId);
            Teams.Add(team);
        }

        public override void Tick(int tick)
        {
           
        }

        public override void OnUpdate()
        {
            foreach (var team in Teams)
            {
                team.OnUpdate();
            }
        }
        
        public void Destroy()
        {
             if(_viewRoot != null) Object.Destroy(_viewRoot);
        }
    }
}
