using System.IO;
using Blaze.Editor.Cms;
using UnityEditor;
using UnityEngine;

namespace Blaze.Editor
{   
    public class Settings : ScriptableObject
    {
        public const string AssetPath = "Assets/Settings/Blaze/Editor/Settings.asset";
        
        public CmsReferencesGeneratorWindowSettings referencesGeneratorWindowSettings = new();
    
        public static Settings Get()
        {
            var settings = AssetDatabase.LoadAssetAtPath<Settings>(AssetPath);

            if (settings != null)
            {
                return settings;
            }

            var directory = Path.GetDirectoryName(AssetPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            settings = CreateInstance<Settings>();

            AssetDatabase.CreateAsset(settings, AssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return settings;
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();            
        }
    }
}