//------------------------------------------------------------
// File: GeneratedNumericType.cs
// Purpose: Auto-generated eNumericType enum from AttributeSchema.
//------------------------------------------------------------
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public enum eNumericType
    {
        Min = 10000,

        /// <summary>
        /// UnityProfession
        /// </summary>
        [InspectorName("UnityProfession")]
        UnityProfession = 1001,

        /// <summary>
        /// ActorSide
        /// </summary>
        [InspectorName("ActorSide")]
        ActorSide = 1002,

        /// <summary>
        /// UnitLv
        /// </summary>
        [InspectorName("UnitLv")]
        UnitLv = 1003,

        /// <summary>
        /// UnitMaxLV
        /// </summary>
        [InspectorName("UnitMaxLV")]
        UnitMaxLV = 1004,

        /// <summary>
        /// Height
        /// </summary>
        [InspectorName("Height")]
        Height = 1005,

        /// <summary>
        /// Radius
        /// </summary>
        [InspectorName("Radius")]
        Radius = 1006,

        /// <summary>
        /// Power
        /// </summary>
        [InspectorName("Power")]
        Power = 1021,
        PowerBase = Power * 10 + 1,
        PowerAdd = Power * 10 + 2,

        /// <summary>
        /// Agility
        /// </summary>
        [InspectorName("Agility")]
        Agility = 1022,
        AgilityBase = Agility * 10 + 1,
        AgilityAdd = Agility * 10 + 2,

        /// <summary>
        /// Vitality
        /// </summary>
        [InspectorName("Vitality")]
        Vitality = 1023,
        VitalityBase = Vitality * 10 + 1,
        VitalityAdd = Vitality * 10 + 2,

        /// <summary>
        /// Life
        /// </summary>
        [InspectorName("Life")]
        Life = 1024,

        /// <summary>
        /// MaxLife
        /// </summary>
        [InspectorName("MaxLife")]
        MaxLife = 1025,

        /// <summary>
        /// MovementSpeed
        /// </summary>
        [InspectorName("MovementSpeed")]
        MovementSpeed = 1041,
        MovementSpeedBase = MovementSpeed * 10 + 1,
        MovementSpeedAdd = MovementSpeed * 10 + 2,

        /// <summary>
        /// RotationSpeed
        /// </summary>
        [InspectorName("RotationSpeed")]
        RotationSpeed = 1042,
        RotationSpeedBase = RotationSpeed * 10 + 1,
        RotationSpeedAdd = RotationSpeed * 10 + 2,

        /// <summary>
        /// RunningAnimSpeed
        /// </summary>
        [InspectorName("RunningAnimSpeed")]
        RunningAnimSpeed = 1043,

        /// <summary>
        /// DropFlyingDuration
        /// </summary>
        [InspectorName("DropFlyingDuration")]
        DropFlyingDuration = 1044,

    }
}
