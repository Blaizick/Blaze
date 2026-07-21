using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;

namespace Blaze.Runtime.Cms
{
    [CreateAssetMenu(fileName = "NewCmsEntity", menuName = "Cms/CmsEntity", order = 1)]
    public class CmsEntityPfb : ScriptableObject
    {
        public string id;
        [SerializeReference, SubclassSelector]
        public ICmsComponent[] components = Array.Empty<ICmsComponent>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CmsEntity AsCmsEntity()
        {
            return Cms.GetEntity(id);
        }
    }

    public interface ICmsComponent
    {
        
    }

    public struct CmsEntityData
    {
        public bool created;
        public string stringId;
        public ICmsComponent[] components;
        public Dictionary<Type, ICmsComponentCachePool> cacheDic;

        public void Setup(string id, ICmsComponent[] components)
        {
            this.created = true;
            this.stringId = id;
            this.components = components;
            this.cacheDic = new();
        }
    }

    public interface ICmsComponentCachePool
    {
        
    }

    public class CmsComponentCachePool<T> : ICmsComponentCachePool where T : ICmsComponent
    {
        public ushort[] index = Array.Empty<ushort>();
        public T[] components = Array.Empty<T>();
    }

    public struct CmsEntity
    {
        public ushort id;

        public CmsEntity(ushort id)
        {
            this.id = id;
        }

        public void EnsureCacheForType<T>() where T : ICmsComponent
        {
            var type = typeof(T);
            ref var data = ref Cms.Instance.entities.buffer[id];
            if (data.cacheDic.ContainsKey(type))
            {
                return;
            }
            List<ushort> componentIndexCacheList = new();
            List<T> componentCacheList = new();
            for (ushort i = 0; i < data.components.Length; i++)
            {
                ref var componentData = ref data.components[i];
                if (type.IsAssignableFrom(componentData.GetType()))
                {
                    componentIndexCacheList.Add(i);
                    componentCacheList.Add((T)componentData);
                }
            }
            data.cacheDic[type] = new CmsComponentCachePool<T>()
            {
                components = componentCacheList.ToArray(),
                index = componentIndexCacheList.ToArray()
            };
        }

        public CmsComponentCachePool<T> GetCachePoolForType<T>() where T : ICmsComponent
        {
            EnsureCacheForType<T>();
            return (CmsComponentCachePool<T>)Cms.Instance.entities.buffer[id].cacheDic[typeof(T)];       
        } 

        public ref T GetComponent<T>() where T : ICmsComponent
        {
            return ref GetCachePoolForType<T>().components[0];
        }
        public bool HasComponent<T>() where T : ICmsComponent
        {
            return GetCachePoolForType<T>().components.Length > 0;
        }
        public T[] GetComponentsOfType<T>() where T : ICmsComponent
        {
            return GetCachePoolForType<T>().components;
        }

        public bool Alive
        {
            get
            {
                return Cms.Instance.entities.buffer[id].created;
            }
        }
    }

    public class Cms : SingletonLite<Cms>
    {
        public Array16<CmsEntityData> entities;
        public Dictionary<string, ushort> entitiesDic;

        public Cms()
        {
            _Reset();
        }

        public void _Reset()
        {
            entities = new(ushort.MaxValue);
            entities.CreateItem(out _);

            entitiesDic = new();
        }

        public static void Reset()
        {
            Instance._Reset();
        }

        public static void LoadAll(string root)
        {
            var list = Resources.
                LoadAll<CmsEntityPfb>(root).
                ToList();
            StringBuilder sb = new();
            sb.Append($"Loaded {list.Count} entities: ");
            for (int i = 0; i < list.Count; i++)
            {
                var ent = list[i];
                
                if (Instance.entitiesDic.ContainsKey(ent.id))
                {
                    QDebugBase<InternalLogChannel>.Warn(InternalLogChannel.System, $"Found entities with the same string ids: {ent.id}");
                    continue;
                }

                Instance.entities.CreateItem(out var valId);
                Instance.entities.buffer[valId].Setup(ent.id, ent.components);
                Instance.entitiesDic[ent.id] = valId;

                sb.Append(ent.id);
                if (i < list.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            QDebugBase<InternalLogChannel>.VerboseLog(InternalLogChannel.System, sb.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CmsEntity GetEntity(string stringId)
        {
            return new(Instance.entitiesDic[stringId]);
        }

        public static bool HasEntity(string stringId)
        {
            return Instance.entitiesDic.ContainsKey(stringId);            
        }

        public static bool TryGetEntity(out CmsEntity cmsEntity, string stringId)
        {
            cmsEntity = new();
            if (Instance.entitiesDic.TryGetValue(stringId, out cmsEntity.id))
            {
                return true;
            }
            return false;
        }
    }
}