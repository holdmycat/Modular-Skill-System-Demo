//------------------------------------------------------------
// File: UnitAttributesNodeDataBase.cs
// Created: 2025-11-29
// Purpose: Base class for unit attribute node data and shared metadata.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Ebonor.Framework;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Unit base attribute data (optimized version)
    /// </summary>
    [System.Serializable]
    [BsonSerializer(typeof(AttributesDataSerializer<UnitAttributesNodeDataBase>))]
    public abstract class UnitAttributesNodeDataBase : ICommonAttributeBase
    {
        #region ConstructionAndInitialization
        public UnitAttributesNodeDataBase()
        {
            OnInit();
        }
        public virtual void OnInit()
        {
            mStrBld = new StringBuilder();
        }
        protected StringBuilder mStrBld;
        #endregion
        
        #region UtilityProperties
        
        [BsonElement("typeStr")]
        [HideInInspector]
        public string typeStr = "";

        /// <summary>
        /// Compose a role key from profession, side, model type, sprite, and name.
        /// </summary>
        public virtual string BuildRoleKey()
        {
            string profession = HeroProfession.ToString();
            string side = ActorSide.ToString();
            string model = ActorModelType.ToString();
            string sprite = string.IsNullOrEmpty(UnitSprite) ? "UnknownSprite" : UnitSprite;
            string name = string.IsNullOrEmpty(UnitName) ? "Unnamed" : UnitName;
            return $"{profession}_{side}_{model}_{sprite}_{name}";
        }

        /// <summary>
        /// Generate a deterministic role id based on the current role key and assign it to UnitDataNodeId.
        /// </summary>
        public virtual long GenerateRoleIdFromData()
        {
            string roleKey = BuildRoleKey();
            long roleId = GlobalHelper.GetRoleID(roleKey);

            if (roleId < 0)
            {
                roleId = 0; // Keep ids non-negative.
            }

            UnitDataNodeId = roleId;
            return roleId;
        }

        /// <summary>
        /// Hook for committing attribute changes; override in subclasses that need persistence.
        /// </summary>
        public virtual void CommitRoleChanges()
        {
            // Intentionally left empty; editors can call this to trigger persistence hooks.
        }

        #endregion
        
        #region ========== PrimaryConfigurationArea ==========
       
        // ===== top Base Attributes =====
        [BsonElement("UnitDataNodeId")]
        [BsonRepresentation(BsonType.Int64)] // prevent overflow when value exceeds int32 during BSON serialization
        public long UnitDataNodeId;
        
        [BsonElement("ActorModelType")]
        public eActorModelType ActorModelType;
        
        [BsonElement("ActorSide")]
        public eSide ActorSide;
        //
        // // Hero only
        [BsonElement("HeroProfession")]
        public eHeroProfession HeroProfession;
        //
        // public eHeroType HeroType;
        //
        // public eCoreAttributeType CharacterCoreAttributeType;
        //
        // // NPC only
        // [BsonElement("NpcProfession")]
        // public eNpcProfession NpcProfession;
        //
        // Basic info
        [BsonElement("UnitName")]
        public string UnitName;

        [BsonElement("UnitSprite")]
        public string UnitSprite;
        
        [BsonElement("UnitAvatar")]
        public string UnitAvatar;
        
        [BsonElement("UnitMaxLV")]
        public uint UnitMaxLV;
        
        [BsonElement("ActiveSkillNum")]
        public uint ActiveSkillNum;
        
        [BsonElement("CanChase")]
        public bool CanChase;
        
        // Physical dimensions
        [BsonElement("Height")]
        public float Height;
        
        [BsonElement("Radius")]
        public float Radius;
        
        [BsonElement("Scale")]
        public float Scale;

        // ===== Tab 1: Core Attributes =====
        [BsonElement("Power")]
        public float Power;
        
        [BsonElement("Agility")]
        public float Agility;
        
        [BsonElement("Vitality")]
        public float Vitality;
        
        public float PowerGrowthRate;
        
        public float AgilityGrowthRate;
        
        
        public float VitalityGrowthRate;

        // ===== Tab 2: Combat Attributes =====
        public float AttackDamge;
        
        public float LifeStealRate;
        
        [HideInInspector]
        public float BaseAttackTime = 1.7f;
        
        [Range(100f, 150f)]
        public float BaseAttackSpeed;
        
        [Range(0f, 200f)]
        public float MagicAttackDamageBonus;
        
        public float Armour;
        
        public float ElementsResistance;
        
        public float DodgeRate;

        [BsonElement("MovementSpeed")]
        public float MovementSpeed;

        [BsonElement("RotationSpeed")]
        public float RotationSpeed;
        
        // ===== Tab 3: Behavior Tree Configuration =====
        [BsonElement("HasBehaviourTree")]
        public bool HasBehaviourTree;
        
        [BsonElement("HasNormalAttackTree")]
        public bool HasNormalAttackTree;
        
        [BsonElement("HasSkillTree")]
        public bool HasSkillTree;
        
        // ===== Tab 4: Visual Effects =====
        [BsonElement("ActorBirthEffectName")]
        public string ActorBirthEffectName;

        [BsonElement("ActorFootStepName")]
        public string ActorFootStepName;
        
        
        // ===== Tab 5: Spawn configuration =====
        [BsonElement("BirthDuration")]
        public float BirthDuration;
        
        [BsonElement("HasBirthAnimation")]
        public bool HasBirthAnimation;
        
        [BsonElement("BirthCanbeLocked")]
        public bool BirthCanbeLocked;
        
        [BsonElement("BirthCanbeSeen")]
        public bool BirthCanbeSeen;
        
        // ===== Tab 6: Death configuration =====
        [BsonElement("DeathDelayDuration")]
        public float DeathDelayDuration;
        
        [BsonElement("DeathRepulseDistance")]
        public float DeathRepulseDistance;
        
        [BsonElement("DeathRepulseDuration")]
        public float DeathRepulseDuration;
        
        #endregion
    }

