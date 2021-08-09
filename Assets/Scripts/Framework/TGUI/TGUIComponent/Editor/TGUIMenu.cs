using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TFramework.TGUI
{
    public static class TGUIMenu
    {
        [MenuItem("GameObject/UI/TGUIText", priority = 1)]
        private static void CreateTGUIText(MenuCommand menuCommand)
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }
            GameObject go = new GameObject();
            go.name = "TGUIText";
            go.transform.SetParent(Selection.activeGameObject.transform, false);
            TGUIText text = go.AddComponent<TGUIText>();
            text.text = "New Text";
        }
    }
}
