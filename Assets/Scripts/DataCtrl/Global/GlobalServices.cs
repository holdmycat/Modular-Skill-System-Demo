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
        
        private static readonly ILog log = LogManager.GetLogger(typeof(GlobalServices));
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

        #region Player Input

        private static IPlayerInputSource playerInputSource;

        /// <summary>Expose read-only player input surface for other systems.</summary>
        public static IPlayerInputSource PlayerInputSource => playerInputSource;

        public static void SetPlayerInputSource(IPlayerInputSource source)
        {
            playerInputSource = source;
        }

        #endregion
        
        #region GlobalGameConfig
        private static GlobalGameConfig globalGameConfig;
        public static GlobalGameConfig GlobalGameConfig => globalGameConfig;
        public static void SetGlobalGameConfig(GlobalGameConfig config)
        {
            globalGameConfig = config;
        }
        #endregion
        
        #region Gen NetId

        private static int _counter = -1;
        
        /// <summary>Get the next unique actor ID (uint, starts at 0).</summary>
        public static uint NextId()
        {
            return unchecked((uint)Interlocked.Increment(ref _counter));
        }

        #endregion
        
        #region App Lifecycle State

        // Core state lock: private set ensures external modules cannot reset it.
        public static bool IsAppInitialized { get; private set; } = false;
        
        public static void MarkAppInitialized()
        {
            if (IsAppInitialized) return;
            IsAppInitialized = true;
            log.Debug("[GlobalServices] App Initialized. State Locked.");
        }
        
        #endregion
        
    }
}
