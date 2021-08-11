using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace TFramework
{
    public class HotfixManager : CSingleton<HotfixManager>
    {
        //AppDomain��ILRuntime����ڣ�������Ϸȫ�־�һ��
        public ILRuntime.Runtime.Enviorment.AppDomain ILRuntimeApp;

        public System.Action RegisterDelegate;//ע��ί��
        public System.Action RegisterAdapter;//ע��������
        public System.Action RegisterCLRRedirection;//CLR�ض���
        public System.Action RegisterCLRBinding;//CLR��

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

#if DEBUG

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

            //����Unity��Profiler�ӿ�ֻ���������߳�ʹ�ã�Ϊ�˱�����쳣����Ҫ����ILRuntime���̵߳��߳�ID������ȷ���������к�ʱ�����Profiler
            this.ILRuntimeApp.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;

            //�������Զ˿�
            this.ILRuntimeApp.DebugService.StartDebugService(56000);

#else
            this.mDllStream = new MemoryStream(dllData);
            try
            {
                this.ILRuntimeApp.LoadAssembly(this.mDllStream);
            }
            catch
            {
                Debug.LogError("Load hotfix lib fail!");
            }
#endif

            //������һЩILRuntime��ע��

            //ע��LitJson
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(this.ILRuntimeApp);

            //ע��ί��
            this.RegisterDelegate?.Invoke();

            //ע��������
            this.RegisterAdapter?.Invoke();

            //CLR�ض���
            this.RegisterCLRRedirection?.Invoke();

            //CLR��
            this.RegisterCLRBinding?.Invoke();
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

