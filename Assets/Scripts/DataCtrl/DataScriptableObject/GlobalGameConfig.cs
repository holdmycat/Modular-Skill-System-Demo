//------------------------------------------------------------
// File: GlobalGameConfig.cs
// Purpose: Global game configuration (loading mode, rules).
//------------------------------------------------------------
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    [CreateAssetMenu(menuName = "Ebonor/Config/Global Game Config", fileName = "GlobalGameConfig")]
    public class GlobalGameConfig : ScriptableObject
    {
        [Header("Resource Loading")]
        public ResourceLoadMode loadMode = ResourceLoadMode.Resources;
        
        [Header("Default Player Id")]
        public long defaultPlayerHeroId;
    }
}
