using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework
{
    public class TPreLoadPool : CSingleton<TPreLoadPool>
    {
        private Dictionary<string, Object> mDic = new Dictionary<string, Object>();

        public void LoadPrefabsToMemory(string a_strPath)
        {
            Object[] objs = AssetLoader.LoadAllAssets(a_strPath);
            for (int i = 0; i < objs.Length; i++)
            {
                if (this.mDic.ContainsKey(objs[i].name))
                {
                    Debug.LogError("PreLoadPool has the same name asset!!");
                    return;
                }
                this.mDic.Add(objs[i].name, objs[i]);
            }
        }

        public void LoadPrefabToMemory(string a_strPath)
        {
            Object obj = AssetLoader.LoadAsset(a_strPath);
            if (obj == null)
            {
                return;
            }
            if (this.mDic.ContainsKey(obj.name))
            {
                Debug.LogError("PreLoadPool has the same name asset!!");
                return;
            }
            this.mDic.Add(obj.name, obj);
        }

        public GameObject InitializeGameObject(string a_strName)
        {
            if (this.mDic.ContainsKey(a_strName) == false)
            {
                this.LoadPrefabToMemory(a_strName);
            }
            if (this.mDic.ContainsKey(a_strName) == false)
            {
                return null;
            }
            return GameObject.Instantiate(this.mDic[a_strName] as GameObject);
        }

        public GameObject InitializeEffect(string a_strName)
        {
            if (this.mDic.ContainsKey(a_strName) == false)
            {
                this.LoadPrefabToMemory(a_strName);
            }
            if (this.mDic.ContainsKey(a_strName) == false)
            {
                return null;
            }
            GameObject go = GameObject.Instantiate(this.mDic[a_strName] as GameObject);
            go.AddComponent<ParticleAutoDestroy>();
            return go;
        }

        public void ClearPool()
        {
            this.mDic.Clear();
        }
    }
}
