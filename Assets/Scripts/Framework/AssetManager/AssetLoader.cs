using System.Collections;
using System.Collections.Generic;
using System.IO;
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
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            ABFolderName = "Windows";
#elif UNITY_EDITOR_OSX
        ABFolderName = "MacOS";
#elif UNITY_ANDROID
        ABFolderName = "Android";
#elif UNITY_IOS
        ABFolderName = "IOS";
#endif
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
            string strPath = Path.Combine(Application.streamingAssetsPath, ABFolderName, aAssetBundleName);
            AssetBundle ab = AssetBundle.LoadFromFile(strPath);
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

        public static void ABUnloadAssets(bool aUnloadAllLoadedAssets)
        {
            foreach (AssetBundle ab in ABList)
            {
                ab.Unload(aUnloadAllLoadedAssets);
            }
        }

    }
}
