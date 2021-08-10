using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;
using TFramework.TGUI;

public class Login : TFSMStateBase
{
    public bool IsEnterNextState = false;

    public Login(int aState) : base(aState)
    {

    }

    public override void OnEnterState()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixLogin", "Init", null, null);
    }

    public override void OnUpdateState()
    {
        NetworkManager.GetSingleton().Update();
        TUIManager.GetSingleton().Update();

        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixLogin", "Update", null, null);

        if (this.IsEnterNextState == true)
        {
            this.FSM.SetState(this.mState + 1);
        }
    }

    public override void OnExitState()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixLogin", "Clear", null, null);
    }
}
