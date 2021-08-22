using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFramework;
using TFramework.TGUI;
using UnityEngine.Networking;
using System.IO;
using LitJson;

public class VerifyAsset : TFSMStateBase
{
    private enum EVerifyState
    {
        WAIT,
        ERROR,

        VERIFY_VERSION,
        VERIFY_FILELIST,
        UPDATE_AB_FILE,
        FINISH,
    }

    private EVerifyState mVerifyState;
    private UIMainLoading mUI;
    private string mCurVersion;
    private string mLastestVersion;
    private string mCDN;
    private UnityWebRequest mWebReq;

    public VerifyAsset(int aState) : base(aState)
    {

    }

    public override void OnEnterState()
    {
#if UNITY_EDITOR_WIN && DEV_ENVIRONMENT
        this.FSM.SetState(this.mState + 1);
        return;
#else

        this.mCurVersion = PersistenceHelper.GetString("GameVersion");
        if (string.IsNullOrEmpty(this.mCurVersion) == true)
        {
            //第一次进游戏，版本号从配置文件读
            string strVersionFile = Path.Combine(Application.streamingAssetsPath, "BasicConfig.json");
            string strJsonVersion = File.ReadAllText(strVersionFile);
            JsonData rBasicCfgJD = JsonMapper.ToObject(strJsonVersion);
            this.mCurVersion = rBasicCfgJD["Version"].ToString();
            this.mCDN = rBasicCfgJD["CDN"].ToString();
        }
        else
        {
            //进游戏后版本号从缓存里读
            string strVersionFile = Path.Combine(Application.persistentDataPath, "BasicConfig.json");
            string strJsonVersion = File.ReadAllText(strVersionFile);
            JsonData rBasicCfgJD = JsonMapper.ToObject(strJsonVersion);
            this.mCDN = rBasicCfgJD["CDN"].ToString();
        }

        this.mUI = TUIManager.GetSingleton().GetUIObject("UIMainLoading") as UIMainLoading;
        Main.GetSingleton().DelegateCoroutine(this.DoVerifyAssetProcess());
#endif
    }


    private IEnumerator DoVerifyAssetProcess()
    {
        this.mVerifyState = EVerifyState.VERIFY_VERSION;
        this.mUI.SetProgress(1, 0);
        string strAssetsURL = string.Empty;

        //下载版本文件，验证版本信息
        {
            //获取版本指引文件
            this.mWebReq = UnityWebRequest.Get(Path.Combine(this.mCDN, "version.json"));
            this.mWebReq.timeout = 10;
            yield return this.mWebReq.SendWebRequest();

            if (string.IsNullOrEmpty(this.mWebReq.error) == false)
            {
                this.mVerifyState = EVerifyState.ERROR;
                yield break;
            }

            MemoryStream ms = new MemoryStream(this.mWebReq.downloadHandler.data);
            StreamReader sr = new StreamReader(ms);
            string strVersionFileJson = sr.ReadToEnd();
            sr.Close();
            JsonData rVersionFileJD = JsonMapper.ToObject(strVersionFileJson);
            this.mLastestVersion = rVersionFileJD["last_version"].ToString();

            //比较版本是否一致
            if (this.mLastestVersion == this.mCurVersion)
            {
                //没有需要更新的新版本
                this.mUI.SetProgress(1, 1.0f);
                this.mVerifyState = EVerifyState.FINISH;
                yield break;
            }

            //如果版本不一致，找到更新目录
            string strPlatformURL = string.Empty;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                strPlatformURL = "windows_url";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                strPlatformURL = "android_url";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                strPlatformURL = "ios_url";
            }
            else
            {
                this.mVerifyState = EVerifyState.ERROR;
                yield break;
            }
            strAssetsURL = rVersionFileJD[strPlatformURL].ToString();
        }

