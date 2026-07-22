using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Blaze.Runtime.DependencyInjection
{
    public class QDiContainer
    {
        private bool m_InstalledBindings;
        private Dictionary<QBindingIdentifier, QBindingWrapper> m_BoundObjectsDic;
        private Array16<QBindingData> m_Bindings;
        private List<ushort> m_ToBind;
        private Dictionary<QBindingIdentifier, ushort> m_BindingDataDic;

        public bool InstalledBindings
        {
            get
            {
                return m_InstalledBindings;
            }
            set
            {
                m_InstalledBindings = true;
            }
        }

        public Dictionary<QBindingIdentifier, QBindingWrapper> Dic
        {
            get
            {
                return m_BoundObjectsDic;
            }
        }

        public Array16<QBindingData> Bindings
        {
            get
            {
                return m_Bindings;
            }
        }

        public List<ushort> ToInject
        {
            get
            {
                return m_ToBind;
            }
        }

        public Dictionary<QBindingIdentifier, ushort> LazyBindingsDic
        {
            get
            {
                return m_BindingDataDic;
            }
        }

        public QDiContainer()
        {
            m_Bindings = new(ushort.MaxValue);
            m_Bindings.CreateItem(out _);
            m_BoundObjectsDic = new();
            m_ToBind = new();
            m_BindingDataDic = new();

            Bind<QDiContainer>().FromInstance(this).AsSingle().NonLazy();
        }
        
        public QBindingBuilder Bind(Type type)
        {
            m_Bindings.CreateItem(out var itemId);
            m_ToBind.Add(itemId);
            return new QBindingBuilder(itemId, type, this);
        }
        public QBindingBuilder Bind<T>()
        {
            return Bind(typeof(T));
        }

        public void Flush()
        {
            for (int i = m_ToBind.Count - 1; i >= 0; i--)
            {
                var bindingId = m_ToBind[i];
                ref var bindingData = ref m_Bindings.buffer[bindingId];

                var identifier = new QBindingIdentifier(bindingData.identifierType);

                m_BindingDataDic[identifier] = bindingId;
                m_ToBind.RemoveAt(i);

                QDebugBase<InternalLogChannel>.VerboseLog(InternalLogChannel.System, $"Bound type: {bindingData.identifierType.Name} to container");

                if (bindingData.lazy)
                {
                    continue;
                }

                if (bindingData.scope == Scope.Single)
                {
                    m_BoundObjectsDic[identifier] = new QBindingWrapper(GetObjectForBinding(bindingId, ref bindingData, Array.Empty<object>()));
                }
            }
        }

        public bool TryGetAndCallConstructor(out object instance, Type type, object[] args)
        {
            var constructors = type.GetConstructors();
            for (int i = 0; i < constructors.Length; i++)
            {
                var constructor = constructors[i];
                var parameters = constructor.GetParameters();
                var objects = new object[constructors.Length];
                bool success = true;
                for (int j = 0; j < parameters.Length; j++)
                {
                    var parameter = parameters[j];
                    var bindingInfo = new QBindingIdentifier(parameter.ParameterType);
                    if (TryResolve(out var _object, bindingInfo, args))
                    {
                        objects[j] = _object;
                    }
                    else
                    {
                        success = false;
                        break;                        
                    }
                }
                if (success)
                {
                    if (parameters.Length == 0)
                    {
                        instance = constructor.Invoke(null);
                    }
                    else
                    {
                        instance = constructor.Invoke(objects);
                    }
                    return true;
                }
            }
            instance = null;
            return false;
        }

        public void Inject(object injectable)
        {
            Inject(injectable, Array.Empty<object>());
        }

        public void Inject(object injectable, Type injectableType)
        {
            Inject(injectable, injectableType, Array.Empty<object>());
        }

        public void Inject(object injectable, object[] args)
        {
            Inject(injectable, injectable.GetType(), args);
        }

        public void Inject(object injectable, Type injectableType, object[] args)
        {
            InjectMethods(injectable, injectableType, args);
            InjectFields(injectable, injectableType, args);
        }

        public void InjectMethods(object instance, Type instanceType, object[] args)
        {
            var methods = instanceType.GetMethods();
            for (int i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                var injectAttribute = method.GetCustomAttribute<QInjectAttribute>();
                if (injectAttribute != null)
                {
                    var parameters = method.GetParameters();
                    var objects = new object[parameters.Length];
                    bool success = true;
                    for (int j = 0; j < parameters.Length; j++)
                    {
                        var parameter = parameters[j];
                        var parameterType = parameter.ParameterType;
                        var bindingInfo = new QBindingIdentifier(parameterType);
                        if (TryResolve(out var _object, bindingInfo, args))
                        {
                            objects[j] = _object;
                        }
                        else
                        {
                            success = false;
                            QDebugBase<InternalLogChannel>.Error(InternalLogChannel.Default, $"Couldnt find object for parameter of type {parameterType.Name} for instance of type {instanceType.Name}");
                            break;
                        }
                    }
                    if (success)
                    {
                        method.Invoke(instance, objects);
                        break;
                    }
                }
            }
        }

        public void InjectFields(object instance, Type instanceType, object[] args)
        {
            var fields = instanceType.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var injectAttribute = field.GetCustomAttribute<QInjectAttribute>();
                if (injectAttribute != null)
                {
                    var fieldType = field.FieldType;
                    var bindingInfo = new QBindingIdentifier(fieldType);
                    if (TryResolve(out var _object, bindingInfo, args))
                    {
                        field.SetValue(instance, _object);
                    }
                    else
                    {
                        QDebugBase<InternalLogChannel>.Error(InternalLogChannel.Default, $"Couldnt find object for field of type {fieldType.Name} for instance of type {instanceType.Name}");
                    }
                }
            }
        }

        public T Instantiate<T>()
        {
            var type = typeof(T);
            if (!TryGetAndCallConstructor(out var instance, type, Array.Empty<object>()))
            {
                QDebugBase<InternalLogChannel>.Error(InternalLogChannel.System, $"Couldnt find constructor for type: {type.Name}");
            }
            Inject(instance, type);
            return (T)instance;
        }

        public void InjectGameObject(GameObject gameObject)
        {
            var monoBehaviours = gameObject.GetComponentsInChildren<MonoBehaviour>();
            foreach (var monoBehaviour in monoBehaviours)
            {
                Inject(monoBehaviour, monoBehaviour.GetType());
            }
        }

        public GameObject InstantiatePrefab(GameObject prefab, 
            Vector3 position = default, 
            Quaternion rotation = default, 
            Transform parent = null)
        {
            var instance = GameObject.Instantiate(prefab, position, rotation, parent);
            InjectGameObject(instance);
            return instance;
        }
        public object InstantiatePrefabForComponent(GameObject prefab, Type componentType, 
            Vector3 position = default, 
            Quaternion rotation = default, 
            Transform parent = null)
        {
            return InstantiatePrefab(prefab, position, rotation, parent).GetComponent(componentType);
        }

        public T InstantiatePrefabForComponent<T>(GameObject prefab, 
            Vector3 position = default, 
            Quaternion rotation = default, 
            Transform parent = null) where T : Component
        {
            return InstantiatePrefab(prefab, position, rotation, parent).GetComponent<T>();
        }

        public object ResolveFromBindings(QBindingIdentifier id)
        {
            var bindingDataId = m_BindingDataDic[id];
            ref var bindingData = ref m_Bindings.buffer[bindingDataId];
            object _object = null;
            switch (bindingData.scope)
            {
                case Scope.Single:
                    if (!m_BoundObjectsDic.TryGetValue(id, out var wrapper))
                    {
                        wrapper = new QBindingWrapper(GetObjectForBinding(bindingDataId, ref bindingData, Array.Empty<object>()));
                        m_BoundObjectsDic[id] = wrapper;
                    }
                    _object = wrapper.Object;
                    break;

                case Scope.Default:
                case Scope.Transient:
                    _object = GetObjectForBinding(bindingDataId, ref bindingData, Array.Empty<object>());
                    break;
            }
            return _object;
        }

        public object GetObjectForBinding(ushort bindingId, ref QBindingData bindingData, object[] args)
        {
            object _object = null;
            switch (bindingData.constructionMethod)
            {
                case ConstructionMethod.Default:
                case ConstructionMethod.New:
                    if (!TryGetAndCallConstructor(out _object, bindingData.instanceType, args))
                    {
                        QDebugBase<InternalLogChannel>.Error(InternalLogChannel.System, $"Couldnt find constructor for type: {bindingData.instanceType.Name}");
                    }
                    break;
                case ConstructionMethod.Instance:
                    _object = bindingData.instance;
                    break;
                
                case ConstructionMethod.Method:
                    _object = bindingData.method.Invoke(InjectContext);
                    break;

                case ConstructionMethod.ComponentOnNewPrefab:
                    _object = InstantiatePrefabForComponent(bindingData.prefab, bindingData.instanceType);
                    break;

                case ConstructionMethod.ComponentOnNewPrefabResource:
                    _object = GameObject.Instantiate(Resources.Load<GameObject>(bindingData.prefabResource)).GetComponent(bindingData.instanceType);
                    break;

                case ConstructionMethod.NewComponentOnNewGameObject:
                    _object = new GameObject(bindingData.instanceType.Name).AddComponent(bindingData.instanceType);
                    break;

                case ConstructionMethod.NewComponentOnNewPrefab:
                    _object = GameObject.Instantiate(bindingData.prefab).AddComponent(bindingData.instanceType);
                    break;

                case ConstructionMethod.NewComponentOnNewPrefabResource:
                    _object = GameObject.Instantiate(Resources.Load<GameObject>(bindingData.prefabResource)).AddComponent(bindingData.instanceType);
                    break;

                case ConstructionMethod.NewComponentOnGameObject:
                    _object = bindingData.gameObject.AddComponent(bindingData.instanceType);
                    break;
                
                case ConstructionMethod.ComponentInHierarchy:
                    _object = GameObject.FindAnyObjectByType(bindingData.instanceType);
                    break;

                case ConstructionMethod.ComponentInChildren:
                    _object = bindingData.gameObject.GetComponentInChildren(bindingData.instanceType);
                    break;

                case ConstructionMethod.ComponentInParents:
                    _object = bindingData.gameObject.GetComponentInParent(bindingData.instanceType);
                    break;
            }
            return _object;
        }

        public bool TryResolve(out object _object, QBindingIdentifier id, object[] args)
        {
            _object = null;
            if (m_BindingDataDic.ContainsKey(id))
            {
                _object = ResolveFromBindings(id);
                return true;
            }
            else
            {
                foreach (var i in args)
                {
                    if (id.Type.IsAssignableFrom(i.GetType()))
                    {
                        _object = i;
                        return true;
                    }
                }
            }
            return default;
        }

        public InjectContext InjectContext
        {
            get
            {
                return new InjectContext(this);
            }
        } 
    }

    public struct InjectContext
    {
        private QDiContainer m_Container;

        public QDiContainer Container
        {
            get
            {
                return m_Container;
            }
        }

        public InjectContext(QDiContainer container)
        {
            this.m_Container = container;
        }
    }

    public enum ConstructionMethod
    {
        Default,
        New,
        Instance,
        Method,
        ComponentOnNewPrefab,
        ComponentOnNewPrefabResource,
        NewComponentOnNewGameObject,
        NewComponentOnNewPrefab,
        NewComponentOnNewPrefabResource,
        NewComponentOnGameObject,
        ComponentInHierarchy,
        ComponentInChildren,
        ComponentInParents,
    }

    public enum Scope
    {
        Default,
        Transient,
        Single,
    }

    public struct QBindingData
    {
        public Type identifierType;
        public Type instanceType;
        public object instance;
        public ConstructionMethod constructionMethod;
        public Func<InjectContext, object> method;
        public Scope scope;
        public GameObject prefab;
        public bool lazy;
        public string prefabResource;
        public GameObject gameObject;
    }

    public struct QBindingBuilder
    {
        public ushort bindingDataId;
        public QDiContainer container;

        public QBindingBuilder(ushort bindingDataId, Type type, QDiContainer container)
        {
            this.bindingDataId = bindingDataId;
            this.container = container;
            container.Bindings.buffer[bindingDataId].identifierType = type;
            container.Bindings.buffer[bindingDataId].instanceType = type;
            container.Bindings.buffer[bindingDataId].instance = null;
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.Default;
            container.Bindings.buffer[bindingDataId].method = null;
            container.Bindings.buffer[bindingDataId].scope = Scope.Default;
            container.Bindings.buffer[bindingDataId].prefab = null;
            container.Bindings.buffer[bindingDataId].lazy = true;
            container.Bindings.buffer[bindingDataId].prefabResource = string.Empty;
            container.Bindings.buffer[bindingDataId].gameObject = null;
        }

        public QBindingBuilder AsTransient()
        {
            container.Bindings.buffer[bindingDataId].scope = Scope.Transient;
            return this;
        }
        
        public QBindingBuilder AsSingle()
        {
            container.Bindings.buffer[bindingDataId].scope = Scope.Single;
            return this;
        }

        public QBindingBuilder FromNew()
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.New;
            return this;
        }
        
        public QBindingBuilder FromInstance(object instance)
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.Instance;
            container.Bindings.buffer[bindingDataId].instance = instance;
            return this;
        }
        
        public QBindingBuilder FromMethod(Func<InjectContext, object> method)
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.Method;
            container.Bindings.buffer[bindingDataId].method = method;
            return this;
        }

        public QBindingBuilder FromComponentOnNewPrefab(GameObject prefab)
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.ComponentOnNewPrefab;
            container.Bindings.buffer[bindingDataId].prefab = prefab;
            return this;
        }
        
        public QBindingBuilder FromComponentOnNewPrefabResource(string resource)
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.ComponentOnNewPrefabResource;
            container.Bindings.buffer[bindingDataId].prefabResource = resource;
            return this;
        }

        public QBindingBuilder FromNewComponentOnNewGameObject()
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.NewComponentOnNewGameObject;
            return this;
        }

        public QBindingBuilder FromNewComponentOnNewPrefab(GameObject prefab)
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.NewComponentOnNewPrefab;
            container.Bindings.buffer[bindingDataId].prefab = prefab;
            return this;
        }

        public QBindingBuilder FromNewComponentOnNewPrefabResource(string resource)
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.NewComponentOnNewPrefabResource;
            container.Bindings.buffer[bindingDataId].prefabResource = resource;
            return this;
        }

        public QBindingBuilder FromNewComponentOnGameObject(GameObject gameObject)
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.NewComponentOnGameObject;
            container.Bindings.buffer[bindingDataId].gameObject = gameObject;
            return this;
        }

        public QBindingBuilder FromComponentInHierarchy()
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.ComponentInHierarchy;
            return this;
        }

        public QBindingBuilder FromComponentInChildren(GameObject gameObject)
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.ComponentInChildren;
            container.Bindings.buffer[bindingDataId].gameObject = gameObject;
            return this;
        }

        public QBindingBuilder FromComponentInParents(GameObject gameObject)
        {
            container.Bindings.buffer[bindingDataId].constructionMethod = ConstructionMethod.ComponentInParents;
            container.Bindings.buffer[bindingDataId].gameObject = gameObject;
            return this;
        }

        public QBindingBuilder Lazy()
        {
            container.Bindings.buffer[bindingDataId].lazy = true;
            return this;
        }

        public QBindingBuilder NonLazy()
        {
            container.Bindings.buffer[bindingDataId].lazy = false;
            return this;
        }

        public QBindingBuilder To(Type type)
        {
            container.Bindings.buffer[bindingDataId].instanceType = type;
            return this;
        }

        public QBindingBuilder To<T>()
        {
            return To(typeof(T));
        }

        public QBindingIdentifier BindingInfo
        {
            get
            {
                return new QBindingIdentifier(container.Bindings.buffer[bindingDataId].identifierType);
            }
        }
    }

    public struct QBindingIdentifier : IEquatable<QBindingIdentifier>
    {
        private Type m_Type;
        private object m_Identifier;

        public Type Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }

        public object Identifier
        {
            get
            {
                return m_Identifier;
            }
            set
            {
                m_Identifier = value;
            }
        }

        public QBindingIdentifier(Type type, object identifier)
        {
            this.m_Type = type;
            this.m_Identifier = identifier;
        }

        public QBindingIdentifier(Type type)
        {
            this.m_Type = type;
            this.m_Identifier = null;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 29 + m_Type.GetHashCode();
            if (m_Identifier != null)
            {
                hash = hash * 29 + m_Identifier.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object other)
        {
            if (other is QBindingIdentifier otherIdentifier)
            {
                return m_Identifier == otherIdentifier.Identifier &&
                    m_Type == otherIdentifier.Type;
            }
            return false;
        }

        public bool Equals(QBindingIdentifier other)
        {
            return Equals((object)other);
        }
    }

    public struct QBindingWrapper
    {
        private object m_Object;
        
        public object Object
        {
            get
            {
                return m_Object;
            }
        }

        public QBindingWrapper(object _object)
        {
            this.m_Object = _object;
        }
    }

    [AttributeUsage(AttributeTargets.Method | 
        AttributeTargets.Constructor | 
        AttributeTargets.Field)]
    public class QInjectAttribute : Attribute
    {
        
    }
}