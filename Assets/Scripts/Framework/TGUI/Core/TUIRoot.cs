using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TFramework.TGUI
{
    public class TUIRoot : MBSingleton<TUIRoot>
    {
        public Camera UICamera;
        public EventSystem EventSys;
        public GameObject GlobalUIRoot;
        public GameObject NormalUIRoot;
        public GameObject PopUIRoot;
    }
}

