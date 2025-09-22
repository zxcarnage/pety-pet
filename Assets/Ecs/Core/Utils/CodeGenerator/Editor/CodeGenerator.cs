#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ecs.Core.Utils.CodeGenerator.Editor
{
    public class CodeGenerator : EditorWindow
    {
        [MenuItem("Tools/Generate Component Extensions")]
        public static void GenerateExtensions()
        {
            var window = GetWindow<CodeGenerator>("Code Generator");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Component Extension Generator", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Generate All Extensions"))
            {
                GenerateAllExtensions();
            }
            
            GUILayout.Space(10);
            GUILayout.Label("This tool will generate extension methods for all components marked with [Generate] attribute.");
        }

        private void GenerateAllExtensions()
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var componentTypes = assemblies
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.GetCustomAttribute<GenerateAttribute>() != null)
                    .ToList();

                if (componentTypes.Count == 0)
                {
                    Debug.LogWarning("No components found with [Generate] attribute.");
                    return;
                }

                var generatedPath = "Assets/Ecs/Generated/Components";
                if (!Directory.Exists(generatedPath))
                {
                    Directory.CreateDirectory(generatedPath);
                    AssetDatabase.Refresh();
                }

                var outputFile = Path.Combine(generatedPath, "ComponentsExtensions.cs");
                try
                {
                    var oldFiles = Directory.GetFiles(generatedPath, "*Extensions.cs");
                    foreach (var old in oldFiles)
                    {
                        if (!string.Equals(old, outputFile, StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(old);
                        }
                    }
                }
                catch (Exception cleanupEx)
                {
                    Debug.LogWarning($"Cleanup of old extension files failed: {cleanupEx.Message}");
                }

                var uniqueNamespaces = componentTypes
                    .Select(t => t.Namespace)
                    .Where(ns => !string.IsNullOrEmpty(ns))
                    .Distinct()
                    .OrderBy(ns => ns)
                    .ToList();

                var writer = new System.Text.StringBuilder();
                writer.AppendLine("using Scellecs.Morpeh;");
                foreach (var ns in uniqueNamespaces)
                {
                    writer.Append("using ").Append(ns).AppendLine(";");
                }
                writer.AppendLine();
                writer.AppendLine("namespace Ecs.Generated.Components");
                writer.AppendLine("{");
                writer.AppendLine("    public static class ComponentsExtensions");
                writer.AppendLine("    {");
                writer.AppendLine("        private static World _world;");
                writer.AppendLine();
                writer.AppendLine("        public static void Build(World world)");
                writer.AppendLine("        {");
                writer.AppendLine("            _world = world;");
                writer.AppendLine("        }");
                writer.AppendLine();

                foreach (var componentType in componentTypes.OrderBy(t => t.Name))
                {
                    var componentName = componentType.Name;
                    writer.Append("        public static Entity Add").Append(componentName).Append("(this Entity entity, ").Append(componentType.Name).AppendLine(" component)");
                    writer.AppendLine("        {");
                    writer.Append("            _world.GetStash<").Append(componentType.Name).AppendLine(">().Add(entity, component);");
                    writer.AppendLine("            return entity;");
                    writer.AppendLine("        }");
                    writer.AppendLine();
                    writer.Append("        public static Entity Set").Append(componentName).Append("(this Entity entity, ").Append(componentType.Name).AppendLine(" component)");
                    writer.AppendLine("        {");
                    writer.Append("            _world.GetStash<").Append(componentType.Name).AppendLine(">().Set(entity, component);");
                    writer.AppendLine("            return entity;");
                    writer.AppendLine("        }");
                    writer.AppendLine();
                    writer.Append("        public static Entity Remove").Append(componentName).AppendLine("(this Entity entity)");
                    writer.AppendLine("        {");
                    writer.Append("            _world.GetStash<").Append(componentType.Name).AppendLine(">().Remove(entity);");
                    writer.AppendLine("            return entity;");
                    writer.AppendLine("        }");
                    writer.AppendLine();
                    writer.Append("        public static bool Has").Append(componentName).AppendLine("(this Entity entity)");
                    writer.AppendLine("        {");
                    writer.Append("            return _world.GetStash<").Append(componentType.Name).AppendLine(">().Has(entity);");
                    writer.AppendLine("        }");
                    writer.AppendLine();
                    writer.Append("        public static ").Append(componentType.Name).Append(" Get").Append(componentName).AppendLine("(this Entity entity)");
                    writer.AppendLine("        {");
                    writer.Append("            return _world.GetStash<").Append(componentType.Name).AppendLine(">().Get(entity);");
                    writer.AppendLine("        }");
                    writer.AppendLine();
                }

                writer.AppendLine("    }");
                writer.AppendLine("}");

                File.WriteAllText(outputFile, writer.ToString());
                AssetDatabase.Refresh();
                Debug.Log($"Successfully generated component extensions in single file: {outputFile}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error generating extensions: {e.Message}");
            }
        }
    }
}
#endif
