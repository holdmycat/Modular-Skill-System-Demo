//------------------------------------------------------------
// File: GlobalServices.cs
// Purpose: Simple global service locator for shared systems (e.g., ResourceLoader).
//------------------------------------------------------------

using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    public static class GlobalServices
    {
        public static ResourceLoader ResourceLoader { get; set; }


        #region GlobalGameConfig
        private static GlobalGameConfig globalGameConfig;
        public static GlobalGameConfig GlobalGameConfig => globalGameConfig;
        public static void SetGlobalGameConfig(GlobalGameConfig config)
        {
            globalGameConfig = config;
        }
        #endregion
        
    }
}
