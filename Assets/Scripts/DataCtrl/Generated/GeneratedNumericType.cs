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
        /// Unity Layer/Tag identifier
        /// </summary>
        [InspectorName("UnityProfession")]
        UnityProfession = 1001,

        /// <summary>
        /// Faction Side (0: Neutral, 1: Player, 2: Enemy)
        /// </summary>
        [InspectorName("ActorSide")]
        ActorSide = 1002,

        /// <summary>
        /// Current Level
        /// </summary>
        [InspectorName("UnitLv")]
        UnitLv = 1003,

        /// <summary>
        /// Max Level Cap
        /// </summary>
        [InspectorName("UnitMaxLv")]
        UnitMaxLv = 1004,

        /// <summary>
        /// Current HP / Max HP (Base + Add)
        /// </summary>
        [InspectorName("Hp")]
        Hp = 1010,
        HpBase = Hp * 10 + 1,
        HpAdd = Hp * 10 + 2,

        /// <summary>
        /// Physical Attack
        /// </summary>
        [InspectorName("Attack")]
        Attack = 1011,
        AttackBase = Attack * 10 + 1,
        AttackAdd = Attack * 10 + 2,

        /// <summary>
        /// Physical Defense
        /// </summary>
        [InspectorName("Defense")]
        Defense = 1012,
        DefenseBase = Defense * 10 + 1,
        DefenseAdd = Defense * 10 + 2,

        /// <summary>
        /// Attack Speed (Attacks per sec)
        /// </summary>
        [InspectorName("AttackSpeed")]
        AttackSpeed = 1013,
        AttackSpeedBase = AttackSpeed * 10 + 1,
        AttackSpeedAdd = AttackSpeed * 10 + 2,

        /// <summary>
        /// Current Soldier Count in Squad
        /// </summary>
        [InspectorName("SoldierCount")]
        SoldierCount = 1020,

        /// <summary>
        /// Max Soldier Count in Squad
        /// </summary>
        [InspectorName("SoldierMaxCount")]
        SoldierMaxCount = 1021,

        /// <summary>
        /// World Map March Speed
        /// </summary>
        [InspectorName("MarchSpeed")]
        MarchSpeed = 2001,
        MarchSpeedBase = MarchSpeed * 10 + 1,
        MarchSpeedAdd = MarchSpeed * 10 + 2,

        /// <summary>
        /// Resource Load Capacity
        /// </summary>
        [InspectorName("LoadCapacity")]
        LoadCapacity = 2002,
        LoadCapacityBase = LoadCapacity * 10 + 1,
        LoadCapacityAdd = LoadCapacity * 10 + 2,

        /// <summary>
        /// Stamina cost per action
        /// </summary>
        [InspectorName("StaminaCost")]
        StaminaCost = 2003,

        /// <summary>
        /// Infantry Atk % Bonus (100=1%)
        /// </summary>
        [InspectorName("InfantryAttackMod")]
        InfantryAttackMod = 2011,
        InfantryAttackModBase = InfantryAttackMod * 10 + 1,
        InfantryAttackModAdd = InfantryAttackMod * 10 + 2,

        /// <summary>
        /// Infantry Def % Bonus
        /// </summary>
        [InspectorName("InfantryDefenseMod")]
        InfantryDefenseMod = 2012,
        InfantryDefenseModBase = InfantryDefenseMod * 10 + 1,
        InfantryDefenseModAdd = InfantryDefenseMod * 10 + 2,

        /// <summary>
        /// Infantry HP % Bonus
        /// </summary>
        [InspectorName("InfantryHpMod")]
        InfantryHpMod = 2013,
        InfantryHpModBase = InfantryHpMod * 10 + 1,
        InfantryHpModAdd = InfantryHpMod * 10 + 2,

        /// <summary>
        /// Lancer Atk % Bonus
        /// </summary>
        [InspectorName("LancerAttackMod")]
        LancerAttackMod = 2021,
        LancerAttackModBase = LancerAttackMod * 10 + 1,
        LancerAttackModAdd = LancerAttackMod * 10 + 2,

        /// <summary>
        /// Lancer Def % Bonus
        /// </summary>
        [InspectorName("LancerDefenseMod")]
        LancerDefenseMod = 2022,
        LancerDefenseModBase = LancerDefenseMod * 10 + 1,
        LancerDefenseModAdd = LancerDefenseMod * 10 + 2,

        /// <summary>
        /// Lancer HP % Bonus
        /// </summary>
        [InspectorName("LancerHpMod")]
        LancerHpMod = 2023,
        LancerHpModBase = LancerHpMod * 10 + 1,
        LancerHpModAdd = LancerHpMod * 10 + 2,

        /// <summary>
        /// Marksman Atk % Bonus
        /// </summary>
        [InspectorName("MarksmanAttackMod")]
        MarksmanAttackMod = 2031,
        MarksmanAttackModBase = MarksmanAttackMod * 10 + 1,
        MarksmanAttackModAdd = MarksmanAttackMod * 10 + 2,

        /// <summary>
        /// Marksman Def % Bonus
        /// </summary>
        [InspectorName("MarksmanDefenseMod")]
        MarksmanDefenseMod = 2032,
        MarksmanDefenseModBase = MarksmanDefenseMod * 10 + 1,
        MarksmanDefenseModAdd = MarksmanDefenseMod * 10 + 2,

        /// <summary>
        /// Marksman HP % Bonus
        /// </summary>
        [InspectorName("MarksmanHpMod")]
        MarksmanHpMod = 2033,
        MarksmanHpModBase = MarksmanHpMod * 10 + 1,
        MarksmanHpModAdd = MarksmanHpMod * 10 + 2,

        /// <summary>
        /// Construction Speed %
        /// </summary>
        [InspectorName("ConstructionSpeed")]
        ConstructionSpeed = 3001,
        ConstructionSpeedBase = ConstructionSpeed * 10 + 1,
        ConstructionSpeedAdd = ConstructionSpeed * 10 + 2,

        /// <summary>
        /// Research Speed %
        /// </summary>
        [InspectorName("ResearchSpeed")]
        ResearchSpeed = 3002,
        ResearchSpeedBase = ResearchSpeed * 10 + 1,
        ResearchSpeedAdd = ResearchSpeed * 10 + 2,

        /// <summary>
        /// Troop Training Speed %
        /// </summary>
        [InspectorName("TrainingSpeed")]
        TrainingSpeed = 3003,
        TrainingSpeedBase = TrainingSpeed * 10 + 1,
        TrainingSpeedAdd = TrainingSpeed * 10 + 2,

    }
}
