using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;

public class AssetVerify : TFSMStateBase
{
    public AssetVerify(int aState) : base(aState)
    {

    }

    public override void OnEnterState()
    {

    }

    public override void OnUpdateState()
    {
        this.FSM.SetState(this.mState + 1);
    }

    public override void OnExitState()
    {

    }
}
