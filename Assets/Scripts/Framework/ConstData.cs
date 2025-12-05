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
        public const string BB_ISGETBIRTH = "IsGetBirth";
        public const string BB_ISCASTSKILL = "IsCastSkill";
        public const string BB_ISBUFFRUNNING = "IsBuffRunning";
        public const string BB_ISCHASETARGET = "IsChaseTarget";
        public const string BB_ISIDLE = "IsIdle";
        public const string BB_ISBOSSATTACK = "IsBossAttack";
        public const string BB_ISTARGETWITHINNPCATTACKRADIUS= "IsTargetWithinNpcAttackRadius";
        
        public const string BB_ISTreeStoped = "IsTreeStoped";
        #endregion
    }
}
