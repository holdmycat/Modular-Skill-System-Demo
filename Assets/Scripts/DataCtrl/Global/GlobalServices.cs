//------------------------------------------------------------
// File: GlobalServices.cs
// Purpose: Simple global service locator for shared systems (e.g., ResourceLoader).
//------------------------------------------------------------

using System.Threading;
using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    public static class GlobalServices
    {
        private static ResourceLoader resourceLoader;
        public static ResourceLoader ResourceLoader => resourceLoader;
        public static void InitResourceLoader(ResourceLoader loader)
        {
            if (resourceLoader != null)
            {
                return;
            }
            resourceLoader = loader;
        }


        #region GlobalGameConfig
        private static GlobalGameConfig globalGameConfig;
        public static GlobalGameConfig GlobalGameConfig => globalGameConfig;
        public static void SetGlobalGameConfig(GlobalGameConfig config)
        {
            globalGameConfig = config;
        }
        #endregion


        #region MyRegion

        private static int _counter = -1;

        /// <summary>Get the next unique actor ID (uint, starts at 0).</summary>
        public static uint NextId()
        {
            return unchecked((uint)Interlocked.Increment(ref _counter));
        }

        #endregion
        
    }
}
