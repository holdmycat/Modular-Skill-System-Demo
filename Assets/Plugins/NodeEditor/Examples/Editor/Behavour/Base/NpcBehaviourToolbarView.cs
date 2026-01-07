using System.IO;
using GraphProcessor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Plugins.NodeEditor
{
    public class NpcBehaviourToolbarView : NPBehaveToolbarView
    {
        public NpcBehaviourToolbarView(BaseGraphView graphView, MiniMap miniMap, BaseGraph baseGraph) : base(graphView, miniMap, baseGraph)
        {
        }

        protected override void AddButtons()
        {
            base.AddButtons();

            AddButton(new GUIContent("CopyAsset", "在当前资源目录新建一个同名拷贝，前缀为 1-"),
                () =>
                {
                    var sourcePath = AssetDatabase.GetAssetPath(m_BaseGraph);
                    if (string.IsNullOrEmpty(sourcePath))
                    {
                        Debug.LogWarning("CopyAsset failed: source asset path is empty.");
                        return;
                    }

                    var targetPath = BuildCopyPath(sourcePath);
                    if (!AssetDatabase.CopyAsset(sourcePath, targetPath))
                    {
                        Debug.LogWarning($"CopyAsset failed: {sourcePath} -> {targetPath}");
                        return;
                    }

                    AssetDatabase.ImportAsset(targetPath);
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(targetPath);
                }, false);
        }

        private static string BuildCopyPath(string sourcePath)
        {
            var dir = Path.GetDirectoryName(sourcePath) ?? string.Empty;
            var fileName = Path.GetFileName(sourcePath);
            var targetPath = Path.Combine(dir, $"1-{fileName}");
            return AssetDatabase.GenerateUniqueAssetPath(targetPath);
        }
    }
}
