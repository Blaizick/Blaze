#if UNITY_EDITOR

using System;
using System.Linq;
using Blaze.Runtime.Cms;
using UnityEditor;
using UnityEngine;

namespace Blaze.Editor.Cms
{
    [CustomEditor(typeof(CmsEntityPfb))]
    public class CmsEntityPfbEditor : UnityEditor.Editor
    {
        private Type[] definerTypes;
        private string[] definerNames;

        private void OnEnable()
        {
            definerTypes = TypeCache.GetTypesDerivedFrom<CmsEntityDefiner>()
                .Where(t => !t.IsAbstract && !t.IsGenericType)
                .OrderBy(t => t.Name)
                .ToArray();

            definerNames = definerTypes
                .Select(t => t.Name)
                .Prepend("<None>")
                .ToArray();
        }

        public override void OnInspectorGUI()
        {
            CmsEntityPfb entity = (CmsEntityPfb)target;

            serializedObject.Update();

            // ID
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("id")
            );

            // Definer selector
            DrawDefinerSelector(entity);

            // Defined components
            DrawDefinedComponents(entity);

            // Components
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("components"),
                true
            );

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawDefinerSelector(CmsEntityPfb entity)
        {
            int currentIndex = 0;

            if (entity.definer != null)
            {
                int index = Array.FindIndex(
                    definerTypes,
                    t => t == entity.definer.GetType()
                );

                if (index >= 0)
                    currentIndex = index + 1;
            }

            EditorGUI.BeginChangeCheck();

            int newIndex = EditorGUILayout.Popup(
                "Definer",
                currentIndex,
                definerNames
            );

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(entity, "Change CMS Definer");

                if (newIndex == 0)
                {
                    entity.SetDefiner(null);
                }
                else
                {
                    CmsEntityDefiner definer =
                        (CmsEntityDefiner)Activator.CreateInstance(
                            definerTypes[newIndex - 1]
                        );

                    entity.SetDefiner(definer);
                }

                EditorUtility.SetDirty(entity);
            }
        }


        private void DrawDefinedComponents(CmsEntityPfb entity)
        {
            SerializedProperty definedComponentsProp =
                serializedObject.FindProperty("definedComponents");

            EditorGUILayout.LabelField(
                "Defined Components",
                EditorStyles.boldLabel
            );

            if (definedComponentsProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox(
                    "No components defined by the definer.",
                    MessageType.Info
                );
                return;
            }

            EditorGUI.indentLevel++;

            for (int i = 0; i < definedComponentsProp.arraySize; i++)
            {
                SerializedProperty element =
                    definedComponentsProp.GetArrayElementAtIndex(i);

                string typeName = "Unknown";

                if (element.managedReferenceValue != null)
                {
                    typeName = element.managedReferenceValue.GetType().Name;
                }

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.PropertyField(
                    element,
                    new GUIContent(typeName),
                    true
                );

                EditorGUILayout.EndVertical();
            }

            EditorGUI.indentLevel--;
        }
    }
}
#endif