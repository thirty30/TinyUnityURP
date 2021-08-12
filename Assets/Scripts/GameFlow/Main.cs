using System.Collections;
using UnityEngine;
using TFramework;
using TFramework.TGUI;

public class Main : MBSingleton<Main>
{
    private TFSM mGameFlowFSM;

    private void Start()
    {
        AssetLoader.Init(EAssetLoadType.ASSET_BUNDLE);
        TUIManager.GetSingleton().Initialize();

        this.mGameFlowFSM = new TFSM();
        this.mGameFlowFSM.RegisterState(new MainLoading(1));
        this.mGameFlowFSM.RegisterState(new VerifyAsset(2));
        this.mGameFlowFSM.RegisterState(new LoadHotfixPlugin(3));
        this.mGameFlowFSM.RegisterState(new Login(4));
        this.mGameFlowFSM.RegisterState(new InGame(5));
        this.mGameFlowFSM.RegisterState(new EndGame(100));

        this.mGameFlowFSM.SetState(1);
    }

    private void Update()
    {
        this.mGameFlowFSM.Update();
        TUIManager.GetSingleton().Update();
    }

    private void OnDestroy()
    {
        this.mGameFlowFSM.SetState(100);
    }

    public void EnterGame()
    {
        Login rLoginState = this.mGameFlowFSM.GetStateObj() as Login;
        rLoginState.IsEnterNextState = true;
    }

    public void DelegateCoroutine(IEnumerator aFunc)
    {
        StartCoroutine(aFunc);
    }
}
