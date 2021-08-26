using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace TFramework
{
    public enum EAssetLoadType
    {
        ASSET_BUNDLE,
        RESOURCES
    }

    public delegate void AsyncLoadCallback(Object aAsset);

    public static class AssetLoader
    {
        private class ABUnit
        {
            public AssetBundle AB;
            public int RefCount;
        }

        private static EAssetLoadType LoadType = EAssetLoadType.ASSET_BUNDLE;
        private static Dictionary<string, ABUnit> ABDic = new Dictionary<string, ABUnit>();
        private static AssetBundleManifest ABManifest = null;
        private static string ABFolderPath = "";

        public static void Init(EAssetLoadType aType)
        {
            LoadType = aType;
        }

        public static void SetABFolder(string aFolderName)
        {
            ABFolderPath = aFolderName;
        }

        public static Object LoadAsset(string aAssetPath)
        {
            if (LoadType == EAssetLoadType.RESOURCES)
            {
                return ResourceLoadAsset(aAssetPath);
            }
            else if (LoadType == EAssetLoadType.ASSET_BUNDLE)
            {
                return ABLoadAsset(aAssetPath);
            }
            return null;
        }

        public static T LoadAsset<T>(string aAssetPath) where T : Object
        {
            if (LoadType == EAssetLoadType.RESOURCES)
            {
                return ResourceLoadAsset<T>(aAssetPath);
            }
            else if (LoadType == EAssetLoadType.ASSET_BUNDLE)
            {
                return ABLoadAsset<T>(aAssetPath);
            }
            return null;
        }

        public static Object[] LoadAllAssets(string aAssetsPath)
        {
            if (LoadType == EAssetLoadType.RESOURCES)
            {
                return ResourceLoadAllAssets(aAssetsPath);
            }
            else if (LoadType == EAssetLoadType.ASSET_BUNDLE)
            {
                return ABLoadAllAssets(aAssetsPath);
            }
            return null;
        }

        #region Load From Resource
        ////////////////////////////////////////////////////////////////////////////////
        //Resource

        public static T ResourceLoadAsset<T>(string aFilePath) where T : Object
        {
            return Resources.Load<T>(aFilePath);
        }

        public static Object ResourceLoadAsset(string aFilePath)
        {
            return Resources.Load(aFilePath);
        }

        public static Object[] ResourceLoadAllAssets(string aFolder)
        {
            return Resources.LoadAll(aFolder);
        }

        public static IEnumerator AsyncResourceLoadAsset(string aFilePath, AsyncLoadCallback aCallback)
        {
            ResourceRequest req = Resources.LoadAsync(aFilePath);
            yield return req.isDone;
            if (aCallback != null) { aCallback(req.asset); };
        }

        public static void ResourceUnloadAsset(Object aObj)
        {
            Resources.UnloadAsset(aObj);
        }

        public static void ResourceUnloadAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        #endregion

        #region Load From AB
        ////////////////////////////////////////////////////////////////////////////////
        //AssetBundle

        private static ABUnit FindABUnit(string aABPath)
        {
            if (ABDic.ContainsKey(aABPath) == true)
            {
                return ABDic[aABPath];
            }
            return null;
        }

        private static string Cover2MD5Name(string aAssetBundleName)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] strBytes = Encoding.UTF8.GetBytes(aAssetBundleName);
            byte[] retVal = md5.ComputeHash(strBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static AssetBundle LoadAB(string aABPath)
        {
            //如果已经加载，就只增加引用计数
            ABUnit unit = FindABUnit(aABPath);
            if (unit != null)
            {
                unit.RefCount++;
                return unit.AB;
            }

            //加载依赖项
            if (ABManifest != null)
            {
                string[] dependencies = ABManifest.GetAllDependencies(aABPath);
                foreach (string strAB in dependencies)
                {
                    LoadAB(strAB);
                }
            }

            string strPath = "";
            AssetBundle ab = null;
            //判断PersistentDataPath里面有没有文件
            strPath = Path.Combine(Application.persistentDataPath, ABFolderPath, aABPath);
            if (File.Exists(strPath) == true)
            {
                ab = AssetBundle.LoadFromFile(strPath);
                if (ab == null)
                {
                    Debug.LogError("Failed to load the AssetBundle: " + strPath);
                    return null;
                }
            }
            else
            {
                //如果PersistentDataPath里面没有文件，那么就从StreamingAssetsPath加载
                strPath = Path.Combine(Application.streamingAssetsPath, ABFolderPath, aABPath);
                ab = AssetBundle.LoadFromFile(strPath);
                if (ab == null)
                {
                    Debug.LogError("Failed to load the AssetBundle: " + strPath);
                    return null;
                }
            }

            unit = new ABUnit();
            unit.AB = ab;
            unit.RefCount = 1;
            ABDic.Add(aABPath, unit);

            return ab;
        }

        public static void UnLoadAB(string aABPath)
        {
            if (ABManifest != null)
            {
                string[] dependencies = ABManifest.GetAllDependencies(aABPath);
                foreach (string strAB in dependencies)
                {
                    UnLoadAB(strAB);
                }
            }

            bool bRemove = false;
            foreach (var pair in ABDic)
            {
                if (pair.Key != aABPath)
                {
                    continue;
                }

                pair.Value.RefCount--;
                if (pair.Value.RefCount <= 0)
                {
                    pair.Value.AB.Unload(true);
                    bRemove = true;
                }
                break;
            }
            if (bRemove == true)
            {
                ABDic.Remove(aABPath);
            }
        }

        public static void LoadManifest(string aManifestName)
        {
            string strPath = Path.Combine(Application.persistentDataPath, ABFolderPath, aManifestName);
            AssetBundle ab = null;
            if (File.Exists(strPath) == true)
            {
                ab = AssetBundle.LoadFromFile(strPath);
                if (ab == null)
                {
                    Debug.LogError("Failed to load the AssetBundle: " + strPath);
                    return;
                }
            }
            else
            {
                //如果PersistentDataPath里面没有文件，那么就从StreamingAssetsPath加载
                strPath = Path.Combine(Application.streamingAssetsPath, ABFolderPath, aManifestName);
                ab = AssetBundle.LoadFromFile(strPath);
                if (ab == null)
                {
                    Debug.LogError("Failed to load the AssetBundle: " + strPath);
                    return;
                }
            }
            ABManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (ABManifest == null)
            {
                Debug.LogError("Load AB Manifest fail!");
            }
        }

        public static T ABLoadAsset<T>(string aFileName) where T : Object
        {
            foreach (var pair in ABDic)
            {
                if (pair.Value.AB.Contains(aFileName) == false)
                {
                    continue;
                }
                return pair.Value.AB.LoadAsset<T>(aFileName);
            }
            return null;
        }

        public static Object ABLoadAsset(string aFileName)
        {
            foreach (var pair in ABDic)
            {
                if (pair.Value.AB.Contains(aFileName) == false)
                {
                    continue;
                }
                return pair.Value.AB.LoadAsset(aFileName);
            }
            return null;
        }

        public static Object[] ABLoadAllAssets(string aABName)
        {
            foreach (var pair in ABDic)
            {
                if (pair.Key == aABName)
                {
                    return pair.Value.AB.LoadAllAssets();
                }
            }
            return null;
        }

        #endregion

    }
}

