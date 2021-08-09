using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;
using System;
using System.Net.Sockets;

public class LoadHotfixGameplay : TFSMStateBase
{
    public LoadHotfixGameplay(int aState) : base(aState)
    {

    }

    public override void OnEnterState() 
    {
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

        //�ض���CLR������CLR��ǰ��


        //��ʼ��CLR�󶨱�����ڳ�ʼ�������һ��
        this.RegisterDelegate();

        yield return new WaitForSeconds(3.0f);
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixMain", "Init", null, null);

        Main.GetSingleton().SetGameFlowState(this.State + 1);
    }

    private void RegisterDelegate()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() => { ((Action)act)(); });
        });

        HotfixManager.GetSingleton().ILRuntimeApp.DelegateManager.RegisterDelegateConvertor<TFramework.TNet.OnConnect>((act) =>
        {
            return new TFramework.TNet.OnConnect(() => { ((Action)act)(); });
        });

        HotfixManager.GetSingleton().ILRuntimeApp.DelegateManager.RegisterDelegateConvertor<TFramework.TNet.OnDisconnect>((act) =>
        {
            return new TFramework.TNet.OnDisconnect(() => { ((Action)act)(); });
        });

        HotfixManager.GetSingleton().ILRuntimeApp.DelegateManager.RegisterDelegateConvertor<TFramework.TNet.OnReceive>((act) =>
        {
            return new TFramework.TNet.OnReceive((aBuffer) => { ((Action<byte[]>)act)(aBuffer); });
        });

        HotfixManager.GetSingleton().ILRuntimeApp.DelegateManager.RegisterDelegateConvertor<TFramework.TNet.OnException>((act) =>
        {
            return new TFramework.TNet.OnException((aException) => { ((Action<SocketException>)act)(aException); });
        });

        HotfixManager.GetSingleton().ILRuntimeApp.DelegateManager.RegisterDelegateConvertor<TFramework.TNet.GetPacketSize>((act) =>
        {
            return new TFramework.TNet.GetPacketSize((aBuffer, aSize, aIndex) =>
            {
                return ((Func<byte[], int, int, int>)act)(aBuffer, aSize, aIndex);
            });
        });

        HotfixManager.GetSingleton().ILRuntimeApp.DelegateManager.RegisterMethodDelegate<byte[]>();
        HotfixManager.GetSingleton().ILRuntimeApp.DelegateManager.RegisterMethodDelegate<SocketException>();
        HotfixManager.GetSingleton().ILRuntimeApp.DelegateManager.RegisterFunctionDelegate<byte[], int, int, int>();
    }
}
