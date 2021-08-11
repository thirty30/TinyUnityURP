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
    }

    //ע��������
    private void RegisterAdapter()
    {
        ILRuntime.Runtime.Enviorment.AppDomain rAppDomain = HotfixManager.GetSingleton().ILRuntimeApp;

        rAppDomain.RegisterCrossBindingAdaptor(new ILRuntimeAdapter.CoroutineAdapter());
        rAppDomain.RegisterCrossBindingAdaptor(new ILRuntimeAdapter.MonoBehaviourAdapter());
        rAppDomain.RegisterCrossBindingAdaptor(new ILRuntimeAdapter.TFSMStateBaseAdapter());
    }

    //CLR�ض���
    private void RegisterCLRRedirection()
    {

    }

    //CLR��
    private void RegisterCLRBinding()
    {
        ILRuntime.Runtime.Enviorment.AppDomain rAppDomain = HotfixManager.GetSingleton().ILRuntimeApp;

        //��ֵ����
        rAppDomain.RegisterValueTypeBinder(typeof(Vector2), new ILRuntimeTypeBinder.Vector2Binder());
        rAppDomain.RegisterValueTypeBinder(typeof(Vector3), new ILRuntimeTypeBinder.Vector3Binder());
        rAppDomain.RegisterValueTypeBinder(typeof(Quaternion), new ILRuntimeTypeBinder.QuaternionBinder());

        //�󶨽ӿ�
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(rAppDomain);
    }
}