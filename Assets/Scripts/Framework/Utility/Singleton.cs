using UnityEngine;

namespace TFramework
{
    public class CSingleton<T> where T : new()
    {
        private static T _instance;
        public static T GetSingleton()
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    public class MBSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        public static T GetSingleton()
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name, typeof(T));
                    _instance = go.GetComponent<T>();
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }
}

