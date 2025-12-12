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
        [MenuItem("Custom Windows/Migrate Attributes to Schema")]
        public static void Migrate()
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

            var enumValues = Enum.GetValues(typeof(eNumericType)).Cast<eNumericType>().Distinct().OrderBy(e => (int)e).ToList();
            var enumNames = Enum.GetNames(typeof(eNumericType)).ToList();

            HashSet<int> processedIds = new HashSet<int>();

            foreach (var val in enumValues)
            {
                int intVal = (int)val;
                if (intVal < 10000) continue; // Skip Min and below if any

                // Skip if it looks like a Base or Add variant (ends in 1 or 2 and is > 100000? No, logic is * 10 + 1)
                // Example: Power = 1021. PowerBase = 10211.
                // So if intVal is 10211, parent is 1021.
                // Check if this value is a child
                if (IsChildValue(intVal))
                {
                    continue;
                }

                if (processedIds.Contains(intVal)) continue;

                string name = val.ToString();
                
                // Check if Base/Add variants exist
                int baseId = intVal * 10 + 1;
                int addId = intVal * 10 + 2;

                bool hasBase = Enum.IsDefined(typeof(eNumericType), baseId);
                bool hasAdd = Enum.IsDefined(typeof(eNumericType), addId);

                AttributeDefinition def = new AttributeDefinition();
                def.Name = name;
                def.Id = intVal;
                def.Type = (hasBase && hasAdd) ? AttributeType.BaseAdd : AttributeType.FinalOnly;
                def.Description = name; // Default description

                schema.Attributes.Add(def);
                processedIds.Add(intVal);
            }

            EditorUtility.SetDirty(schema);
            AssetDatabase.SaveAssets();
            Debug.Log($"Migrated {schema.Attributes.Count} attributes to {assetPath}");
        }

        private static bool IsChildValue(int val)
        {
            // A child value comes from Parent * 10 + 1 or + 2.
            // So (val - 1) / 10 should be a valid parent ID?
            // Or simpler: check if the last digit is 1 or 2, AND the parent exists in the enum.
            
            int lastDigit = val % 10;
            if (lastDigit == 1 || lastDigit == 2)
            {
                int parentId = val / 10;
                // Check if parentId exists in eNumericType
                if (Enum.IsDefined(typeof(eNumericType), parentId))
                {
                    // It is a child only if the parent actually exists.
                    // Example: 10211 -> parent 1021 (Power). Power exists. So 10211 is child.
                    // Example: 1001 (UnityProfession). 1001 % 10 = 1. Parent 100. Does 100 exist? Likely not.
                    return true;
                }
            }
            return false;
        }
    }
}
