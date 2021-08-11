using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;
using TFramework.TGUI;
using ILRuntime.CLR.Method;

public class Login : TFSMStateBase
{
    public bool IsEnterNextState = false;
    private IMethod mMethodInit;
    private IMethod mMethodUpdate;
    private IMethod mMethodClear;

    public Login(int aState) : base(aState)
    {

    }

    public override void OnEnterState()
    {
        NetworkManager.GetSingleton().Initialize();

        ILRuntime.Runtime.Enviorment.AppDomain rAppDomain = HotfixManager.GetSingleton().ILRuntimeApp;
        this.mMethodInit = rAppDomain.LoadedTypes["HotfixGameplay.HotfixLogin"].GetMethod("Init", 0);
        this.mMethodUpdate = rAppDomain.LoadedTypes["HotfixGameplay.HotfixLogin"].GetMethod("Update", 0);
        this.mMethodClear = rAppDomain.LoadedTypes["HotfixGameplay.HotfixLogin"].GetMethod("Clear", 0);

        HotfixManager.GetSingleton().ILRuntimeApp.Invoke(this.mMethodInit, null, null);
    }

    public override void OnUpdateState()
    {
        NetworkManager.GetSingleton().Update();
        TUIManager.GetSingleton().Update();

        HotfixManager.GetSingleton().ILRuntimeApp.Invoke(this.mMethodUpdate, null, null);

        if (this.IsEnterNextState == true)
        {
            this.FSM.SetState(this.mState + 1);
        }
    }

    public override void OnExitState()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke(this.mMethodClear, null, null);
    }
}
