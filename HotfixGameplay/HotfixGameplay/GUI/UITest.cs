using System;
using System.Collections.Generic;
using TFramework.TGUI;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixGameplay.GUI
{
    public class UITest
    {
        private Button BtnTest;
        private TGUIText TextTest;

        public void OnBindingComponent(object[] aComponents)
        {
            this.BtnTest = (aComponents[0] as GameObject).GetComponent<Button>();
            this.TextTest = (aComponents[1] as GameObject).GetComponent<TGUIText>();
        }

        public void OnInitialize(params object[] parms)
        {
            this.BtnTest.onClick.AddListener(this.OnBtnTestClick);
        }

        public void OnUpdate()
        {
            
        }

        public void OnShow()
        {

        }

        public void OnHide()
        {

        }

        public void OnDestroy()
        {

        }

        private void OnBtnTestClick()
        {
            this.TextTest.text = "Clicked";
        }
    }
}
