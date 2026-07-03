#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Blaze.Runtime.Cms;
using UnityEditor.SearchService;
using System;

namespace Blaze.Editor.Cms
{
    public static class GenerateIdsC
    {
        [MenuItem("Tools/Blaze/Generate Ids %#o")]
        public static void GeneratedIds()
        {
            StringBuilder debugSb = new();
            var guids = AssetDatabase.FindAssets("t:CmsEntityPfb");
            debugSb.AppendLine($"Generated ids for {guids.Length} entities.");
            List<CmsEntityPfb> changed = new();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<CmsEntityPfb>(path);
                var so = new SerializedObject(asset);
                var idProp = so.FindProperty("id");
                string subpath = path.Substring(path.IndexOf("Resources") + "Resources/".Length);
                subpath = Path.ChangeExtension(subpath, null);
                // subpath = subpath.Substring(0, subpath.IndexOf(Path.GetExtension(subpath)));
                if (asset.id != subpath)
                {
                    idProp.stringValue = subpath;
                    changed.Add(asset);
                }
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(asset);
            }
            debugSb.Append($"Changed {changed.Count} entities: ");
            for (int  i = 0 ; i < changed.Count;i++)
            {
                debugSb.Append(changed[i].id);
                if (i < changed.Count - 1)
                {
                    debugSb.Append(", ");
                }
            }
            AssetDatabase.SaveAssets();
            Debug.Log(debugSb);
        }
    }

    public class CmsReferencesGeneratorWindow : EditorWindow
    {
        public CmsReferencesGenerator model = new();

        [MenuItem("Tools/Blaze/Cms References Generator")]
        public static void Open()
        {
            GetWindow<CmsReferencesGeneratorWindow>("Cms Code Generator");
        }

        public void OnEnable()
        {
            model.Load();
        }

        public void OnDisable()
        {
            model.Save();
        }

        [MenuItem("Tools/Blaze/Generate References Cms %#i")]
        public static void GenerateHotkey()
        {
            var model = new CmsReferencesGenerator();
            model.Load();
            model.Generate();
            model.Save();
        }

        private void OnGUI()
        {
            GUILayout.Label("Cms Code Generator", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            model.namespaceName = EditorGUILayout.TextField(
                "Namespace",
                model.namespaceName);

            EditorGUILayout.BeginHorizontal();

            model.outputPath = EditorGUILayout.TextField(
                "Output Path",
                model.outputPath);

            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string path = EditorUtility.SaveFilePanelInProject(
                    "Select Output File",
                    Path.GetFileName(model.outputPath),
                    "cs",
                    "Select output file");

                if (!string.IsNullOrEmpty(path))
                {
                    model.outputPath = path;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
            {
                model.Generate();
            }
        }
    }

    public class CmsReferencesGenerator
    {
        public string outputPath = "Generated/CmsReferences.cs";
        public string namespaceName = "";
        
        public const string KeyPath = "CmsRefGen_OutputPath";
        public const string KeyNamespace = "CmsRefGen_Namespace";

        public const string Section = "Blaze.Editor.Cms.CmsReferencesGeneratorWindow";

        public void Generate()
        {
            StringBuilder debugSb = new();
            StringBuilder sb = new();
            sb.Append("using Blaze.Runtime.Cms;");
            sb.Append("using System.Collections.Generic;");
            if (!string.IsNullOrEmpty(namespaceName))
            {
                sb.Append($"namespace {namespaceName}{"{"}");
            }
            var guids = AssetDatabase.FindAssets("t:CmsEntityPfb");
            HashSet<string> tmp = new();
            debugSb.AppendLine($"Generated references for {guids.Length} entities.");
            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string subpath = path.Substring(path.IndexOf("Resources") + "Resources/".Length);
                subpath = Path.GetDirectoryName(subpath);
                var strings = subpath.Split(Path.DirectorySeparatorChar);
                foreach (var j in strings)
                {
                    sb.Append($"public partial class {j}{"{"}");
                    var tmp2 = path.Substring(0, path.IndexOf(j) + j.Length);
                    if (!tmp.Contains(tmp2))
                    {
                        var guids2 = AssetDatabase.FindAssets("t:CmsEntityPfb", new[]{tmp2});
                        sb.Append("public static List<CmsEntity> All => new(){");
                        foreach (var k in guids2)
                        {
                            var path2 = AssetDatabase.GUIDToAssetPath(k);
                            CmsEntityPfb cmsEntityPfb2 = AssetDatabase.LoadAssetAtPath<CmsEntityPfb>(path2);
                            sb.Append($"Cms.GetEntity(\"{cmsEntityPfb2.id}\"),");
                        }
                        sb.Append("};");

                        tmp.Add(tmp2);
                    }
                }
                CmsEntityPfb cmsEntityPfb = AssetDatabase.LoadAssetAtPath<CmsEntityPfb>(path); 
                sb.Append($"public static CmsEntity {Path.GetFileNameWithoutExtension(path)} => Cms.GetEntity(\"{cmsEntityPfb.id}\");");
                for (int j = 0; j < strings.Length; j++)
                {
                    sb.Append("}");
                }

                debugSb.Append(cmsEntityPfb.id);
                if (i < guids.Length - 1)
                {
                    debugSb.Append(", ");
                }
            }
            if (!string.IsNullOrEmpty(namespaceName))
            {
                sb.Append($"{"}"}");
            }

            debugSb.AppendLine();
            debugSb.Append($"Generated references for {tmp.Count} directories: ");
            for  (int i = 0; i < tmp.Count; i++)
            {
                debugSb.Append(tmp.ElementAt(i));
                if (i < tmp.Count - 1)
                {
                    debugSb.Append(", ");
                }
            }

            File.WriteAllText(outputPath, sb.ToString());
            AssetDatabase.Refresh();
            Debug.Log(debugSb);
        }

        public void Load()
        {
            var settings = Settings.Get();
            outputPath = settings.referencesGeneratorWindowSettings.outputPath;
            namespaceName = settings.referencesGeneratorWindowSettings.namespaceName;
        }

        public void Save()
        {
            var settings = Settings.Get();
            settings.referencesGeneratorWindowSettings.outputPath = outputPath;
            settings.referencesGeneratorWindowSettings.namespaceName = namespaceName;
            settings.Save();
        }
    }

    [Serializable]
    public class CmsReferencesGeneratorWindowSettings 
    {
        public string namespaceName;
        public string outputPath;        
    }
}

#endif