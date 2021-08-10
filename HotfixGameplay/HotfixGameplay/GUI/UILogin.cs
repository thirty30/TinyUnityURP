using System;
using System.Collections.Generic;
using TFramework.TGUI;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixGameplay.GUI
{
    public class UILogin
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
            this.BtnTest.onClick.AddListener(this.OnBtnConnect);
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

        private void OnBtnConnect()
        {
            this.TextTest.text = "Connecting server...";
            bool bConnected = NetworkManager.GetSingleton().ConnectServer("127.0.0.1", 18610);
            if (bConnected == true)
            {
                this.SendLoginMessage();
                TUIManager.GetSingleton().CloseUI("UILogin");
            }
        }

        private void SendLoginMessage()
        {
            Test t = new Test();
            t.Value0 = true;
            t.Value1 = 108;
            t.Value2 = 322;
            t.Value3 = 23212321;
            t.Value4 = 5158497546841;
            t.Value5 = 255;
            t.Value6 = 65535;
            t.Value7 = 4135745215;
            t.Value8 = 5465464685435465;
            t.Value9 = 3.259689f;
            t.Value10 = 5465465.5469546f;
            t.Value11 = "niubi niubi niu  bii";
            t.Value12 = new T1();
            t.Value12.Value1 = 123123;
            t.Value12.Value2 = 3.22f;
            t.Value12.Value3 = "牛逼下班回家gogogogocccccc!!#4#$%^&*%&%&!!!!!!!!!!!!";
            t.Value13 = new List<T2>();
            T2 a = new T2();
            a.Value1 = 1;
            a.Value2 = 1;
            a.Value3 = "abc";
            a.Value4 = 11;
            T2 b = new T2();
            b.Value1 = 2;
            b.Value2 = 2;
            b.Value3 = "def";
            b.Value4 = 22;
            t.Value13.Add(a);
            t.Value13.Add(b);
            t.Value14 = new List<int>();
            t.Value14.Add(1);
            t.Value14.Add(2);
            t.Value14.Add(3);
            t.Value14.Add(3287923);
            t.Value15 = new List<string>();
            t.Value15.Add("123123");
            t.Value15.Add("sfesf");
            t.Value15.Add("ssfi89e890");
            HotfixNetWork.SendMessageToServer(MessageID.C2S_LOGIN, t);
        }
    }
}
