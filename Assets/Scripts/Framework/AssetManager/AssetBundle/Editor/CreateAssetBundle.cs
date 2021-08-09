using System.IO;
using UnityEditor;
using UnityEngine;

namespace TFramework
{
    public class CreateAssetBundle : Editor
    {
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
    }
}