        List<string> rToUpdate = new List<string>();
        PersistenceList<string> rDownloadedFiles = null;
        //找到需要更新的文件
        {
            this.mVerifyState = EVerifyState.VERIFY_VERSION;

            //判断当前版本文件列表是否存在Persistent目录
            string strFileListPath = Path.Combine(Application.persistentDataPath, "FileList.json");
            if (File.Exists(strFileListPath) == false)
            {
                strFileListPath = Path.Combine(Application.streamingAssetsPath, "FileList.json");
            }

            //加载当前版本的文件列表信息 md5
            Dictionary<string, string> rDicCurFileList = new Dictionary<string, string>();
            string strJsonFileList = File.ReadAllText(strFileListPath);
            JsonReader readerCurFileList = new JsonReader(strJsonFileList);
            while (readerCurFileList.Read())
            {
                if (readerCurFileList.Token == JsonToken.ObjectStart || readerCurFileList.Token == JsonToken.ObjectEnd)
                {
                    continue;
                }
                if (readerCurFileList.Token == JsonToken.PropertyName)
                {
                    string strFile = readerCurFileList.Value.ToString();
                    readerCurFileList.Read();
                    string strMD5 = readerCurFileList.Value.ToString();
                    rDicCurFileList.Add(strFile, strMD5);
                    continue;
                }
            }

            //获取最新版本的文件列表信息 md5
            this.mWebReq = UnityWebRequest.Get(Path.Combine(strAssetsURL, "FileList.json"));
            this.mWebReq.timeout = 10;
            yield return this.mWebReq.SendWebRequest();

            if (string.IsNullOrEmpty(this.mWebReq.error) == false)
            {
                this.mVerifyState = EVerifyState.ERROR;
                yield break;
            }

            Dictionary<string, string> rDicLastFileList = new Dictionary<string, string>();
            MemoryStream ms = new MemoryStream(this.mWebReq.downloadHandler.data);
            StreamReader sr = new StreamReader(ms);
            JsonReader readerLastFileList = new JsonReader(sr.ReadToEnd());
            while (readerLastFileList.Read())
            {
                if (readerLastFileList.Token == JsonToken.ObjectStart || readerLastFileList.Token == JsonToken.ObjectEnd)
                {
                    continue;
                }
                if (readerLastFileList.Token == JsonToken.PropertyName)
                {
                    string strFile = readerLastFileList.Value.ToString();
                    readerLastFileList.Read();
                    string strMD5 = readerLastFileList.Value.ToString();
                    rDicLastFileList.Add(strFile, strMD5);
                    continue;
                }
            }
            sr.Close();

            //找到需要更新的文件
            foreach (var lastPair in rDicLastFileList)
            {
                if (rDicCurFileList.ContainsKey(lastPair.Key) == true)
                {
                    string strCurMD5 = rDicCurFileList[lastPair.Key];
                    if (strCurMD5 != lastPair.Value)
                    {
                        rToUpdate.Add(lastPair.Key);
                    }
                }
                else
                {
                    rToUpdate.Add(lastPair.Key);
                }
            }

            //已经更新过的文件就不更新了
            rDownloadedFiles = PersistenceHelper.GetList<string>("HotfixDownloadedFiles");
            if (rDownloadedFiles == null)
            {
                rDownloadedFiles = new PersistenceList<string>();
            }
            else
            {
                foreach (string strExisted in rDownloadedFiles.Data)
                {
                    if (rToUpdate.Contains(strExisted) == true)
                    {
                        rToUpdate.Remove(strExisted);
                    }
                }
            }

            this.mUI.SetProgress(1, 1.0f);
        }

        //更新新文件
        {
            this.mVerifyState = EVerifyState.UPDATE_AB_FILE;
            this.mUI.SetProgress(2, 0);

            int nCount = 0;
            foreach (string file in rToUpdate)
            {
                this.mWebReq = UnityWebRequest.Get(Path.Combine(strAssetsURL, file));
                this.mWebReq.timeout = 10;
                string strLocalPath = Path.Combine(Application.persistentDataPath, file);
                this.mWebReq.downloadHandler = new DownloadHandlerFile(strLocalPath);
                yield return this.mWebReq.SendWebRequest();

                if (string.IsNullOrEmpty(this.mWebReq.error) == false)
                {
                    this.mVerifyState = EVerifyState.ERROR;
                    yield break;
                }

                rDownloadedFiles.Data.Add(file);
                PersistenceHelper.SetList<string>("HotfixDownloadedFiles", rDownloadedFiles);
                PersistenceHelper.Save();

                nCount++;
                if (nCount > 3)
                {
                    yield break;
                }
            }

            PersistenceHelper.SetString("GameVersion", this.mLastestVersion);
            PersistenceHelper.ClearKey("HotfixDownloadedFiles");
            PersistenceHelper.Save();

            this.mUI.SetProgress(2, 1.0f);
            this.mVerifyState = EVerifyState.FINISH;
        }
    }

    public override void OnUpdateState()
    {
        switch (this.mVerifyState)
        {
            case EVerifyState.VERIFY_VERSION:
                {
                    this.mUI.SetProgress(1, this.mWebReq.downloadProgress);
                }
                break;
            case EVerifyState.VERIFY_FILELIST:
                {
                    this.mUI.SetProgress(1, this.mWebReq.downloadProgress);
                }
                break;
            case EVerifyState.UPDATE_AB_FILE:
                {
                    this.mUI.SetProgress(2, this.mWebReq.downloadProgress);
                }
                break;
            case EVerifyState.FINISH:
                {
                    this.FSM.SetState(this.mState + 1);
                }
                break;
            case EVerifyState.ERROR:
                {
                    this.mVerifyState = EVerifyState.WAIT;
                }
                break;
            default:
                break;
        }
    }

    public override void OnExitState()
    {
        this.mWebReq?.Dispose();
    }

}
