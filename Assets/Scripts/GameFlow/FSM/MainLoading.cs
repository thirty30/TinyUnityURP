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
        AssetLoader.LoadAB("main_loading");
        
        TUIManager.GetSingleton().RegisterUI("UIMainLoading", "Assets/MainLoading/UIMainLoading.prefab");
        TUIManager.GetSingleton().OpenUI("UIMainLoading");
    }
    
    public override void OnUpdateState() 
    {
        this.FSM.SetState(this.mState + 1);
    }
    
    public override void OnExitState() 
    {
        
    }
}
