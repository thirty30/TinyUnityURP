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
            //��һ�ν���Ϸ���汾�Ŵ������ļ���
            string strVersionFile = Path.Combine(Application.streamingAssetsPath, "BasicConfig.json");
            string strJsonVersion = File.ReadAllText(strVersionFile);
            JsonData rBasicCfgJD = JsonMapper.ToObject(strJsonVersion);
            this.mCurVersion = rBasicCfgJD["Version"].ToString();
            this.mCDN = rBasicCfgJD["CDN"].ToString();
        }
        else
        {
            //����Ϸ��汾�Ŵӻ������
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

        //���ذ汾�ļ�����֤�汾��Ϣ
        {
            //��ȡ�汾ָ���ļ�
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

            //�Ƚϰ汾�Ƿ�һ��
            if (this.mLastestVersion == this.mCurVersion)
            {
                //û����Ҫ���µ��°汾
                this.mUI.SetProgress(1, 1.0f);
                this.mVerifyState = EVerifyState.FINISH;
                yield break;
            }

            //����汾��һ�£��ҵ�����Ŀ¼
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
        //�ҵ���Ҫ���µ��ļ�
        {
            this.mVerifyState = EVerifyState.VERIFY_VERSION;

            //�жϵ�ǰ�汾�ļ��б��Ƿ����PersistentĿ¼
            string strFileListPath = Path.Combine(Application.persistentDataPath, "FileList.json");
            if (File.Exists(strFileListPath) == false)
            {
                strFileListPath = Path.Combine(Application.streamingAssetsPath, "FileList.json");
            }

            //���ص�ǰ�汾���ļ��б���Ϣ md5
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

            //��ȡ���°汾���ļ��б���Ϣ md5
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

            //�ҵ���Ҫ���µ��ļ�
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

            //�Ѿ����¹����ļ��Ͳ�������
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

        //�������ļ�
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
