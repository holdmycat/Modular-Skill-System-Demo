//------------------------------------------------------------
// File: eUnitNumericType.cs
// Purpose: SLG-specific numeric identifiers to keep strategy attributes
//          separate from the action/hero numeric set.
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Numeric keys for SLG unit stats. Keep distinct from eNumericType (action game).
    /// </summary>
    public enum eUnitNumericType
    {
        Min = 20000,

        CombatPositionType = 2001,
        UnitClassType = 2002,
        ArmorType = 2003,
        DamageType = 2004,

        BaseHp = 2021,
        Attack = 2022,
        AttackRange = 2023,
        MoveSpeed = 2024,
        PopulationCost = 2025,
        SquadSize = 2026,
    }
}
