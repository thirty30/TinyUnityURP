using System.Collections;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using UnityEngine.Networking;
using System.IO;

namespace TFramework
{
    public class HotfixManager : CSingleton<HotfixManager>
    {
        //AppDomain��ILRuntime����ڣ�������Ϸȫ�־�һ��
        public AppDomain ILRuntimeApp;

        private System.IO.MemoryStream mDllStream;
        private System.IO.MemoryStream mPdbStream;

        public IEnumerator InitILRuntime(string aLibName)
        {
            //���ȼ����ȸ���dll
            this.ILRuntimeApp = new ILRuntime.Runtime.Enviorment.AppDomain();
            string strDllName = "/Hotfix/" + aLibName + ".dll";
            string strPdbName = "/Hotfix/" + aLibName + ".pdb";

#if UNITY_ANDROID
        UnityWebRequest dllReq = new UnityWebRequest(Application.streamingAssetsPath + strDllName);
#elif UNITY_IOS
        UnityWebRequest dllReq = new UnityWebRequest(Application.streamingAssetsPath + strDllName);
#else
            UnityWebRequest dllReq = new UnityWebRequest("file:///" + Application.streamingAssetsPath + strDllName);
#endif
            dllReq.downloadHandler = new DownloadHandlerBuffer();
            yield return dllReq.SendWebRequest();
            if (string.IsNullOrEmpty(dllReq.error) == false)
            {
                UnityEngine.Debug.LogError(dllReq.error);
                yield break;
            }

            byte[] dllData = dllReq.downloadHandler.data;
            dllReq.Dispose();

            //PDB�ļ��ǵ������ݿ⣬����Ҫ����־����ʾ������кţ�������ṩPDB�ļ����������ڻ��������ڴ棬��ʽ����ʱ�뽫PDBȥ��������LoadAssembly��ʱ��pdb��null����
#if UNITY_ANDROID
        UnityWebRequest pdbReq = new UnityWebRequest(Application.streamingAssetsPath + strPdbName);
#elif UNITY_IOS
        UnityWebRequest pdbReq = new UnityWebRequest(Application.streamingAssetsPath + strPdbName);
#else
            UnityWebRequest pdbReq = new UnityWebRequest("file:///" + Application.streamingAssetsPath + strPdbName);
#endif
            pdbReq.downloadHandler = new DownloadHandlerBuffer();
            yield return pdbReq.SendWebRequest();
            if (string.IsNullOrEmpty(pdbReq.error) == false)
            {
                UnityEngine.Debug.LogError(pdbReq.error);
                yield break;
            }

            byte[] pdbData = pdbReq.downloadHandler.data;
            pdbReq.Dispose();

            this.mDllStream = new MemoryStream(dllData);
            this.mPdbStream = new MemoryStream(pdbData);
            try
            {
                this.ILRuntimeApp.LoadAssembly(this.mDllStream, this.mPdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            catch
            {
                Debug.LogError("Load hotfix lib fail!");
            }

            //��ʼ��ILRuntime
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //����Unity��Profiler�ӿ�ֻ���������߳�ʹ�ã�Ϊ�˱�����쳣����Ҫ����ILRuntime���̵߳��߳�ID������ȷ���������к�ʱ�����Profiler
            this.ILRuntimeApp.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            //������һЩILRuntime��ע��
            this.ILRuntimeApp.DebugService.StartDebugService(56000);
        }

        public void Clear()
        {
            if (this.mDllStream != null)
            {
                this.mDllStream.Close();
            }
            if (this.mPdbStream != null)
            {
                this.mPdbStream.Close();
            }
            this.mDllStream = null;
            this.mPdbStream = null;
        }
    }
}

