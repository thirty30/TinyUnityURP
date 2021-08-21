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

    public VerifyAsset(int aState) : base(aState)
    {

    }

    public override void OnEnterState()
    {
#if UNITY_EDITOR_WIN
        //this.FSM.SetState(this.mState + 1);
#endif
        //�ж��ļ��Ƿ����
        string strVersionFile = Path.Combine(Application.persistentDataPath, "_version.json");
        if (File.Exists(strVersionFile) == false)
        {
            strVersionFile = Path.Combine(Application.streamingAssetsPath, "_version.json");
        }

        StreamReader reader = new StreamReader(strVersionFile);
        JsonData rJD= JsonMapper.ToObject(reader.ReadToEnd());
        reader.Close();
        this.mCurVersion = rJD["version"].ToString();
        this.mCDN = rJD["cdn"].ToString();

        this.mUI = TUIManager.GetSingleton().GetUIObject("UIMainLoading") as UIMainLoading;
        this.mVerifyState = EVerifyState.VERIFY_VERSION;
    }

    public override void OnUpdateState()
    {
        switch (this.mVerifyState)
        {
            case EVerifyState.VERIFY_VERSION:
                {
                    //���ذ汾�ļ�����֤�汾��Ϣ
                    Main.GetSingleton().DelegateCoroutine(this.DownloadVersionFile());
                    this.mVerifyState = EVerifyState.WAIT;
                }
                break;
            case EVerifyState.VERIFY_FILELIST:
                {
                    //�Ա�������Դ��׼������
                    Main.GetSingleton().DelegateCoroutine(this.DownloadFileList());
                    this.mVerifyState = EVerifyState.WAIT;
                }
                break;
            case EVerifyState.UPDATE_AB_FILE:
                {

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

    }

    IEnumerator DownloadVersionFile()
    {
        this.mUI.SetProgress(1, 0);

        //��ȡ�汾�б�
        UnityWebRequest req = UnityWebRequest.Get(Path.Combine(this.mCDN, "version_list"));
        req.timeout = 10;
        yield return req.SendWebRequest();

        if (string.IsNullOrEmpty(req.error) == false)
        {
            this.mVerifyState = EVerifyState.ERROR;
            yield break;
        }

        List<string> rVersionList = new List<string>();
        MemoryStream ms = new MemoryStream(req.downloadHandler.data);
        StreamReader sr = new StreamReader(ms);
        while(sr.EndOfStream == false)
        {
            string strText = sr.ReadLine();
            if (strText == "")
            {
                continue;
            }
            rVersionList.Add(strText);
        }
        sr.Close();

        //�ԱȰ汾�Ƿ���Ҫ����
        this.mLastestVersion = rVersionList[rVersionList.Count - 1];
        if (this.mLastestVersion == this.mCurVersion)
        {
            //û����Ҫ���µ��°汾
            this.mUI.SetProgress(1, 1.0f);
            this.mVerifyState = EVerifyState.FINISH;
            Debug.Log("No update...");
        }
        else
        {
            this.mVerifyState = EVerifyState.VERIFY_FILELIST;
        }

        //FileStream rStream = new FileStream(Application.streamingAssetsPath, FileMode.Create);
        //rStream.Write(rData, 0, rData.Length);
        //rStream.Close();
    }

    IEnumerator DownloadFileList()
    {
        this.mUI.SetProgress(2, 0);

        string strListFile = Path.Combine(Application.persistentDataPath, "_file_list");
        //�ж��ļ��Ƿ����
        if (File.Exists(strListFile) == false)
        {
            strListFile = Path.Combine(Application.streamingAssetsPath, "_file_list");
        }

        //���ص�ǰ�汾���ļ��б���Ϣ md5
        StreamReader reader = new StreamReader(strListFile);
        List<string> rCurFileList = new List<string>();
        while (reader.EndOfStream == false)
        {
            string strText = reader.ReadLine();
            if (strText == "")
            {
                continue;
            }
            rCurFileList.Add(strText);
        }
        reader.Close();

        //��ȡ���°汾���ļ��б���Ϣ md5
        UnityWebRequest req = UnityWebRequest.Get(Path.Combine(this.mCDN, this.mLastestVersion, "_file_list"));
        req.timeout = 10;
        yield return req.SendWebRequest();

        if (string.IsNullOrEmpty(req.error) == false)
        {
            this.mVerifyState = EVerifyState.ERROR;
            yield break;
        }

        List<string> rNewFileList = new List<string>();
        MemoryStream ms = new MemoryStream(req.downloadHandler.data);
        StreamReader sr = new StreamReader(ms);
        while (sr.EndOfStream == false)
        {
            string strText = sr.ReadLine();
            if (strText == "")
            {
                continue;
            }
            rNewFileList.Add(strText);
        }
        sr.Close();

        //�Ƚϰ汾�ļ�����
        List<string> rToUpdate = new List<string>();
        foreach (string newFile in rNewFileList)
        {
            bool bIsDifferent = true;
            foreach (string curFile in rCurFileList)
            {
                if (newFile == curFile)
                {
                    bIsDifferent = false;
                    break;
                }
            }

            if (bIsDifferent == true)
            {
                rToUpdate.Add(newFile);
            }
        }

        //�������ļ�
        foreach (string file in rToUpdate)
        {
            string strFileName = file.Split(':')[0];
            //if (strFileName.Contains("main_loading") == true || strFileName.Contains("StreamingAssets") == true)
            //{
            //    continue;
            //}
            //string strMD5 = file.Split(':')[1];

            UnityWebRequest reqUpdate = UnityWebRequest.Get(Path.Combine(this.mCDN, this.mLastestVersion, strFileName));
            reqUpdate.timeout = 10;
            string strLocalPath = Path.Combine(Application.persistentDataPath, strFileName);
            reqUpdate.downloadHandler = new DownloadHandlerFile(strLocalPath);
            yield return reqUpdate.SendWebRequest();
            
            if (string.IsNullOrEmpty(reqUpdate.error) == false)
            {
                this.mVerifyState = EVerifyState.ERROR;
                yield break;
            }

            //UnityWebRequest reqUpdate = UnityWebRequestAssetBundle.GetAssetBundle(Path.Combine(this.mCDN, this.mLastestVersion, strFileName));
            //reqUpdate.timeout = 10;
            //yield return reqUpdate.SendWebRequest();
            //
            //if (reqUpdate.result != UnityWebRequest.Result.Success)
            //{
            //    this.mVerifyState = EVerifyState.ERROR;
            //    yield break;
            //}
            //DownloadHandlerAssetBundle.GetContent(reqUpdate);
        }

        this.mVerifyState = EVerifyState.FINISH;
    }
}
