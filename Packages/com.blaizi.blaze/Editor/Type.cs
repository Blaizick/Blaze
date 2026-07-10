// using System;
// using UnityEngine;
// using System.Collections.Generic;
// using System.Linq;
// #if UNITY_EDITOR
// using UnityEditor;
// #endif

// namespace Blaze.Editor
// {
//     [AttributeUsage(AttributeTargets.Field)]
//     public class TypeSelectorAttribute : PropertyAttribute
//     {
//         public Type BaseType { get; }

//         public TypeSelectorAttribute()
//         {
//         }

//         public TypeSelectorAttribute(Type baseType)
//         {
//             BaseType = baseType;
//         }
//     }


// #if UNITY_EDITOR
//     [CustomPropertyDrawer(typeof(TypeSelectorAttribute))]
//     public class TypeSelectorDrawer : PropertyDrawer
//     {
//         private string[] _names;
//         private Type[] _types;

//         private void Build(TypeSelectorAttribute attr)
//         {
//             if (_types != null)
//                 return;

//             IEnumerable<Type> types = AppDomain.CurrentDomain
//                 .GetAssemblies()
//                 .SelectMany(a =>
//                 {
//                     try
//                     {
//                         return a.GetTypes();
//                     }
//                     catch
//                     {
//                         return Array.Empty<Type>();
//                     }
//                 });

//             if (attr.BaseType != null)
//                 types = types.Where(t => attr.BaseType.IsAssignableFrom(t));

//             types = types
//                 .Where(t => !t.IsAbstract && !t.IsGenericType);

//             _types = types.OrderBy(t => t.Name).ToArray();
//             _names = _types.Select(t => t.FullName).ToArray();
//         }

//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             if (property.propertyType != SerializedPropertyType.String)
//             {
//                 EditorGUI.LabelField(position, label.text, "Use TypeSelector with string.");
//                 return;
//             }

//             var attr = (TypeSelectorAttribute)attribute;
//             Build(attr);

//             int index = -1;

//             if (!string.IsNullOrEmpty(property.stringValue))
//             {
//                 index = Array.FindIndex(_types,
//                     t => t.AssemblyQualifiedName == property.stringValue);
//             }

//             int newIndex = EditorGUI.Popup(
//                 position,
//                 label.text,
//                 Mathf.Max(index, 0),
//                 _names);

//             if (newIndex >= 0 && newIndex < _types.Length)
//             {
//                 property.stringValue = _types[newIndex].AssemblyQualifiedName;
//             }
//         }
//     }
// #endif
// }