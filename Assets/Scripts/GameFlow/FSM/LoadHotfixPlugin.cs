using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;
using System;
using System.Net.Sockets;

public class LoadHotfixPlugin : TFSMStateBase
{
    public LoadHotfixPlugin(int aState) : base(aState)
    {

    }

    public override void OnEnterState() 
    {
        HotfixManager.GetSingleton().RegisterDelegate += this.RegisterDelegate;
        HotfixManager.GetSingleton().RegisterAdapter += this.RegisterAdapter;
        HotfixManager.GetSingleton().RegisterCLRRedirection += this.RegisterCLRRedirection;
        HotfixManager.GetSingleton().RegisterCLRBinding += this.RegisterCLRBinding;

        //加载热更新代码 ILRuntime
        Main.GetSingleton().DelegateCoroutine(this.InitHotfixGameplay());
    }
    
    public override void OnUpdateState() 
    {

    }
    
    public override void OnExitState() 
    {

    }

    private IEnumerator InitHotfixGameplay()
    {
        yield return HotfixManager.GetSingleton().InitILRuntime("HotfixGameplay");
        //yield return new WaitForSeconds(3.0f);
        //加载完切到下一个状态
        this.FSM.SetState(this.mState + 1);
    }

    //注册委托
    private void RegisterDelegate()
    {
        ILRuntime.Runtime.Enviorment.AppDomain rAppDomain = HotfixManager.GetSingleton().ILRuntimeApp;

        rAppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() => { ((Action)act)(); });
        });

        rAppDomain.DelegateManager.RegisterDelegateConvertor<TFramework.TNet.OnConnect>((act) =>
        {
            return new TFramework.TNet.OnConnect(() => { ((Action)act)(); });
        });

        rAppDomain.DelegateManager.RegisterDelegateConvertor<TFramework.TNet.OnDisconnect>((act) =>
        {
            return new TFramework.TNet.OnDisconnect(() => { ((Action)act)(); });
        });

        rAppDomain.DelegateManager.RegisterDelegateConvertor<TFramework.TNet.OnReceive>((act) =>
        {
            return new TFramework.TNet.OnReceive((aBuffer) => { ((Action<byte[]>)act)(aBuffer); });
        });

        rAppDomain.DelegateManager.RegisterDelegateConvertor<TFramework.TNet.OnException>((act) =>
        {
            return new TFramework.TNet.OnException((aException) => { ((Action<SocketException>)act)(aException); });
        });

        rAppDomain.DelegateManager.RegisterDelegateConvertor<TFramework.TNet.GetPacketSize>((act) =>
        {
            return new TFramework.TNet.GetPacketSize((aBuffer, aSize, aIndex) =>
            {
                return ((Func<byte[], int, int, int>)act)(aBuffer, aSize, aIndex);
            });
        });

        rAppDomain.DelegateManager.RegisterMethodDelegate<byte[]>();
        rAppDomain.DelegateManager.RegisterMethodDelegate<SocketException>();
        rAppDomain.DelegateManager.RegisterFunctionDelegate<byte[], int, int, int>();
    }

    //注册适配器
    private void RegisterAdapter()
    {
        ILRuntime.Runtime.Enviorment.AppDomain rAppDomain = HotfixManager.GetSingleton().ILRuntimeApp;

        rAppDomain.RegisterCrossBindingAdaptor(new ILRuntimeAdapter.TFSMStateBaseAdapter());
    }

    //CLR重定向
    private void RegisterCLRRedirection()
    {

    }

    //CLR绑定
    private void RegisterCLRBinding()
    {
        //ILRuntime.Runtime.Generated.CLRBindings.Initialize(HotfixManager.GetSingleton().ILRuntimeApp);
    }
}
