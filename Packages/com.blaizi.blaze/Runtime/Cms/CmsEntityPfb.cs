using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Blaze.Runtime.Cms
{
    [CreateAssetMenu(fileName = "NewCmsEntity", menuName = "Cms/CmsEntity", order = 1)]
    public class CmsEntityPfb : ScriptableObject
    {
        public string id;
        [SerializeReference]
        public CmsEntityDefiner definer = null;
        [SerializeReference, SubclassSelector]
        public List<CmsComponent> components = new();
        [SerializeReference]
        public List<CmsComponent> definedComponents = new();

        public string DefinerTypeFullName => definer == null ? string.Empty : definer.GetType().FullName;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CmsEntity AsCmsEntity()
        {
            List<CmsComponent> allComponents = new();
            allComponents.AddRange(components);
            allComponents.AddRange(definedComponents);
            return new CmsEntity(id, new(allComponents));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CmsEntity GetCmsEntity()
        {
            return Cms.GetEntity(id);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCmsComponentDefined(CmsComponent component)
        {
            return definedComponents.Contains(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDefiner(CmsEntityDefiner definer)
        {
            definedComponents.Clear();
            this.definer = definer;
            if (definer != null)
            {
                definer.cmsEntity = this;
                definer.OnDefine();
            }

            #if UNITY_EDITOR
                EditorUtility.SetDirty(this);
            #endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DefineComponent(Type componentType)
        {
            var comp = (CmsComponent)Activator.CreateInstance(componentType);
            definedComponents.Add(comp);
        }
    }

    [Serializable]
    public class CmsEntityDefiner
    {
        [NonSerialized]
        public CmsEntityPfb cmsEntity;

        public virtual void OnDefine()
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Define(Type type)
        {
            cmsEntity.DefineComponent(type);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Define<T>() where T : CmsComponent
        {
            Define(typeof(T));
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class WithDefinerAttribute : PropertyAttribute
    {
        public Type DefinerType { get; }

        public WithDefinerAttribute(Type type)
        {
            DefinerType = type;
        }
    }

    public class CmsEntityBuilder
    {
        public string id;
        public List<CmsComponent> components = new();

        public CmsEntity CmsEntity
        {
            get
            {
                return new CmsEntity(id, new(components));
            }
        }

        public CmsEntityBuilder WithId(string id)
        {
            this.id = id;
            return this; 
        }

        public CmsEntityBuilder WithComponent(CmsComponent component)
        {
            components.Add(component);
            return this;
        }

        public CmsEntityBuilder WithComponents(List<CmsComponent> components)
        {
            this.components = new(components);
            return this;
        }

        public CmsEntityBuilder Register()
        {
            Cms.Push(CmsEntity);
            return this;
        }
    }

    public class CmsEntity
    {
        private string m_Id;
        private List<CmsComponent> m_Components = new();
        private Dictionary<Type, List<CmsComponent>> m_ComponentsCache = new();

        public string Id => m_Id;
        public IReadOnlyList<CmsComponent> Components => m_Components.AsReadOnly();

        public CmsEntity()
        {
            
        }

        public CmsEntity(string id, List<CmsComponent> components)
        {
            m_Id = id;
            m_Components = components;
            InvalidateComponentsCache();
        }

        public void InvalidateComponentsCache()
        {
            m_ComponentsCache.Clear();
            foreach (var i in m_Components)
            {
                var type = i.GetType();
                List<CmsComponent> list;
                if (!m_ComponentsCache.TryGetValue(type, out list))
                {
                    list = new();
                    m_ComponentsCache[type] = list;
                }
                list.Add(i);
            }
        }

        public static CmsEntityBuilder Create()
        {
            return new CmsEntityBuilder();
        }

        private List<CmsComponent> GetListForComponentOfType(Type type)
        {
            List<CmsComponent> list;
            if (!m_ComponentsCache.TryGetValue(type, out list))
            {
                list = m_Components.Where(i => type.IsAssignableFrom(i.GetType())).ToList();
                m_ComponentsCache[type] = list;
            }
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]    
        public CmsComponent GetComponent(Type type)
        {
            var list = GetListForComponentOfType(type);
            return list.Count == 0 ? null : list.First();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]    
        public bool HasComponent(Type type)
        {
            var list = GetListForComponentOfType(type);
            return list.Count > 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]    
        public List<CmsComponent> GetAllComponentsOfType(Type type)
        {
            return new(GetListForComponentOfType(type));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]    
        public T GetComponent<T>() where T : CmsComponent
        {
            return GetComponent(typeof(T)) as T;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]    
        public bool HasComponent<T>() where T : CmsComponent
        {
            return HasComponent(typeof(T));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]    
        public List<T> GetAllComponentsOfType<T>() where T : CmsComponent
        {
            return GetAllComponentsOfType(typeof(T)).Cast<T>().ToList();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]    
        public bool TryGetComponent(Type type, out CmsComponent component)
        {
            var list = GetListForComponentOfType(type);
            if (list.Count > 0)
            {
                component = list.First();
                return true;
            }
            component = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]    
        public bool TryGetComponent<T>(out T component) where T : CmsComponent
        {
            var type = typeof(T);
            var list = GetListForComponentOfType(type);
            if (list.Count > 0)
            {
                component = (T)list.First();
                return true;
            }
            component = null;
            return false;
        }
    }

    [Serializable]
    public class CmsComponent
    {
        
    }

    public static class Cms
    {
        private static Dictionary<string, CmsEntity> s_Entities = new();
        
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CmsEntity GetEntity(string id)
        {
            return s_Entities.TryGetValue(id, out var i) ? i : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear()
        {
            s_Entities.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Push(CmsEntity cmsEntity)
        {
            s_Entities[cmsEntity.Id] = cmsEntity;
        }
    }
}