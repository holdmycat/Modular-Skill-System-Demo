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
        /// profession
        /// </summary>
        [InspectorName("UnityProfession")]
        UnityProfession = 1001,

        /// <summary>
        /// faction/side
        /// </summary>
        [InspectorName("ActorSide")]
        ActorSide = 1002,

        /// <summary>
        /// level (in-session)
        /// </summary>
        [InspectorName("UnitLv")]
        UnitLv = 1003,

        /// <summary>
        /// max level
        /// </summary>
        [InspectorName("UnitMaxLV")]
        UnitMaxLV = 1004,

        [InspectorName("Height")]
        Height = 1005,

        [InspectorName("Radius")]
        Radius = 1006,

        /// <summary>
        /// Strength
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
        /// animation speed
        /// </summary>
        [InspectorName("RunningAnimSpeed")]
        RunningAnimSpeed = 1043,

        /// <summary>
        /// drop flight duration
        /// </summary>
        [InspectorName("DropFlyingDuration")]
        DropFlyingDuration = 1044,

    }
}
