using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
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
            return new CmsEntity(id, new(components));
        }

        public CmsEntity GetCmsEntity()
        {
            return Cms.GetEntity(id);
        }
    }

    public class CmsEntity
    {
        private string m_Id;
        private List<CmsComponent> m_Components = new();
    
        public string Id => m_Id;
        public IReadOnlyList<CmsComponent> Components => m_Components.AsReadOnly();

        public CmsEntity(string id, List<CmsComponent> components)
        {
            m_Id = id;
            m_Components = components;
        }

        public CmsComponent GetComponent(Type type)
        {
            int id = m_Components.FindIndex(i => type.IsAssignableFrom(i.GetType()));
            if (id > -1)
            {
                return m_Components[id];
            }
            else
            {
                return null;
            }
        }
        public bool HasComponent(Type type)
        {
            return m_Components.FindIndex(i => type.IsAssignableFrom(i.GetType())) > -1;
        }
        public List<CmsComponent> GetAllComponentsOfType(Type type)
        {
            return m_Components.Where(i => type.IsAssignableFrom(i.GetType())).ToList();
        }

        public T GetComponent<T>() where T : CmsComponent
        {
            return GetComponent(typeof(T)) as T;
        }
        public bool HasComponent<T>() where T : CmsComponent
        {
            return HasComponent(typeof(T));
        }
        public List<T> GetAllComponentsOfType<T>() where T : CmsComponent
        {
            return GetAllComponentsOfType(typeof(T)).Cast<T>().ToList();
        }
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
            s_Entities = new();
            var list = Resources.
                LoadAll<CmsEntityPfb>(root).
                Select(i => i.AsCmsEntity()).
                ToList();
            StringBuilder sb = new();
            sb.Append($"Loaded {list.Count} entities: ");
            for (int i = 0; i < list.Count; i++)
            {
                var ent = list[i];
                s_Entities[ent.Id] = ent;
                sb.Append(ent.Id);
                if (i < list.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            Debug.Log(sb);
        }

        public static CmsEntity GetEntity(string id)
        {
            return s_Entities.TryGetValue(id, out var i) ? i : null;
        }
    }
}