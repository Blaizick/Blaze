#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Blaze.Runtime.Cms;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Blaze.Editor.Cms
{
    [CustomPropertyDrawer(typeof(WithDefinerAttribute))]
    public class CmsEntityPfbReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            WithDefinerAttribute attribute =
                (WithDefinerAttribute)this.attribute;


            EditorGUI.BeginProperty(position, label, property);

            string current =
                property.objectReferenceValue == null
                    ? "None"
                    : property.objectReferenceValue.name;


            Rect fieldRect = EditorGUI.PrefixLabel(position, label);

            if (EditorGUI.DropdownButton(
                    fieldRect,
                    new GUIContent($"{current}"),
                    FocusType.Keyboard))
            {
                CmsEntityPfbDropdown dropdown =
                    new CmsEntityPfbDropdown(
                        property,
                        attribute.DefinerType
                    );

                dropdown.Show(fieldRect);
            }

            EditorGUI.EndProperty();
        }
    }

    public class CmsEntityPfbDropdown : AdvancedDropdown
    {
        private SerializedProperty property;
        private Type requiredDefiner;

        private List<CmsEntityPfb> entities;


        public CmsEntityPfbDropdown(
            SerializedProperty property,
            Type requiredDefiner)
            : base(new AdvancedDropdownState())
        {
            this.property = property;
            this.requiredDefiner = requiredDefiner;

            minimumSize = new Vector2(350, 400);

            entities = AssetDatabase
                .FindAssets("t:CmsEntityPfb")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(
                    AssetDatabase.LoadAssetAtPath<CmsEntityPfb>
                )
                .Where(x => x != null)
                .Where(x =>
                    x.definer != null &&
                    requiredDefiner.IsAssignableFrom(x.definer.GetType()) 
                )
                .OrderBy(x => x.name)
                .ToList();
        }


        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root =
                new AdvancedDropdownItem("Entities");

            foreach (CmsEntityPfb entity in entities)
            {
                
                root.AddChild(
                    new CmsEntityDropdownItem(
                        entity
                    )
                );
            }

            return root;
        }

        protected override void ItemSelected(
            AdvancedDropdownItem item)
        {
            base.ItemSelected(item);

            CmsEntityDropdownItem entityItem =
                item as CmsEntityDropdownItem;

            if (entityItem == null)
                return;

            property.objectReferenceValue =
                entityItem.Entity;

            property.serializedObject
                .ApplyModifiedProperties();
        }
    }

    public class CmsEntityDropdownItem : AdvancedDropdownItem
    {
        public CmsEntityPfb Entity { get; }

        public CmsEntityDropdownItem(
            CmsEntityPfb entity)
            : base(entity.name)
        {
            Entity = entity;
        }
    }
}

#endif
