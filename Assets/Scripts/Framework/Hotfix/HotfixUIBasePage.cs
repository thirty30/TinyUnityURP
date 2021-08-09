using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework.TGUI;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.CLR.TypeSystem;
using System.Reflection;

namespace TFramework
{
    public class HotfixUIBasePage : TUIBasePage
    {
        public string HotfixUIClass;
        public List<GameObject> RefComponents;

        private object mHotfixInstance;
        private System.Type mReflectionType;
        private MethodInfo[] mMethodInfo = new MethodInfo[6];

        public override void OnInitialize(params object[] parms)
        {
            AppDomain appdomain = HotfixManager.GetSingleton().ILRuntimeApp;
            IType hotfixType = appdomain.LoadedTypes[this.HotfixUIClass];
            this.mReflectionType = hotfixType.ReflectionType;
            this.mHotfixInstance = appdomain.Instantiate(this.HotfixUIClass);

            this.mMethodInfo[0] = this.mReflectionType.GetMethod("OnBindingComponent");
            this.mMethodInfo[1] = this.mReflectionType.GetMethod("OnInitialize");
            this.mMethodInfo[2] = this.mReflectionType.GetMethod("OnUpdate");
            this.mMethodInfo[3] = this.mReflectionType.GetMethod("OnShow");
            this.mMethodInfo[4] = this.mReflectionType.GetMethod("OnHide");
            this.mMethodInfo[5] = this.mReflectionType.GetMethod("OnDestroy");

            this.mMethodInfo[0]?.Invoke(this.mHotfixInstance, new object[] { this.RefComponents.ToArray() });
            this.mMethodInfo[1]?.Invoke(this.mHotfixInstance, new object[] { parms });
        }

        public override void OnUpdate()
        {
            this.mMethodInfo[2]?.Invoke(this.mHotfixInstance, null);
        }

        public override void OnShow()
        {
            this.mMethodInfo[3]?.Invoke(this.mHotfixInstance, null);
        }

        public override void OnHide()
        {
            this.mMethodInfo[4]?.Invoke(this.mHotfixInstance, null);
        }

        public override void OnDestroy()
        {
            this.mMethodInfo[5]?.Invoke(this.mHotfixInstance, null);
        }
    }
}
