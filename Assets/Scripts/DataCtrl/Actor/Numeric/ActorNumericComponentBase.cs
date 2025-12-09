//------------------------------------------------------------
// File: ActorNumericComponent.cs
// Purpose: Base implementation storing numeric attribute metadata.
//------------------------------------------------------------

using UnityEngine;

namespace Ebonor.DataCtrl
{
    
    public abstract class ActorNumericComponentBase : MonoBehaviour, IActorNumericComponent
    {
        protected string attrName;
        protected string attrDesc;
        protected string attrIconName;
        protected string attrAvatarName;
        protected uint _netId;
        protected long _unitModelNodeId;
        public string AttrName => attrName;
        public string AttrDesc => attrDesc;
        public string AttrIconName => attrIconName;
        public string AttrAvatarName => attrAvatarName;
        public uint NetId => _netId;
        
        public long UnitModelNodeId => _unitModelNodeId;
        
        public void OnInitActorNumericComponent(CharacterRuntimeData characterRuntimeData, uint netid)
        {
            _netId = netid;

            _unitModelNodeId = characterRuntimeData._numericId;

            var unitAttr = DataCtrl.Inst.GetUnitAttributeNodeData(characterRuntimeData._numericId);

            attrAvatarName = unitAttr.UnitAvatar;

            attrName = unitAttr.UnitName;

            attrIconName = unitAttr.UnitSprite;
        }
        
        public void OnUnInitActorNumericComponent()
        {
            
        }

        public void OnResetActorNumericComponent()
        {
           
        }
    }
}
