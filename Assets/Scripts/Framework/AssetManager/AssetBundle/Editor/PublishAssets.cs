using LitJson;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TFramework
{
    public class PublishAssets : EditorWindow
    {
        public enum ETargetPlatform
        {
            WINDOWS,
            MAC_OS,
            ANDROID,
            IOS
        }
        
        public class BasicConfig
        {
            public string Version;
            public string CDN;
        }

        private string mVersion = "1.0.0";
        private string mCDN = "127.0.0.1/version";
        private string mBasicConfigName = "BasicConfig.json";
        private ETargetPlatform mTargetPlatform = ETargetPlatform.WINDOWS;
        private string mOutputPath = string.Empty;
        private string mFileListName = "FileList.json";

        [MenuItem("TTool/Publish Assets", priority = 100)]
        public static void Init()
        {
            PublishAssets window = GetWindow<PublishAssets>();
            window.Show();
            window.titleContent.text = "Publish Assets";
        }

        private void OnGUI()
        {
            if (string.IsNullOrEmpty(this.mOutputPath) == true) { this.mOutputPath = Application.streamingAssetsPath + "/PublishAssets/"; }

            this.mVersion = EditorGUILayout.TextField("Version:", this.mVersion);
            this.mCDN = EditorGUILayout.TextField("CDN:", this.mCDN);
            this.mBasicConfigName = EditorGUILayout.TextField("BasicConfig Name:", this.mBasicConfigName);
            this.mTargetPlatform = (ETargetPlatform)EditorGUILayout.EnumPopup("Target Platform:", this.mTargetPlatform);
            this.mOutputPath = EditorGUILayout.TextField("Output Path:", this.mOutputPath);
            this.mFileListName = EditorGUILayout.TextField("FileList Name:", this.mFileListName);

            GUILayout.Space(10.0f);
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Generate AssetBundle") == true)
                {
                    //打包资源
                    this.GenerateAssetBundle();
                }
                if (GUILayout.Button("Generate File List") == true)
                {
                    //生成MD5文件列表
                    this.GenerateMD5File();
                }
                if (GUILayout.Button("Rename Files") == true)
                {
                    //RenameFiles(this.mTargetPath);
                }
            }
            GUILayout.EndHorizontal();
        }

        private string GetPublishPath()
        {
            string strPath = "";
            if (this.mTargetPlatform == ETargetPlatform.WINDOWS)
            {
                strPath = Path.Combine(this.mOutputPath, "Windows", this.mVersion);
            }
            else if (this.mTargetPlatform == ETargetPlatform.MAC_OS)
            {
                strPath = Path.Combine(this.mOutputPath, "MacOS", this.mVersion);
            }
            else if (this.mTargetPlatform == ETargetPlatform.ANDROID)
            {
                strPath = Path.Combine(this.mOutputPath, "Android", this.mVersion);
            }
            else if (this.mTargetPlatform == ETargetPlatform.IOS)
            {
                strPath = Path.Combine(this.mOutputPath, "IOS", this.mVersion);
            }
            return strPath;
        }

        public void GenerateAssetBundle()
        {
            string strPath = "";
            BuildTarget eTarget = BuildTarget.NoTarget;

            if (this.mTargetPlatform == ETargetPlatform.WINDOWS)
            {
                strPath = Path.Combine(this.mOutputPath, "Windows", this.mVersion);
                eTarget = BuildTarget.StandaloneWindows64;
            }
            else if (this.mTargetPlatform == ETargetPlatform.MAC_OS)
            {
                strPath = Path.Combine(this.mOutputPath, "MacOS", this.mVersion);
                eTarget = BuildTarget.StandaloneOSX;
            }
            else if (this.mTargetPlatform == ETargetPlatform.ANDROID)
            {
                strPath = Path.Combine(this.mOutputPath, "Android", this.mVersion);
                eTarget = BuildTarget.Android;
            }
            else if (this.mTargetPlatform == ETargetPlatform.IOS)
            {
                strPath = Path.Combine(this.mOutputPath, "IOS", this.mVersion);
                eTarget = BuildTarget.iOS;
            }

            //清空目录
            if (Directory.Exists(strPath) == true) { Directory.Delete(strPath, true); }
            Directory.CreateDirectory(strPath);

            //基础配置写入文件
            {
                BasicConfig cfg = new BasicConfig();
                cfg.Version = this.mVersion;
                cfg.CDN = this.mCDN;
                string strJson = JsonMapper.ToJson(cfg);
                string strCfgPath = Path.Combine(strPath, this.mBasicConfigName);
                FileStream fs = new FileStream(strCfgPath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(strJson);
                sw.Close();
            }

            //编译热更新代码
            {
                //todo
            }

            //打包AB
            BuildPipeline.BuildAssetBundles(strPath, BuildAssetBundleOptions.ChunkBasedCompression, eTarget);

            Debug.Log("Generate AB successfully!");
        }

        public void GenerateMD5File()
        {
            string strPath = this.GetPublishPath();
            string strFileListPath = Path.Combine(strPath, this.mFileListName);
            File.Delete(strFileListPath);

            List<string> rABFiles = new List<string>();
            GetFiles(strPath, rABFiles);

            Dictionary<string, string> rFileList = new Dictionary<string, string>();
            foreach (string info in rABFiles)
            {
                FileStream file = new FileStream(info, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                string k = info.Replace(strPath + "/", "");
                string v = sb.ToString();
                rFileList.Add(k, v);
            }

            string strJson = JsonMapper.ToJson(rFileList);
            FileStream fs = new FileStream(strFileListPath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(strJson);
            sw.Flush();
            sw.Close();
            Debug.Log("Generate AB FileList successfully!");
        }

        public static void GetFiles(string aPath, List<string> aFileList)
        {
            DirectoryInfo dir = new DirectoryInfo(aPath);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (FileInfo f in files)
            {
                aFileList.Add(aPath + "/" + f.Name);
            }
            foreach (DirectoryInfo d in dirs)
            {
                GetFiles(d.FullName, aFileList);
            }
        }

        public static void RenameFiles(string aPath)
        {
            DirectoryInfo dir = new DirectoryInfo(aPath);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (FileInfo f in files)
            {
                string rFileName = f.Name;
                string[] s = rFileName.Split('.');
                bool bIsMeta = false;
                if (s[s.Length - 1] == "meta")
                {
                    rFileName = rFileName.Substring(0, rFileName.Length - 5);
                    bIsMeta = true;
                }

                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] strBytes = Encoding.UTF8.GetBytes(rFileName);
                byte[] retVal = md5.ComputeHash(strBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                if (bIsMeta == true)
                {
                    File.Move(f.FullName, Path.Combine(f.DirectoryName, sb.ToString() + ".meta"));
                }
                else
                {
                    File.Move(f.FullName, Path.Combine(f.DirectoryName, sb.ToString()));
                }
            }
            foreach (DirectoryInfo d in dirs)
            {
                RenameFiles(d.FullName);
            }
        }
    }
}
