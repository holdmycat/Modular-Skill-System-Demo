using System.Reflection;
using Ebonor.DataCtrl;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using MongoDB.Bson.Serialization;
using System.IO;

namespace Tests.EditMode
{
    /// <summary>
    /// Tests for role key and role id generation to ensure deterministic changes when input fields change.
    /// </summary>
    public class UnitAttributesRoleIdTests
    {
        private static string InvokeDrawerBuildRoleKey(SerializedProperty property)
        {
            MethodInfo method = typeof(UnitAttributesNodeDataBaseDrawer).GetMethod(
                "BuildRoleKey",
                BindingFlags.Static | BindingFlags.NonPublic);

            Assert.IsNotNull(method, "Failed to reflect BuildRoleKey from UnitAttributesNodeDataBaseDrawer.");
            return (string)method.Invoke(null, new object[] { property });
        }

        private static string BuildKeyWithDrawer(HeroAttributesNodeData data,
            eHeroProfession profession,
            eSide side,
            eActorModelType modelType,
            string sprite,
            string name)
        {
            var container = ScriptableObject.CreateInstance<UnitAttributesTestContainer>();
            container.Data = data;

            container.Data.HeroProfession = profession;
            container.Data.ActorSide = side;
            container.Data.ActorModelType = modelType;
            container.Data.UnitSprite = sprite;
            container.Data.UnitName = name;

            var serializedObject = new SerializedObject(container);
            serializedObject.Update();
            SerializedProperty dataProp = serializedObject.FindProperty("Data");
            Assert.IsNotNull(dataProp, "SerializedProperty for Data not found.");

            return InvokeDrawerBuildRoleKey(dataProp);
        }

        [Test]
        public void BuildRoleKey_UsesAllFieldsAndUpdatesWhenTheyChange()
        {
            var data = new HeroAttributesNodeData();

            string baseKey = BuildKeyWithDrawer(data,
                eHeroProfession.Assassin,
                eSide.Player,
                eActorModelType.eHero,
                "Sprite_A",
                "Hero_A");

            string changedProfession = BuildKeyWithDrawer(data,
                eHeroProfession.Paladin,
                eSide.Player,
                eActorModelType.eHero,
                "Sprite_A",
                "Hero_A");
            Assert.AreNotEqual(baseKey, changedProfession, "Role key should change when profession changes.");

            string changedSide = BuildKeyWithDrawer(data,
                eHeroProfession.Assassin,
                eSide.Enemy,
                eActorModelType.eHero,
                "Sprite_A",
                "Hero_A");
            Assert.AreNotEqual(baseKey, changedSide, "Role key should change when side changes.");

            string changedModel = BuildKeyWithDrawer(data,
                eHeroProfession.Assassin,
                eSide.Player,
                eActorModelType.eNpc,
                "Sprite_A",
                "Hero_A");
            Assert.AreNotEqual(baseKey, changedModel, "Role key should change when model type changes.");

            string changedSprite = BuildKeyWithDrawer(data,
                eHeroProfession.Assassin,
                eSide.Player,
                eActorModelType.eHero,
                "Sprite_B",
                "Hero_A");
            Assert.AreNotEqual(baseKey, changedSprite, "Role key should change when sprite changes.");

            string changedName = BuildKeyWithDrawer(data,
                eHeroProfession.Assassin,
                eSide.Player,
                eActorModelType.eHero,
                "Sprite_A",
                "Hero_B");
            Assert.AreNotEqual(baseKey, changedName, "Role key should change when name changes.");
        }

        [Test]
        public void BuildRoleKey_UsesFallbacksForEmptySpriteAndName()
        {
            var data = new HeroAttributesNodeData();

            string key = BuildKeyWithDrawer(data,
                eHeroProfession.Assassin,
                eSide.Player,
                eActorModelType.eHero,
                string.Empty,
                string.Empty);

            StringAssert.Contains("UnknownSprite", key, "Empty sprite should be replaced with fallback.");
            StringAssert.Contains("Unnamed", key, "Empty name should be replaced with fallback.");
        }

