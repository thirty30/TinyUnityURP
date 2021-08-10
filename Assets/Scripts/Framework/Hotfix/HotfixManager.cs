using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace TFramework
{
    public class HotfixManager : CSingleton<HotfixManager>
    {
        //AppDomain是ILRuntime的入口，整个游戏全局就一个
        public ILRuntime.Runtime.Enviorment.AppDomain ILRuntimeApp;

        public System.Action RegisterDelegate;//注册委托
        public System.Action RegisterAdapter;//注册适配器
        public System.Action RegisterCLRRedirection;//CLR重定向
        public System.Action RegisterCLRBinding;//CLR绑定

        private System.IO.MemoryStream mDllStream;
        private System.IO.MemoryStream mPdbStream;

        public IEnumerator InitILRuntime(string aLibName)
        {
            //首先加载热更新dll
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

            //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
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

            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            this.ILRuntimeApp.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;

            //开启调试端口
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

            //这里做一些ILRuntime的注册

            //注册委托
            this.RegisterDelegate?.Invoke();

            //注册适配器
            this.RegisterAdapter?.Invoke();

            //CLR重定向
            this.RegisterCLRRedirection?.Invoke();

            //CLR绑定
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

