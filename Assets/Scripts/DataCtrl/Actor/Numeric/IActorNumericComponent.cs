
//------------------------------------------------------------
// File: IActorNumericComponent.cs
// Purpose: Interface exposing numeric attribute metadata for actors.
//------------------------------------------------------------

namespace Ebonor.DataCtrl
{
    
    public interface IActorNumericComponent 
    {
        /// <summary>Display name of the attribute.</summary>
        string AttrName { get; }
        /// <summary>Icon resource/key for the attribute.</summary>
        string AttrIconName { get; }
        /// <summary>Avatar/portrait resource/key for the attribute.</summary>
        string AttrAvatarName { get; }
        /// <summary>Description text for the attribute.</summary>
        string AttrDesc { get; }

        uint NetId { get; }
        
        //first time initialization
        void OnInitActorNumericComponent(CharacterRuntimeData characterRuntimeData);

        void OnUnInitActorNumericComponent();
        
        //reset
        void OnResetActorNumericComponent();




    }
}