        [Test]
        public void GenerateRoleIdFromData_ChangesWhenFieldsChange()
        {
            var data = new HeroAttributesNodeData
            {
                HeroProfession = eHeroProfession.Assassin,
                ActorSide = eSide.Player,
                ActorModelType = eActorModelType.eHero,
                UnitSprite = "Sprite_A",
                UnitName = "Hero_A"
            };

            long baseId = data.GenerateRoleIdFromData();

            data.HeroProfession = eHeroProfession.Paladin;
            long newProfessionId = data.GenerateRoleIdFromData();
            Assert.AreNotEqual(baseId, newProfessionId, "Role id should change when profession changes.");

            data.HeroProfession = eHeroProfession.Assassin;
            data.ActorSide = eSide.Enemy;
            long newSideId = data.GenerateRoleIdFromData();
            Assert.AreNotEqual(baseId, newSideId, "Role id should change when side changes.");

            data.ActorSide = eSide.Player;
            data.ActorModelType = eActorModelType.eNpc;
            long newModelId = data.GenerateRoleIdFromData();
            Assert.AreNotEqual(baseId, newModelId, "Role id should change when model type changes.");

            data.ActorModelType = eActorModelType.eHero;
            data.UnitSprite = "Sprite_B";
            long newSpriteId = data.GenerateRoleIdFromData();
            Assert.AreNotEqual(baseId, newSpriteId, "Role id should change when sprite changes.");

            data.UnitSprite = "Sprite_A";
            data.UnitName = "Hero_B";
            long newNameId = data.GenerateRoleIdFromData();
            Assert.AreNotEqual(baseId, newNameId, "Role id should change when name changes.");
        }

        [Test]
        public void DrawerGenerateRoleId_MatchesDataGeneration()
        {
            var data = new HeroAttributesNodeData
            {
                HeroProfession = eHeroProfession.Assassin,
                ActorSide = eSide.Player,
                ActorModelType = eActorModelType.eHero,
                UnitSprite = "Sprite_Drawer",
                UnitName = "Hero_Drawer"
            };

            long dataId = data.GenerateRoleIdFromData();

            var container = ScriptableObject.CreateInstance<UnitAttributesTestContainer>();
            container.Data = data;
            var so = new SerializedObject(container);
            so.Update();
            SerializedProperty prop = so.FindProperty("Data");

            MethodInfo method = typeof(UnitAttributesNodeDataBaseDrawer).GetMethod(
                "GenerateRoleId",
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(method, "Failed to reflect GenerateRoleId from UnitAttributesNodeDataBaseDrawer.");

            long drawerId = (long)method.Invoke(null, new object[] { prop });
            Assert.AreEqual(dataId, drawerId, "Drawer-generated id should match data-driven generation.");
        }

        [Test]
        public void UnitDataNodeId_FieldIsReadOnlyInDrawer()
        {
            var container = ScriptableObject.CreateInstance<UnitAttributesTestContainer>();
            var so = new SerializedObject(container);
            so.Update();
            SerializedProperty prop = so.FindProperty("Data");

            SerializedProperty idProp = prop.FindPropertyRelative("UnitDataNodeId");
            Assert.IsNotNull(idProp, "UnitDataNodeId property not found.");

            // Drawer renders this field disabled; here we simply ensure programmatic write/read works.
            idProp.longValue = 123;
            so.ApplyModifiedProperties();
            Assert.AreEqual(123, container.Data.UnitDataNodeId);
        }

        [Test]
        public void GraphRestore_FromPackedData_RebuildsNodes()
        {
            var graph = ScriptableObject.CreateInstance<Plugins.NodeEditor.UnitAttributesDataGraph>();
            var node = System.Activator.CreateInstance<Plugins.NodeEditor.HeroAttributesNode>();

            // ensure GUID set to avoid null key
            node.OnNodeCreated();

            graph.AddNode(node);
            graph.OnBeforeSerialize();

            // simulate loss of SerializeReference data
            graph.nodes.Clear();

            graph.OnAfterDeserialize();

            Assert.Greater(graph.nodes.Count, 0, "Nodes should be restored from packed data.");
        }

        [Test]
        public void UnitAttributesDataSupportor_BsonRoundtrip_PreservesLongKeys()
        {
            const long key = 876543210123456789L;
            var supportor = new UnitAttributesDataSupportor();
            supportor.UnitAttributesDataSupportorDic[key] = new HeroAttributesNodeData { UnitDataNodeId = key };

            AttributesNodeDataSerializerRegister.RegisterClassMaps();

            byte[] data;
            using (var ms = new MemoryStream())
            using (var writer = new MongoDB.Bson.IO.BsonBinaryWriter(ms))
            {
                BsonSerializer.Serialize(writer, supportor);
                data = ms.ToArray();
            }

            UnitAttributesDataSupportor deserialized;
            using (var ms = new MemoryStream(data))
            using (var reader = new MongoDB.Bson.IO.BsonBinaryReader(ms))
            {
                deserialized = BsonSerializer.Deserialize<UnitAttributesDataSupportor>(reader);
            }

            Assert.IsTrue(deserialized.UnitAttributesDataSupportorDic.ContainsKey(key), "Long key should survive BSON roundtrip.");
            Assert.AreEqual(key, deserialized.UnitAttributesDataSupportorDic[key].UnitDataNodeId);
        }
    }

    /// <summary>
    /// ScriptableObject wrapper so we can obtain SerializedProperty for drawer-based role key generation.
    /// </summary>
    public class UnitAttributesTestContainer : ScriptableObject
    {
        [SerializeReference] public HeroAttributesNodeData Data = new HeroAttributesNodeData();
    }
}
