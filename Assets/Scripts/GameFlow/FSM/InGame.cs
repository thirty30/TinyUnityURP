using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;
using TFramework.TGUI;

public class InGame : TFSMStateBase
{
    public InGame(int aState) : base(aState)
    {

    }

    public override void OnEnterState()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixMain", "Init", null, null);
    }

    public override void OnUpdateState()
    {
        NetworkManager.GetSingleton().Update();
        TUIManager.GetSingleton().Update();

        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixMain", "Update", null, null);
    }

    public override void OnExitState()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixMain", "Clear", null, null);
        HotfixManager.GetSingleton().Clear();
    }
}
