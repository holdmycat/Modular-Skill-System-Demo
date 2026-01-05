//------------------------------------------------------------
// File: ConstData.cs
// Created: 2025-11-29
// Purpose: Static container for framework-wide constants.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.Framework
{
    public static class ConstData
    {
        
        #region assembly names
        public const string AD_DATACTRL = "Ebonor.DataCtrl"; 
        public const string AD_MANAGER = "Ebonor.Manager";
        public const string AD_MULTIPLAYER = "Ebonor.MultiPlayer";
        #endregion
         
        #region blackboard consts
       
        public const string BB_BUFFBINDANIMSTACKSTATE = "BuffBindAnimStackState";
        
        public const string BELONGTOSKILLID = "BelongToSkillId";
        
        public const string PATH_SLGSQUADBEHAVOUR = "Assets/Resources/AllSquadBehavour";
        
        #endregion
        
        #region ui const

        public const string UI_PLAYERACTION = "PlayerInput";

        public const string UIATLAS_CHARACTERICON = "ui_icon_characters";

        #endregion
    }
}
