using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TFramework
{
    public class ABFileInfo
    {
        public string FileName;
        public string FilePath;
    }

    public class GenerateABFileList : EditorWindow
    {
        private string mTargetPath = Application.streamingAssetsPath;
        private string mOutputPath = "D:/ToolDownload/nginx-1.16.1/html/version";

        [MenuItem("AssetsBundle/Generate AB File List")]
        public static void Init()
        {
            GenerateABFileList window = GetWindow<GenerateABFileList>();
            window.Show();
            window.titleContent.text = "Generate AB File List";
        }

        private void OnGUI()
        {
            this.mTargetPath = EditorGUILayout.TextField("Target Path:", this.mTargetPath);
            this.mOutputPath = EditorGUILayout.TextField("Output Path:", this.mOutputPath);

            GUILayout.Space(10.0f);
            if (GUILayout.Button("Generate") == true)
            {
                GenerateMD5File(this.mTargetPath, this.mOutputPath);
            }

            //GUILayout.Space(10.0f);
            //if (GUILayout.Button("RenameFiles") == true)
            //{
            //    RenameFiles(this.mTargetPath);
            //}
        }

        public static void GenerateMD5File(string aTargetPath, string aOutputPath)
        {
            //Object rJson = AssetDatabase.LoadAssetAtPath<Object>("Assets/LoadingConfig/Version.json");
            //LitJson.JsonData rJsonData = LitJson.JsonMapper.ToObject(rJson.ToString());
            //string strVersion = rJsonData["Version"].ToString();

            HotfixConfig rHotfixConfig = AssetDatabase.LoadAssetAtPath<HotfixConfig>("Assets/HotfixConfig.asset");
            string strVersion = rHotfixConfig.Version;
            string strCDNPath = Path.Combine(aOutputPath, strVersion);
            if (Directory.Exists(strCDNPath) == false) {  Directory.CreateDirectory(strCDNPath); }

            if (Directory.Exists(aTargetPath) == false) { return; }
            List<ABFileInfo> rFileList = new List<ABFileInfo>();
            GetFiles(aTargetPath, rFileList);

            string strFileList = Path.Combine(strCDNPath, "FileList");
            FileStream fs = new FileStream(strFileList, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            foreach (ABFileInfo info in rFileList)
            {
                FileStream file = new FileStream(info.FilePath, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                sw.Write(info.FileName + ":" + sb.ToString() + "\n");

                File.Copy(info.FilePath, Path.Combine(strCDNPath, info.FileName), true);
            }

            sw.Flush();
            sw.Close();
        }

        public static void GetFiles(string aPtah, List<ABFileInfo> aFileList)
        {
            DirectoryInfo dir = new DirectoryInfo(aPtah);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (FileInfo f in files)
            {
                aFileList.Add(new ABFileInfo { FileName = f.Name, FilePath = f.FullName });
            }
            foreach (DirectoryInfo d in dirs)
            {
                Debug.LogError("AB directory has nested directory!");
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
