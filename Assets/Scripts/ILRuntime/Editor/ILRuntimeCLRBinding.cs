#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
   [MenuItem("ILRuntime/通过自动分析热更DLL生成CLR绑定")]
    static void GenerateCLRBindingByAnalysis()
    {
        //用新的分析热更dll调用引用来生成绑定代码
        AppDomain rAppDomain = new AppDomain();
        using (FileStream fs = new FileStream("Assets/StreamingAssets/Hotfix/HotfixGameplay.dll", FileMode.Open, FileAccess.Read))
        {
            rAppDomain.LoadAssembly(fs);
            InitILRuntime(rAppDomain);
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(rAppDomain, "Assets/Scripts/ILRuntime/CLRBinding");
        }

        AssetDatabase.Refresh();
    }

    //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain aAppDomain)
    {
        aAppDomain.RegisterCrossBindingAdaptor(new ILRuntimeAdapter.MonoBehaviourAdapter());
        aAppDomain.RegisterCrossBindingAdaptor(new ILRuntimeAdapter.CoroutineAdapter());
        aAppDomain.RegisterCrossBindingAdaptor(new ILRuntimeAdapter.TFSMStateBaseAdapter());

        aAppDomain.RegisterValueTypeBinder(typeof(Vector3), new ILRuntimeTypeBinder.Vector2Binder());
        aAppDomain.RegisterValueTypeBinder(typeof(Vector3), new ILRuntimeTypeBinder.Vector3Binder());
        aAppDomain.RegisterValueTypeBinder(typeof(Vector3), new ILRuntimeTypeBinder.QuaternionBinder());
    }
}
#endif
