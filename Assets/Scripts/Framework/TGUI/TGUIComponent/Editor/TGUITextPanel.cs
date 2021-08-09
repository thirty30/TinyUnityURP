using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor;

namespace TFramework.TGUI
{
    [CustomEditor(typeof(TGUIText), true), CanEditMultipleObjects]
    public class TGUITextPanel : TMP_EditorPanelUI
    {
        private SerializedProperty mLanguageProp;

        protected override void OnEnable()
        {
            this.mLanguageProp = serializedObject.FindProperty("LanguageID");
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(this.mLanguageProp);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
