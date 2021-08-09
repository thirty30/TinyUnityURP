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
        Main.GetSingleton().SetGameFlowState(this.State + 1);
    }

    public override void OnExitState()
    {

    }
}
