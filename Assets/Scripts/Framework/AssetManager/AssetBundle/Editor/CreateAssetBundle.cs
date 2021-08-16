using System.IO;
using UnityEditor;
using UnityEngine;

namespace TFramework
{
    public class CreateAssetBundle : Editor
    {
        [MenuItem("TTool/Build AssetBundle")]
        static void BuildAssetBundle()
        {
            string strPath = Application.streamingAssetsPath;
            if (Directory.Exists(strPath) == false) { Directory.CreateDirectory(strPath); }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            BuildPipeline.BuildAssetBundles(strPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
#elif UNITY_EDITOR_OSX
            BuildPipeline.BuildAssetBundles(strPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);
#elif UNITY_ANDROID
            BuildPipeline.BuildAssetBundles(strPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
#elif UNITY_IOS
            BuildPipeline.BuildAssetBundles(strPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
#endif
        }

        /*
        [MenuItem("AssetsBundle/Build For Windows")]
        static void Build4Windows()
        {
            string strPath = Path.Combine(Application.streamingAssetsPath, "Windows");
            if (Directory.Exists(strPath) == false) { Directory.CreateDirectory(strPath); }
            BuildPipeline.BuildAssetBundles(strPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("AssetsBundle/Build For MacOS")]
        static void Build4MacOS()
        {
            string strPath = Path.Combine(Application.streamingAssetsPath, "MacOS");
            if (Directory.Exists(strPath) == false) { Directory.CreateDirectory(strPath); }
            BuildPipeline.BuildAssetBundles(strPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);
        }

        [MenuItem("AssetsBundle/Build For Android")]
        static void Build4Android()
        {
            string strPath = Path.Combine(Application.streamingAssetsPath, "Android");
            if (Directory.Exists(strPath) == false) { Directory.CreateDirectory(strPath); }
            BuildPipeline.BuildAssetBundles(strPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
        }

        [MenuItem("AssetsBundle/Build For IOS")]
        static void Build4IOS()
        {
            string strPath = Path.Combine(Application.streamingAssetsPath, "IOS");
            if (Directory.Exists(strPath) == false) { Directory.CreateDirectory(strPath); }
            BuildPipeline.BuildAssetBundles(strPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
        }
        */

    }
}
