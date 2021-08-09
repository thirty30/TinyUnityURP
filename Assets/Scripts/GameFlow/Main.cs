using System.Collections;
using UnityEngine;
using TFramework;

public class Main : MBSingleton<Main>
{
    private TFSM mGameFlowFSM;

    private void Start()
    {
        this.mGameFlowFSM = new TFSM();
        this.mGameFlowFSM.RegisterState(new MainLoading(1));
        this.mGameFlowFSM.RegisterState(new AssetVerify(2));
        this.mGameFlowFSM.RegisterState(new LoadHotfixGameplay(3));
        this.mGameFlowFSM.RegisterState(new InGame(4));
        this.mGameFlowFSM.RegisterState(new EndGame(100));
        this.mGameFlowFSM.SetState(1);
    }

    private void Update()
    {
        this.mGameFlowFSM.Update();
    }

    private void OnDestroy()
    {
        this.SetGameFlowState(100);
    }

    public void SetGameFlowState(int aState)
    {
        this.mGameFlowFSM.SetState(aState);
    }

    public void DelegateCoroutine(IEnumerator aFunc)
    {
        StartCoroutine(aFunc);
    }
}
