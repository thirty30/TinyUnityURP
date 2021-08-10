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

        //�����ȸ��´��� ILRuntime
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
        //�������е���һ��״̬
        this.FSM.SetState(this.mState + 1);
    }

    //ע��ί��
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

    //ע��������
    private void RegisterAdapter()
    {
        ILRuntime.Runtime.Enviorment.AppDomain rAppDomain = HotfixManager.GetSingleton().ILRuntimeApp;

        rAppDomain.RegisterCrossBindingAdaptor(new ILRuntimeAdapter.TFSMStateBaseAdapter());
    }

    //CLR�ض���
    private void RegisterCLRRedirection()
    {

    }

    //CLR��
    private void RegisterCLRBinding()
    {
        //ILRuntime.Runtime.Generated.CLRBindings.Initialize(HotfixManager.GetSingleton().ILRuntimeApp);
    }
}
