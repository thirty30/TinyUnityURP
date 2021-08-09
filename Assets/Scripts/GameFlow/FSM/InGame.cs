using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;

public class InGame : TFSMStateBase
{
    public InGame(int aState) : base(aState)
    {

    }

    public override void OnEnterState()
    {

    }

    public override void OnUpdateState()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixMain", "Update", null, null);
    }

    public override void OnExitState()
    {
        HotfixManager.GetSingleton().Clear();
    }
}
