
using UnityEngine;

namespace Ebonor.Framework
{
    
    public static class ApplicationQuitHelper
    {
        private static bool _isQuitting = false;
        public static bool IsQuitting => _isQuitting;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _isQuitting = false;
            Application.quitting += OnApplicationQuitting;
        }

        private static void OnApplicationQuitting()
        {
            _isQuitting = true;
        }
    }
}
