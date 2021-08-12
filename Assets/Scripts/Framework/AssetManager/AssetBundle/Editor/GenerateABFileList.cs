using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TFramework
{
    public class GenerateABFileList : Editor
    {
        [MenuItem("AssetsBundle/Generate AB File List")]
        static void GenerateMD5File()
        {
            string strPath = Path.Combine(Application.streamingAssetsPath, "Windows");
            if (Directory.Exists(strPath) == false) { return; }
            List<string> rFileList = new List<string>();
            GetFiles(strPath, rFileList);

            foreach (string name in rFileList)
            {
                FileStream file = new FileStream(name, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                Debug.Log(sb.ToString());
            }
        }

        static void GetFiles(string aPtah, List<string> aFileList)
        {
            DirectoryInfo dir = new DirectoryInfo(aPtah);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (FileInfo f in files)
            {
                aFileList.Add(f.FullName);
            }
            foreach (DirectoryInfo d in dirs)
            {
                GetFiles(d.FullName, aFileList);
            }
        }
    }
}
