using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// 改序列化方式是基于JsonUtility的，由于JsonUtility不能直接序列化 Array List Dictionary 类型，所以在这些数据结构上增加了Persistence类
// 其中当 List 和 Array 被放在一个对象结构中的时候，他们可以直接被JsonUtility序列化，但是Dictionary不能
// 当 PersistenceArray PersistenceList PersistenceDictionary 被放进对象中时，不能被序列化，所以它们必须在最外面一层由JsonUtility直接序列化
// 例如：
// JsonUtility.ToJson(PersistenceArray) 
// JsonUtility.ToJson(PersistenceList)
// JsonUtility.ToJson(PersistenceDictionary)
// 综上：第一层结构中要使用List和Array的话，PersistenceList 和 PersistenceArray 
// 如果在第二层以后的结构中，必须直接用List 和 Array
// 如果要使用Dictionary，只能使用 PersistenceDictionary 并且 放在第一层结构中
// 新的对象结构需要用[Serializable]属性

namespace TFramework
{
    [Serializable]
    public class PersistenceArray<T>
    {
        [SerializeField]
        public T[] Data;
    }

    [Serializable]
    public class PersistenceList<T>
    {
        [SerializeField]
        public List<T> Data = new List<T>();
    }

    [Serializable]
    public class PersistenceDictionary<K, V> : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<K> mKeys;
        [SerializeField]
        List<V> mValues;

        public Dictionary<K, V> Data = new Dictionary<K, V>();

        public void OnBeforeSerialize()
        {
            this.mKeys = new List<K>(this.Data.Keys);
            this.mValues = new List<V>(this.Data.Values);
        }

