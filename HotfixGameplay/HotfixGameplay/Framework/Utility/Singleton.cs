using UnityEngine;

namespace HotfixGameplay.Framework
{
    public class HotfixSingleton<T> where T : new()
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
}
