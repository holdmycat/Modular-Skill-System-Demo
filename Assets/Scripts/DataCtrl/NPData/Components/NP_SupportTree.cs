//------------------------------------------------------------
// File: NP_SupportTree.cs
// Created: 2025-12-05
// Purpose: MonoBehaviour wrapper for support-skill behaviour tree data.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    
    public class NP_SupportTree : MonoBehaviour
    {
        static readonly ILog log = LogManager.GetLogger(typeof(NP_SupportTree));

        private NP_SupportSkillDataSupportor mSupportSkillDataSupporter;
        
        public NP_SupportSkillDataSupportor SupportSkillDataSupporter => mSupportSkillDataSupporter;
        
        public void OnInitSupportData(NP_SupportSkillDataSupportor support)
        {
            mSupportSkillDataSupporter = support;
        }
    }
}
