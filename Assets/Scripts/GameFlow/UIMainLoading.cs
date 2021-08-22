using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework.TGUI;
using TFramework;
using UnityEngine.UI;
using LitJson;

public class UIMainLoading : TUIBasePage
{
    public TGUIText TipText;
    public Slider ProgressBar;

    private string mCurLan;
    private JsonData mJD;

    public override void OnInitialize(params object[] parms)
    {
        this.mCurLan = "english";
        Object rLanFile = AssetLoader.LoadAsset("Assets/MainLoading/UIMainLoadingLanguage.json");
        this.mJD = JsonMapper.ToObject(rLanFile.ToString());


        this.TipText.gameObject.SetActive(true);
        this.ProgressBar.gameObject.SetActive(true);
        this.ProgressBar.value = 0;
        this.TipText.text = this.mJD[this.mCurLan][0].ToString();
    }

    public void SetProgress(int aTipIndex, float aValue)
    {
        this.TipText.text = this.mJD[this.mCurLan][aTipIndex].ToString();
        this.ProgressBar.value = aValue;
    }

    public void HideProgress()
    {
        this.TipText.gameObject.SetActive(false);
        this.ProgressBar.gameObject.SetActive(false);
    }
}
