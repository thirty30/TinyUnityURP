using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;
using TFramework.TGUI;
using ILRuntime.CLR.Method;

public class InGame : TFSMStateBase
{
    private IMethod mMethodInit;
    private IMethod mMethodUpdate;
    private IMethod mMethodClear;

    public InGame(int aState) : base(aState)
    {

    }

    public override void OnEnterState()
    {
        ILRuntime.Runtime.Enviorment.AppDomain rAppDomain = HotfixManager.GetSingleton().ILRuntimeApp;
        this.mMethodInit = rAppDomain.LoadedTypes["HotfixGameplay.HotfixMain"].GetMethod("Init", 0);
        this.mMethodUpdate = rAppDomain.LoadedTypes["HotfixGameplay.HotfixMain"].GetMethod("Update", 0);
        this.mMethodClear = rAppDomain.LoadedTypes["HotfixGameplay.HotfixMain"].GetMethod("Clear", 0);

        HotfixManager.GetSingleton().ILRuntimeApp.Invoke(this.mMethodInit, null, null);
    }

    public override void OnUpdateState()
    {
        NetworkManager.GetSingleton().Update();

        HotfixManager.GetSingleton().ILRuntimeApp.Invoke(this.mMethodUpdate, null, null);
    }

    public override void OnExitState()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke(this.mMethodClear, null, null);
        HotfixManager.GetSingleton().Clear();
    }
}
