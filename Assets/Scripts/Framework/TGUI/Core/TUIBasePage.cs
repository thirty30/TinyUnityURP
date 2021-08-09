using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework.TGUI
{
    public enum EUILayer
    {
        GLOBLE,
        NORMAL,
        POPUP
    }

    public class TUIBasePage : MonoBehaviour
    {
        public EUILayer UILayer;
        public int UIOrder;
        [HideInInspector] public string UIName;

        public virtual void OnInitialize(params object[] parms) { }
        public virtual void OnUpdate() { }
        public virtual void OnShow() { }
        public virtual void OnHide() { }
        public virtual void OnDestroy() { }

        public void Close()
        {
            TUIManager.GetSingleton().CloseUI(this);
        }
    }
}

