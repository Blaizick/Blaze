#if UNITY_EDITOR

using Blaze.Runtime.Cms;
using UnityEditor;

namespace Blaze.Editor.Cms
{
    [CustomEditor(typeof(CmsEntityPfb))]
    public class CmsEntityPfbEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            CmsEntityPfb entity = (CmsEntityPfb)target;

            serializedObject.Update();

            // ID
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("id")
            );

            // Components
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("components"),
                true
            );

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif