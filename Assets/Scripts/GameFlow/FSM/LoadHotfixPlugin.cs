using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;
using System;
using System.Net.Sockets;
using TFramework.TGUI;
using System.IO;

public class LoadHotfixPlugin : TFSMStateBase
{
    private UIMainLoading mUI;

    public LoadHotfixPlugin(int aState) : base(aState)
    {

    }

    public override void OnEnterState() 
    {
        this.mUI = TUIManager.GetSingleton().GetUIObject("UIMainLoading") as UIMainLoading;
        this.mUI.SetProgress(0, 0);

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
        //�ж��ļ��Ƿ����
        string strLibFile = Path.Combine(Application.persistentDataPath, "AssetBundles/HotfixGameplay.dll");
        if (File.Exists(strLibFile) == false)
        {
            strLibFile = Path.Combine(Application.streamingAssetsPath, "AssetBundles/HotfixGameplay.dll");
        }

        yield return HotfixManager.GetSingleton().InitILRuntime(strLibFile);

        //�������е���һ��״̬
        this.mUI.SetProgress(0, 1.0f);
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
