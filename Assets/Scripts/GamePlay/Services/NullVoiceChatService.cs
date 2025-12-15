using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// No-op fallback for voice chat; allows startup to continue if no real provider is bound.
    /// </summary>
    public class NullVoiceChatService : IVoiceChatService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NullVoiceChatService));

        public UniTask InitializeAsync()
        {
            log.Info("[VoiceChat] Null provider initialized (feature disabled).");
            return UniTask.CompletedTask;
        }
    }
}
