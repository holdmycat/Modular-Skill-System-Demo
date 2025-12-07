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

        public string AttrName => attrName;
        public string AttrDesc => attrDesc;
        public string AttrIconName => attrIconName;
        public string AttrAvatarName => attrAvatarName;
        
        
        public void OnInitActorNumericComponent(CharacterRuntimeData characterRuntimeData)
        {
            
        }

        public void OnUnInitActorNumericComponent()
        {
            
        }

        public void OnResetActorNumericComponent()
        {
           
        }
        
    }
}
