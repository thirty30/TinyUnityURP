using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TFramework.TGUI
{
    [AddComponentMenu("TUI/TGUIText", 1)]
    public class TGUIText : TMPro.TextMeshProUGUI
    {
        [SerializeField]
        public int LanguageID = -1;

        protected override void Awake()
        {
            base.Awake();
            if (this.LanguageID == -1) { return; }
            if (Application.isPlaying == false) { return; }
            //this.text = RofManager.GetSingleton().GetMultiLanguage(this.LanguageID);
        }
    }
}

