using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntimeAdapter
{   
    public class TFSMStateBaseAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mOnEnterState_0 = new CrossBindingMethodInfo("OnEnterState");
        static CrossBindingMethodInfo mOnUpdateState_1 = new CrossBindingMethodInfo("OnUpdateState");
        static CrossBindingMethodInfo mOnExitState_2 = new CrossBindingMethodInfo("OnExitState");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(TFramework.TFSMStateBase);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : TFramework.TFSMStateBase, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void OnEnterState()
            {
                if (mOnEnterState_0.CheckShouldInvokeBase(this.instance))
                    base.OnEnterState();
                else
                    mOnEnterState_0.Invoke(this.instance);
            }

            public override void OnUpdateState()
            {
                if (mOnUpdateState_1.CheckShouldInvokeBase(this.instance))
                    base.OnUpdateState();
                else
                    mOnUpdateState_1.Invoke(this.instance);
            }

            public override void OnExitState()
            {
                if (mOnExitState_2.CheckShouldInvokeBase(this.instance))
                    base.OnExitState();
                else
                    mOnExitState_2.Invoke(this.instance);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}

