using UnityEditor;
using UnityEngine;
using System;
using System.IO;

namespace TFramework
{
    public class ClearPersistentFolder : EditorWindow
    {
        [MenuItem("TTool/Clear Persistent Folder", priority = 200)]
        public static void Init()
        {
            Directory.Delete(Application.persistentDataPath, true);
            Debug.Log("Clear Done!");
        }
    }
}
