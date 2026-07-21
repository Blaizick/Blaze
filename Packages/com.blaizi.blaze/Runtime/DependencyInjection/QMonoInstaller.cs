using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Blaze.Runtime.DependencyInjection
{
    public abstract class QMonoInstaller : ManagedBehaviour
    {
        public QDiContainer Container { get; set; } = new();
        
        public abstract void InstallBindings();
    }

    public class QDiContainer
    {
        public bool installedBindings;
        private Dictionary<QBindingInfo, object> m_Dic;

        public QDiContainer()
        {
            m_Dic = new();

            Bind<QDiContainer>().FromInstance(this);
        }
        
        public QBindingBuilder Bind(Type type)
        {
            return new QBindingBuilder(type, this);
        }
        public QBindingBuilder Bind<T>()
        {
            return Bind(typeof(T));
        }

        public void RegisterBinding(QBindingBuilder bindingBuilder)
        {
            if (bindingBuilder.type == null)
            {
                QDebugBase<InternalLogChannel>.Error(InternalLogChannel.System, "Binding builder type is null");
                return;
            }
            if (bindingBuilder.instance == null)
            {
                if (!TryGetAndCallConstructor(out var instance, bindingBuilder.type))
                {
                    QDebugBase<InternalLogChannel>.Error(InternalLogChannel.System, $"Couldnt find constructor for type: {bindingBuilder.type.Name}");
                    return;
                }
                bindingBuilder.instance = instance;
            }
            m_Dic[bindingBuilder.BindingInfo] = bindingBuilder.instance;
        }

        public bool TryGetAndCallConstructor(out object instance, Type type)
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
                    var bindingInfo = new QBindingInfo(parameter.ParameterType);
                    if (m_Dic.TryGetValue(bindingInfo, out var _object))
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

        public void Inject(object instance, Type instanceType)
        {
            InjectMethods(instance, instanceType);
            InjectFields(instance, instanceType);
        }

        public void Inject<T>(T instance)
        {
            Inject(instance, typeof(T));
        }

        public void InjectMethods(object instance, Type instanceType)
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
                        var bindingInfo = new QBindingInfo(parameterType);
                        if (m_Dic.TryGetValue(bindingInfo, out var _object))
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

        public void InjectFields(object instance, Type instanceType)
        {
            var fields = instanceType.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var injectAttribute = field.GetCustomAttribute<QInjectAttribute>();
                if (injectAttribute != null)
                {
                    var fieldType = field.FieldType;
                    var bindingInfo = new QBindingInfo(fieldType);
                    if (m_Dic.TryGetValue(bindingInfo, out var _object))
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
            if (!TryGetAndCallConstructor(out var instance, type))
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

        public T InstantiatePrefabForComponent<T>(GameObject prefab, 
            Vector3 position = default, 
            Quaternion rotation = default, 
            Transform parent = null) where T : Component
        {
            return InstantiatePrefab(prefab, position, rotation, parent).GetComponent<T>();
        } 
    }

    public struct QBindingBuilder
    {
        public Type type;
        public object instance;
        public QDiContainer container;

        public QBindingBuilder(Type type, QDiContainer container)
        {
            this.type = type;
            this.instance = null;
            this.container = container;
        }

        public void AsSingle()
        {
            container.RegisterBinding(this);
        }
        public void FromInstance(object instance)
        {
            this.instance = instance;
            container.RegisterBinding(this);
        }

        public QBindingInfo BindingInfo
        {
            get
            {
                return new QBindingInfo(type);
            }
        }
    }

    public struct QBindingInfo
    {
        public Type type;

        public QBindingInfo(Type type)
        {
            this.type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Method | 
        AttributeTargets.Constructor | 
        AttributeTargets.Field)]
    public class QInjectAttribute : Attribute
    {
        
    }
}