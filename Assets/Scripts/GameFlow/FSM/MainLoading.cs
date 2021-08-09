using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;

public class MainLoading : TFSMStateBase
{
    public MainLoading(int aState) : base(aState)
    {

    }

    public override void OnEnterState() 
    {
        AssetLoader.Init(EAssetLoadType.ASSET_BUNDLE);
        AssetLoader.LoadAB("loading_config");
    }
    
    public override void OnUpdateState() 
    {
        Main.GetSingleton().SetGameFlowState(this.State + 1);
    }
    
    public override void OnExitState() 
    {

    }
}
