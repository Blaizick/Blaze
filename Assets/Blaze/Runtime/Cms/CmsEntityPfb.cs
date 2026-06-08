using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blaze.Runtime.Cms
{
    [CreateAssetMenu(fileName = "NewCmsEntity", menuName = "Cms/CmsEntity", order = 1)]
    public class CmsEntityPfb : ScriptableObject
    {
        public string id;
        [SubclassSelector, SerializeReference]
        public List<CmsComponent> components = new();
    
        public CmsEntity AsCmsEntity()
        {
            return new CmsEntity()
            {
                id = id,
                components = new(components),
            };
        }
    }

    public class CmsEntity
    {
        public string id;
        public List<CmsComponent> components = new();
    }

    [Serializable]
    public class CmsComponent
    {
        
    }

    public static class Cms
    {
        private static Dictionary<string, CmsEntity> s_Entities;
        
        public static void LoadAll(string root)
        {
            s_Entities.Clear();
            Resources.
                LoadAll<CmsEntityPfb>(root).
                Select(i => i.AsCmsEntity()).
                ToList().
                ForEach(i => s_Entities[i.id] = i);
        }
    }
}