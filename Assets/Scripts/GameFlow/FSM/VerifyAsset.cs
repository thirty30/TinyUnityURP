using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;
using TFramework.TGUI;

public class VerifyAsset : TFSMStateBase
{
    private UIMainLoading mUI;

    public VerifyAsset(int aState) : base(aState)
    {

    }

    public override void OnEnterState()
    {
        this.mUI = TUIManager.GetSingleton().GetUIObject("UIMainLoading") as UIMainLoading;
        this.mUI.SetProgress(1, 0);
        this.mUI.SetProgress(2, 0);
    }

    public override void OnUpdateState()
    {
        this.mUI.SetProgress(2, 100);
        this.FSM.SetState(this.mState + 1);
    }

    public override void OnExitState()
    {

    }
}