        public void OnAfterDeserialize()
        {
            int nCount = this.mKeys.Count;
            for (int i = 0; i < nCount; i++)
            {
                this.Data.Add(this.mKeys[i], this.mValues[i]);
            }
        }
    }

    public class PersistenceObject
    {
        public void Save()
        {
            Type rType = this.GetType();
            FieldInfo[] rFieldInfos = rType.GetFields();
            for (int i = 0; i < rFieldInfos.Length; i++)
            {
                FieldInfo rFieldInfo = rFieldInfos[i];
                if (rFieldInfo.FieldType == typeof(int))
                {
                    PlayerPrefs.SetInt(rFieldInfo.Name, (int)rFieldInfo.GetValue(this));
                }
                else if (rFieldInfo.FieldType == typeof(long))
                {
                    PlayerPrefs.SetString(rFieldInfo.Name, ((long)rFieldInfo.GetValue(this)).ToString());
                }
                else if (rFieldInfo.FieldType == typeof(float))
                {
                    PlayerPrefs.SetFloat(rFieldInfo.Name, (float)rFieldInfo.GetValue(this));
                }
                else if (rFieldInfo.FieldType == typeof(double))
                {
                    PlayerPrefs.SetString(rFieldInfo.Name, ((double)rFieldInfo.GetValue(this)).ToString());
                }
                else if (rFieldInfo.FieldType == typeof(string))
                {
                    PlayerPrefs.SetString(rFieldInfo.Name, (string)rFieldInfo.GetValue(this));
                }
                else
                {
                    string strValue = JsonUtility.ToJson(rFieldInfo.GetValue(this));
                    PlayerPrefs.SetString(rFieldInfo.Name, strValue);
                }
            }
            PlayerPrefs.Save();
        }

        public void Load()
        {
            Type rType = this.GetType();
            FieldInfo[] rFieldInfos = rType.GetFields();
            for (int i = 0; i < rFieldInfos.Length; i++)
            {
                FieldInfo rFieldInfo = rFieldInfos[i];
                if (rFieldInfo.FieldType == typeof(int))
                {
                    rFieldInfo.SetValue(this, PlayerPrefs.GetInt(rFieldInfo.Name, 0));
                }
                else if (rFieldInfo.FieldType == typeof(long))
                {
                    string strValue = PlayerPrefs.GetString(rFieldInfo.Name, "0");
                    rFieldInfo.SetValue(this, long.Parse(strValue));
                }
                else if (rFieldInfo.FieldType == typeof(float))
                {
                    rFieldInfo.SetValue(this, PlayerPrefs.GetFloat(rFieldInfo.Name, 0));
                }
                else if (rFieldInfo.FieldType == typeof(double))
                {
                    string strValue = PlayerPrefs.GetString(rFieldInfo.Name, "0");
                    rFieldInfo.SetValue(this, double.Parse(strValue));
                }
                else if (rFieldInfo.FieldType == typeof(string))
                {
                    rFieldInfo.SetValue(this, PlayerPrefs.GetString(rFieldInfo.Name));
                }
                else
                {
                    string strValue = PlayerPrefs.GetString(rFieldInfo.Name, "");
                    if (strValue.Length > 0)
                    {
                        rFieldInfo.SetValue(this, JsonUtility.FromJson(strValue, rFieldInfo.FieldType));
                    }
                }
            }
        }
    }

    public static class PersistenceHelper
    {
        public static void Save()
        {
            PlayerPrefs.Save();
        }
        public static void ClearAll()
        {
            PlayerPrefs.DeleteAll();
        }
        public static void ClearKey(string aKey)
        {
            PlayerPrefs.DeleteKey(aKey);
        }

        public static void SetInt(string aKey, int aValue)
        {
            PlayerPrefs.SetInt(aKey, aValue);
        }
        public static void SetLong(string aKey, long aValue)
        {
            PlayerPrefs.SetString(aKey, aValue.ToString());
        }
        public static void SetFloat(string aKey, float aValue)
        {
            PlayerPrefs.SetFloat(aKey, aValue);
        }
        public static void SetDouble(string aKey, double aValue)
        {
            PlayerPrefs.SetString(aKey, aValue.ToString());
        }
        public static void SetString(string aKey, string aValue)
        {
            PlayerPrefs.SetString(aKey, aValue);
        }
        public static void SetArray<T>(string aKey, PersistenceArray<T> aValue)
        {
            string strValue = JsonUtility.ToJson(aValue);
            PlayerPrefs.SetString(aKey, strValue);
        }
        public static void SetList<T>(string aKey, PersistenceList<T> aValue)
        {
            string strValue = JsonUtility.ToJson(aValue);
            PlayerPrefs.SetString(aKey, strValue);
        }
        public static void SetDictionary<K, V>(string aKey, PersistenceDictionary<K, V> aValue)
        {
            string strValue = JsonUtility.ToJson(aValue);
            PlayerPrefs.SetString(aKey, strValue);
        }

        public static int GetInt(string aKey)
        {
            return PlayerPrefs.GetInt(aKey, 0);
        }
        public static long GetLong(string aKey)
        {
            string strValue = PlayerPrefs.GetString(aKey, "0");
            return long.Parse(strValue);
        }
        public static float GetFloat(string aKey)
        {
            return PlayerPrefs.GetFloat(aKey, 0);
        }
        public static double GetDouble(string aKey)
        {
            string strValue = PlayerPrefs.GetString(aKey, "0");
            return double.Parse(strValue);
        }
        public static string GetString(string aKey)
        {
            return PlayerPrefs.GetString(aKey);
        }
        public static PersistenceArray<T> GetArray<T>(string aKey)
        {
            return JsonUtility.FromJson<PersistenceArray<T>>(PlayerPrefs.GetString(aKey));
        }
        public static PersistenceList<T> GetList<T>(string aKey)
        {
            return JsonUtility.FromJson<PersistenceList<T>>(PlayerPrefs.GetString(aKey));
        }
        public static PersistenceDictionary<K, V> GetDictionary<K, V>(string aKey)
        {
            return JsonUtility.FromJson<PersistenceDictionary<K, V>>(PlayerPrefs.GetString(aKey));
        }
    }
}

