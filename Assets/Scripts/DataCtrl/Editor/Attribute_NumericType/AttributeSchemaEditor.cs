using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Ebonor.DataCtrl.Editor
{
    [CustomEditor(typeof(AttributeSchema))]
    public class AttributeSchemaEditor : UnityEditor.Editor
    {
        private const string GeneratedFilePath = "Assets/Scripts/DataCtrl/Generated/GeneratedNumericType.cs";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Code Generation", EditorStyles.boldLabel);

            if (GUILayout.Button("Generate Code", GUILayout.Height(30)))
            {
                GenerateCode((AttributeSchema)target);
            }
        }

        private void GenerateCode(AttributeSchema schema)
        {
            if (schema == null) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//------------------------------------------------------------");
            sb.AppendLine("// File: GeneratedNumericType.cs");
            sb.AppendLine("// Purpose: Auto-generated eNumericType enum from AttributeSchema.");
            sb.AppendLine("//------------------------------------------------------------");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("namespace Ebonor.DataCtrl");
            sb.AppendLine("{");
            sb.AppendLine("    public enum eNumericType");
            sb.AppendLine("    {");
            sb.AppendLine("        Min = 10000,");
            sb.AppendLine();

            foreach (var attr in schema.Attributes)
            {
                if (!string.IsNullOrEmpty(attr.Description))
                {
                    sb.AppendLine($"        /// <summary>");
                    sb.AppendLine($"        /// {attr.Description}");
                    sb.AppendLine($"        /// </summary>");
                }
                sb.AppendLine($"        [InspectorName(\"{attr.Name}\")]");
                sb.AppendLine($"        {attr.Name} = {attr.Id},");

                if (attr.Type == AttributeType.BaseAdd)
                {
                    sb.AppendLine($"        {attr.Name}Base = {attr.Name} * 10 + 1,");
                    sb.AppendLine($"        {attr.Name}Add = {attr.Name} * 10 + 2,");
                }
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            string dir = Path.GetDirectoryName(GeneratedFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(GeneratedFilePath, sb.ToString());
            AssetDatabase.Refresh();
            Debug.Log($"Generated eNumericType at {GeneratedFilePath}");
        }
    }
}