#if UNITY_EDITOR
    /// <summary>
    /// Inspector helper that keeps the base attributes in a foldout and organizes the rest into tabs without relying on Odin.
    /// </summary>
    [CustomPropertyDrawer(typeof(UnitAttributesNodeDataBase), true)]
    public class UnitAttributesNodeDataBaseDrawer : PropertyDrawer
    {
        private static readonly string[] BaseAttributePropertyNames =
        {
            "UnitDataNodeId",
            "ActorModelType",
            "ActorSide",
            "HeroProfession",
            "UnitName",
            "UnitSprite",
            "UnitAvatar",
            "UnitMaxLV",
            "ActiveSkillNum",
            "CanChase",
            "Height",
            "Radius",
            "Scale",
            "WeaponTypeIndex",
            "HeroActorIndex",
            "NpcActorIndex"
        };

        private static readonly string[] TabTitles =
        {
            "Core Attributes",
            "Combat",
            "Behaviour Tree",
            "Visual Effects",
            "Spawn",
            "Death"
        };

        private static readonly string[][] TabPropertyNames =
        {
            new[]
            {
                "Power",
                "Agility",
                "Vitality",
                "PowerGrowthRate",
                "AgilityGrowthRate",
                "VitalityGrowthRate"
            },
            new[]
            {
                "AttackDamge",
                "LifeStealRate",
                "BaseAttackSpeed",
                "MagicAttackDamageBonus",
                "Armour",
                "ElementsResistance",
                "DodgeRate",
                "MovementSpeed",
                "RotationSpeed"
            },
            new[]
            {
                "HasBehaviourTree",
                "HasNormalAttackTree",
                "HasSkillTree"
            },
            new[]
            {
                "ActorBirthEffectName",
                "ActorFootStepName"
            },
            new[]
            {
                "BirthDuration",
                "HasBirthAnimation",
                "BirthCanbeLocked",
                "BirthCanbeSeen"
            },
            new[]
            {
                "DeathDelayDuration",
                "DeathRepulseDistance",
                "DeathRepulseDuration"
            }
        };

        private static readonly Dictionary<string, bool> BaseFoldoutStates = new Dictionary<string, bool>();
        private static readonly Dictionary<string, int> TabSelectionStates = new Dictionary<string, int>();

        private const float ButtonRowPadding = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight; // main foldout line
            if (!property.isExpanded)
            {
                return height;
            }

            string propertyKey = property.propertyPath;
            height += EditorGUIUtility.standardVerticalSpacing;

            // Base attributes foldout header + content
            height += EditorGUIUtility.singleLineHeight;
            if (IsBaseFoldoutOpen(propertyKey))
            {
                float baseHeight = GetGroupedPropertyHeight(property, BaseAttributePropertyNames);
                if (baseHeight > 0f)
                {
                    height += EditorGUIUtility.standardVerticalSpacing + baseHeight;
                }
            }

            height += EditorGUIUtility.standardVerticalSpacing;

            // Toolbar + active tab content
            height += EditorGUIUtility.singleLineHeight;

            string[] activeTabProperties = GetActiveTabProperties(propertyKey);
            float tabContentHeight = GetGroupedPropertyHeight(property, activeTabProperties);
            if (tabContentHeight > 0f)
            {
                height += EditorGUIUtility.standardVerticalSpacing + tabContentHeight;
            }

            // Buttons row
            height += EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect lineRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(lineRect, property.isExpanded, label, true);

            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            float yOffset = lineRect.yMax + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel++;

            string propertyKey = property.propertyPath;
            Rect baseHeaderRect = EditorGUI.IndentedRect(new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight));
            bool baseFoldout = DrawBaseFoldout(baseHeaderRect, IsBaseFoldoutOpen(propertyKey));
            SetBaseFoldoutState(propertyKey, baseFoldout);
            yOffset = baseHeaderRect.yMax;

            if (baseFoldout)
            {
                yOffset = DrawPropertyGroup(position, yOffset + EditorGUIUtility.standardVerticalSpacing, property, BaseAttributePropertyNames);
            }

            yOffset += EditorGUIUtility.standardVerticalSpacing;

            Rect tabRect = EditorGUI.IndentedRect(new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight));
            int selectedTab = DrawTabs(tabRect, GetTabIndex(propertyKey));
            SetTabIndex(propertyKey, selectedTab);

            yOffset = tabRect.yMax + EditorGUIUtility.standardVerticalSpacing;
            string[] activeTabProperties = GetActiveTabProperties(propertyKey);
            yOffset = DrawPropertyGroup(position, yOffset, property, activeTabProperties);

            yOffset += EditorGUIUtility.standardVerticalSpacing;
            DrawActionButtons(position, yOffset, property);

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        private static bool IsBaseFoldoutOpen(string key)
        {
            bool isOpen;
            if (!BaseFoldoutStates.TryGetValue(key, out isOpen))
            {
                isOpen = true;
                BaseFoldoutStates[key] = isOpen;
            }

            return isOpen;
        }

        private static void SetBaseFoldoutState(string key, bool isOpen)
        {
            BaseFoldoutStates[key] = isOpen;
        }

        private static int GetTabIndex(string key)
        {
            int index;
            if (!TabSelectionStates.TryGetValue(key, out index))
            {
                index = 0;
                TabSelectionStates[key] = index;
            }

            return Mathf.Clamp(index, 0, TabTitles.Length - 1);
        }

        private static void SetTabIndex(string key, int index)
        {
            TabSelectionStates[key] = Mathf.Clamp(index, 0, TabTitles.Length - 1);
        }

        private static bool DrawBaseFoldout(Rect rect, bool currentState)
        {
            bool nextState = EditorGUI.BeginFoldoutHeaderGroup(rect, currentState, new GUIContent("Base Attributes"));
            EditorGUI.EndFoldoutHeaderGroup();
            return nextState;
        }

        private static int DrawTabs(Rect rect, int currentIndex)
        {
            return GUI.Toolbar(rect, currentIndex, TabTitles);
        }

        private static float DrawPropertyGroup(Rect fullPosition, float startY, SerializedProperty rootProperty, IReadOnlyList<string> propertyNames)
        {
            float yCursor = startY;
            foreach (string propertyName in propertyNames)
            {
                SerializedProperty childProperty = rootProperty.FindPropertyRelative(propertyName);
                if (childProperty == null)
                {
                    continue;
                }

                bool isReadonlyId = propertyName == "UnitDataNodeId";
                float propertyHeight = EditorGUI.GetPropertyHeight(childProperty, !isReadonlyId);
                Rect propertyRect = new Rect(fullPosition.x, yCursor, fullPosition.width, propertyHeight);

                if (isReadonlyId)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.PropertyField(propertyRect, childProperty, true);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUI.PropertyField(propertyRect, childProperty, true);
                }

                yCursor += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return yCursor;
        }

        private static float GetGroupedPropertyHeight(SerializedProperty rootProperty, IReadOnlyList<string> propertyNames)
        {
            float height = 0f;
            bool hasContent = false;

            foreach (string propertyName in propertyNames)
            {
                SerializedProperty childProperty = rootProperty.FindPropertyRelative(propertyName);
                if (childProperty == null)
                {
                    continue;
                }

                hasContent = true;
                height += EditorGUI.GetPropertyHeight(childProperty, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            if (hasContent)
            {
                height -= EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        private static string[] GetActiveTabProperties(string propertyKey)
        {
            int activeIndex = GetTabIndex(propertyKey);
            if (activeIndex < 0 || activeIndex >= TabPropertyNames.Length)
            {
                return TabPropertyNames[0];
            }

            return TabPropertyNames[activeIndex];
        }

        private static void DrawActionButtons(Rect fullPosition, float startY, SerializedProperty property)
        {
            Rect buttonRow = EditorGUI.IndentedRect(new Rect(fullPosition.x, startY, fullPosition.width, EditorGUIUtility.singleLineHeight));
            float buttonWidth = (buttonRow.width - (ButtonRowPadding * 2f)) / 3f;

            Rect generateRect = new Rect(buttonRow.x, buttonRow.y, buttonWidth, buttonRow.height);
            Rect copyRect = new Rect(generateRect.xMax + ButtonRowPadding, buttonRow.y, buttonWidth, buttonRow.height);
            Rect commitRect = new Rect(copyRect.xMax + ButtonRowPadding, buttonRow.y, buttonWidth, buttonRow.height);

            if (GUI.Button(generateRect, "Generate Role ID"))
            {
                // Sync any pending inspector edits (e.g., changed profession) before computing the ID.
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();

                long generatedId = GenerateRoleId(property);
                Debug.Log($"Generated role id: {generatedId}");
                property.serializedObject.ApplyModifiedProperties();
            }

            if (GUI.Button(copyRect, "Copy Role ID"))
            {
                SerializedProperty idProp = property.FindPropertyRelative("UnitDataNodeId");
                if (idProp != null)
                {
                    EditorGUIUtility.systemCopyBuffer = idProp.longValue.ToString();
                    Debug.Log($"Copied role id: {EditorGUIUtility.systemCopyBuffer}");
                }
            }

            if (GUI.Button(commitRect, "Commit Role Changes"))
            {
                property.serializedObject.ApplyModifiedProperties();

                Object target = property.serializedObject.targetObject;
                if (target != null)
                {
                    EditorUtility.SetDirty(target);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Role changes committed and asset saved.");
                }
            }
        }

        private static long GenerateRoleId(SerializedProperty property)
        {
            string roleKey = BuildRoleKey(property);
            long roleId = GlobalHelper.GetRoleID(roleKey);

            SerializedProperty idProp = property.FindPropertyRelative("UnitDataNodeId");
            if (idProp != null)
            {
                idProp.longValue = roleId < 0 ? 0 : roleId;
            }

            return roleId;
        }

        private static string BuildRoleKey(SerializedProperty property)
        {
            string profession = GetEnumName(property.FindPropertyRelative("HeroProfession"));
            string side = GetEnumName(property.FindPropertyRelative("ActorSide"));
            string model = GetEnumName(property.FindPropertyRelative("ActorModelType"));
            SerializedProperty spriteProp = property.FindPropertyRelative("UnitSprite");
            SerializedProperty nameProp = property.FindPropertyRelative("UnitName");
            string sprite = spriteProp != null ? spriteProp.stringValue : "UnknownSprite";
            string name = nameProp != null ? nameProp.stringValue : "Unnamed";

            if (string.IsNullOrEmpty(sprite))
            {
                sprite = "UnknownSprite";
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "Unnamed";
            }

            return $"{profession}_{side}_{model}_{sprite}_{name}";
        }

        private static string GetEnumName(SerializedProperty enumProperty)
        {
            if (enumProperty == null || enumProperty.propertyType != SerializedPropertyType.Enum)
            {
                return "UnknownProfession";
            }

            // Use raw enum names (not display names) to match runtime BuildRoleKey().
            string[] names = enumProperty.enumNames;
            int index = Mathf.Clamp(enumProperty.enumValueIndex, 0, names.Length - 1);
            return names[index];
        }
    }
#endif
}
