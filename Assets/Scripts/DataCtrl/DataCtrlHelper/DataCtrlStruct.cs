using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    
    [Flags]
    public enum eBuffBindAnimStackState 
    {
        
        /// <summary>
        /// Death
        /// </summary>
        [LabelText("Death")]
        Die = 1<<20, // Highest priority

        [LabelText("Victory")]
        Victory = 1<<19, // Second highest priority
        
        /// <summary>
        /// Invincible
        /// </summary>
        [LabelText("Invincible")]
        Invincible = 1<<16, // High priority

        /// <summary>
        /// Waiting for buff execution; do nothing else. Highest priority.
        /// </summary>
        //WaitForBuff = 1<<15,
        
        // [LabelText("Teleport")]
        // Teleport = 1<<14, // Directly teleport the target to a position

        [LabelText("Frozen")]
        Frozen = 1<<13, // Frozen
        
        /// <summary>
        /// Knock up
        /// </summary>
        [LabelText("Knock Up")]
        KnockUp = 1<<12, // High priority

        /// <summary>
        /// Repulse
        /// </summary>
        [LabelText("Repulse")]
        Repulse = 1<<11, // Medium priority

        // [LabelText("Drag")]
        // Drag = 1<<10, // Drag target along a specific trajectory
        
        [LabelText("Pull")]
        Pull = 1<<9, // Pull target to caster position
        
        /// <summary>
        /// Stun
        /// </summary>
        [LabelText("Stunned")]
        Stunned = 1<<8, // Medium priority

        /// <summary>
        /// Hurt
        /// </summary>
        [LabelText("Hurt")]
        GetHurt = 1<<7, // Medium priority

        /// <summary>
        /// Disarm
        /// </summary>
        [LabelText("Disarmed")]
        Disarmed = 1<<6, // Medium priority
        
        // [LabelText("Request Cast Skill")]
        // RequestCastSkill = 1<<5,  // Low priority
        
        /// <summary>
        /// Cast skill
        /// </summary>
        [LabelText("Cast Skill")]
        CastSkill = 1<<4, // Low priority

        /// <summary>
        /// Normal attack
        /// </summary>
        [LabelText("Normal Attack")]
        NormalAttack = 1<<3, // Low priority

        /// <summary>
        /// Move
        /// </summary>
        [LabelText("Move")]
        Chasing = 1<<2, // Low priority

        /// <summary>
        /// Idle: can move and play movement animation
        /// </summary>
        [LabelText("Idle")]
        Idle = 1<<1, // Low priority

        /// <summary>
        /// Birth
        /// </summary>
        [LabelText("Birth")]
        Birth = 1<< 0, // Lowest priority
        
        NullStateID = 0,
    }
}
