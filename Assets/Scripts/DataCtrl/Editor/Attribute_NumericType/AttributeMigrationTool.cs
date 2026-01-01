using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ebonor.DataCtrl.Editor
{
    public class AttributeMigrationTool
    {
        [MenuItem("Custom Windows/Setup SLG Attribute Schema")]
        public static void SetupSLGSchema()
        {
            string assetPath = "Assets/Resources/ScriptableObject/AttributeSchema.asset";
            string dir = Path.GetDirectoryName(assetPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            AttributeSchema schema = AssetDatabase.LoadAssetAtPath<AttributeSchema>(assetPath);
            if (schema == null)
            {
                schema = ScriptableObject.CreateInstance<AttributeSchema>();
                AssetDatabase.CreateAsset(schema, assetPath);
            }

            schema.Attributes.Clear();
            
            // --- 1. Basic / RPG Attributes (1000 - 1999) ---
            // Used by Squads for Combat interactions
            AddAttr(schema, "UnityProfession", 1001, AttributeType.Final, "Unity Layer/Tag identifier");
            AddAttr(schema, "ActorSide", 1002, AttributeType.Final, "Faction Side (0: Neutral, 1: Player, 2: Enemy)");
            AddAttr(schema, "UnitLv", 1003, AttributeType.Final, "Current Level");
            AddAttr(schema, "UnitMaxLv", 1004, AttributeType.Final, "Max Level Cap");
            
            AddAttr(schema, "Hp", 1010, AttributeType.Base, "Current HP / Max HP (Base + Add)");
            AddAttr(schema, "Attack", 1011, AttributeType.Base, "Physical Attack");
            AddAttr(schema, "Defense", 1012, AttributeType.Base, "Physical Defense");
            AddAttr(schema, "AttackSpeed", 1013, AttributeType.Base, "Attack Speed (Attacks per sec)");
            
            AddAttr(schema, "SoldierCount", 1020, AttributeType.Final, "Current Soldier Count in Squad");
            AddAttr(schema, "SoldierMaxCount", 1021, AttributeType.Final, "Max Soldier Count in Squad");
            
            // --- 2. SLG Military Attributes (2000 - 2999) ---
            // Used by Commander (Modifiers) and Legion (March Stats)
            
            // March Physics
            AddAttr(schema, "MarchSpeed", 2001, AttributeType.Base, "World Map March Speed");
            AddAttr(schema, "LoadCapacity", 2002, AttributeType.Base, "Resource Load Capacity");
            AddAttr(schema, "StaminaCost", 2003, AttributeType.Final, "Stamina cost per action");

            // Unit Type Modifiers (Percent, usually FinalOnly, e.g. 1000 = 10%)
            // These gather inputs from Tech, Gear, VIP.
            // Changed to BaseAdd to support separation of "Permanent Base" (Tech) vs "Temporary" (Buffs) if needed.
            AddAttr(schema, "InfantryAttackMod", 2011, AttributeType.Base, "Infantry Atk % Bonus (100=1%)");
            AddAttr(schema, "InfantryDefenseMod", 2012, AttributeType.Base, "Infantry Def % Bonus");
            AddAttr(schema, "InfantryHpMod", 2013, AttributeType.Base, "Infantry HP % Bonus");
            
            AddAttr(schema, "LancerAttackMod", 2021, AttributeType.Base, "Lancer Atk % Bonus");
            AddAttr(schema, "LancerDefenseMod", 2022, AttributeType.Base, "Lancer Def % Bonus");
            AddAttr(schema, "LancerHpMod", 2023, AttributeType.Base, "Lancer HP % Bonus");
            
            AddAttr(schema, "MarksmanAttackMod", 2031, AttributeType.Base, "Marksman Atk % Bonus");
            AddAttr(schema, "MarksmanDefenseMod", 2032, AttributeType.Base, "Marksman Def % Bonus");
            AddAttr(schema, "MarksmanHpMod", 2033, AttributeType.Base, "Marksman HP % Bonus");

            // --- 3. SLG Economy Attributes (3000 - 3999) ---
            // Commander Level
            AddAttr(schema, "ConstructionSpeed", 3001, AttributeType.Base, "Construction Speed %");
            AddAttr(schema, "ResearchSpeed", 3002, AttributeType.Base, "Research Speed %");
            AddAttr(schema, "TrainingSpeed", 3003, AttributeType.Base, "Troop Training Speed %");


            EditorUtility.SetDirty(schema);
            AssetDatabase.SaveAssets();
            Debug.Log($"[AttributeMigrationTool] Set up {schema.Attributes.Count} SLG attributes in {assetPath}");
        }

        private static void AddAttr(AttributeSchema schema, string name, int id, AttributeType type, string desc)
        {
            schema.Attributes.Add(new AttributeDefinition
            {
                Name = name,
                Id = id,
                Type = type,
                Description = desc
            });
        }
    }
}
