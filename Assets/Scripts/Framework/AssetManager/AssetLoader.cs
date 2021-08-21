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
        private static EAssetLoadType LoadType = EAssetLoadType.ASSET_BUNDLE;
        private static string ABFolderName;
        private static List<AssetBundle> ABList = new List<AssetBundle>();

        public static void Init(EAssetLoadType aType)
        {
            LoadType = aType;

            /*
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            ABFolderName = "Windows";
#elif UNITY_EDITOR_OSX
        ABFolderName = "MacOS";
#elif UNITY_ANDROID
        ABFolderName = "Android";
#elif UNITY_IOS
        ABFolderName = "IOS";
#endif
            */
        }

        public static Object Load(string aFile)
        {
            if (LoadType == EAssetLoadType.RESOURCES)
            {
                return ResourceLoad(aFile);
            }
            else if (LoadType == EAssetLoadType.ASSET_BUNDLE)
            {
                return ABLoad(aFile);
            }
            return null;
        }

        public static T Load<T>(string aFile) where T : Object
        {
            if (LoadType == EAssetLoadType.RESOURCES)
            {
                return ResourceLoad<T>(aFile);
            }
            else if (LoadType == EAssetLoadType.ASSET_BUNDLE)
            {
                return ABLoad<T>(aFile);
            }
            return null;
        }

        public static Object[] LoadAll(string aFile)
        {
            if (LoadType == EAssetLoadType.RESOURCES)
            {
                return ResourceLoadAll(aFile);
            }
            else if (LoadType == EAssetLoadType.ASSET_BUNDLE)
            {
                return ABLoadAll(aFile);
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////

        public static T ResourceLoad<T>(string aFilePath) where T : Object
        {
            return Resources.Load<T>(aFilePath);
        }

        public static Object ResourceLoad(string aFilePath)
        {
            return Resources.Load(aFilePath);
        }

        public static Object[] ResourceLoadAll(string aFolder)
        {
            return Resources.LoadAll(aFolder);
        }

        public static IEnumerator AsyncResourceLoad(string aFilePath, AsyncLoadCallback aCallback)
        {
            ResourceRequest req = Resources.LoadAsync(aFilePath);
            yield return req.isDone;
            if (aCallback != null) { aCallback(req.asset); };
        }

        public static void ResourceUnloadAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        ////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////

        public static AssetBundle LoadAB(string aAssetBundleName)
        {
            //MD5 md5 = new MD5CryptoServiceProvider();
            //byte[] strBytes = Encoding.UTF8.GetBytes(aAssetBundleName);
            //byte[] retVal = md5.ComputeHash(strBytes);
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < retVal.Length; i++)
            //{
            //    sb.Append(retVal[i].ToString("x2"));
            //}

            string strPath = "";
            AssetBundle ab = null;
            //判断PersistentDataPath里面有没有文件
            strPath = Path.Combine(Application.persistentDataPath, aAssetBundleName);
            if (File.Exists(strPath) == true)
            {
                ab = AssetBundle.LoadFromFile(strPath);
                if (ab == null)
                {
                    Debug.LogError("Failed to load the AssetBundle: " + strPath);
                    return null;
                }
                ABList.Add(ab);
                return ab;
            }


            //如果PersistentDataPath里面没有文件，那么就从StreamingAssetsPath加载
            strPath = Path.Combine(Application.streamingAssetsPath, aAssetBundleName);
            ab = AssetBundle.LoadFromFile(strPath);
            if (ab == null)
            {
                Debug.LogError("Failed to load the AssetBundle: " + strPath);
                return null;
            }
            ABList.Add(ab);
            return ab;
        }

        public static T ABLoad<T>(string aFileName) where T : Object
        {
            foreach (AssetBundle ab in ABList)
            {
                if (ab.Contains(aFileName) == false)
                {
                    continue;
                }
                return ab.LoadAsset<T>(aFileName);
            }
            return null;
        }

        public static Object ABLoad(string aFileName)
        {
            foreach (AssetBundle ab in ABList)
            {
                if (ab.Contains(aFileName) == false)
                {
                    continue;
                }
                return ab.LoadAsset(aFileName);
            }
            return null;
        }

        public static Object[] ABLoadAll(string aABName)
        {
            foreach (AssetBundle ab in ABList)
            {
                if (ab.name == aABName)
                {
                    return ab.LoadAllAssets();
                }
            }
            return null;
        }

        public static void ABUnload(string aABName)
        {
            AssetBundle temp = null;
            foreach (AssetBundle ab in ABList)
            {
                if (ab.name == aABName)
                {
                    ab.Unload(true);
                    break;
                }
            }
            if (temp != null)
            {
                ABList.Remove(temp);
            }
        }

        public static void ABUnloadAssets(bool aUnloadAllLoadedAssets)
        {
            foreach (AssetBundle ab in ABList)
            {
                ab.Unload(aUnloadAllLoadedAssets);
            }
        }

    }
}

