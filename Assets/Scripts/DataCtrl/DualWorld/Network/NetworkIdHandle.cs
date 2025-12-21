using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    public struct NetworkIdHandle
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkIdHandle));
        
        private uint _netid;
        public uint NetId => _netid;
        
        public void BindId(uint netid)
        {
            if (_netid != 0)
            {
                log.Error("[NetworkIdHandle] Fatal error, _netId is " + _netid);
                return;
            }
            _netid = netid;
        }
    }
}
