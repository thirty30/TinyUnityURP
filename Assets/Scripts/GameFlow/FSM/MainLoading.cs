using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;
using TFramework.TGUI;

public class MainLoading : TFSMStateBase
{
    public MainLoading(int aState) : base(aState)
    {

    }

    public override void OnEnterState() 
    {
        AssetLoader.LoadAB("loading_config");

        TUIManager.GetSingleton().Initialize();

        this.FSM.SetState(this.mState + 1);
    }
    
    public override void OnUpdateState() 
    {
        
    }
    
    public override void OnExitState() 
    {

    }
}
