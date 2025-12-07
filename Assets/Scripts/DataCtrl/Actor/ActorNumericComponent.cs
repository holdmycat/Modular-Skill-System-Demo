//------------------------------------------------------------
// File: ActorNumericComponent.cs
// Purpose: Base implementation storing numeric attribute metadata.
//------------------------------------------------------------

using UnityEngine;

namespace Ebonor.DataCtrl
{
    
    public class ActorNumericComponent : MonoBehaviour, IActorNumericComponent
    {
        [SerializeField] protected string attrName;
        [SerializeField] protected string attrDesc;
        [SerializeField] protected string attrIconName;
        [SerializeField] protected string attrAvatarName;

        public string AttrName => attrName;
        public string AttrDesc => attrDesc;
        public string AttrIconName => attrIconName;
        public string AttrAvatarName => attrAvatarName;
    }
}
