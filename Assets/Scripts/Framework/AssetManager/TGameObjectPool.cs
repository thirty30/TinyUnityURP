using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework
{
    public class TGameObjectPool : MBSingleton<TGameObjectPool>
    {
        private Dictionary<string, LinkedList<GameObject>> mObjectPool;
        private TPreLoadPool mPreLoadPool;

        public void Init(TPreLoadPool aPool)
        {
            this.mPreLoadPool = aPool;
            this.mObjectPool = new Dictionary<string, LinkedList<GameObject>>();
        }

        public void Clear()
        {
            foreach (var pool in this.mObjectPool)
            {
                pool.Value.Clear();
            }
            this.mObjectPool.Clear();
        }

        public GameObject AllocGameObject(string aName)
        {
            GameObject obj = null;
            if (this.mObjectPool.ContainsKey(aName) == false)
            {
                obj = this.mPreLoadPool.InitializeGameObject(aName);
                obj.name = aName;
            }
            else
            {
                LinkedList<GameObject> pool = this.mObjectPool[aName];
                if (pool.Count > 0)
                {
                    obj = pool.Last.Value;
                    pool.RemoveLast();
                }
                else
                {
                    obj = this.mPreLoadPool.InitializeGameObject(aName);
                    obj.name = aName;
                }
            }
            obj.SetActive(true);
            return obj;
        }

        public void DestroyGameObject(GameObject aObj)
        {
            if (aObj == null) { return; }
            aObj.transform.SetParent(this.transform, false);
            aObj.SetActive(false);
            string strName = aObj.name;
            if (this.mObjectPool.ContainsKey(strName) == false)
            {
                LinkedList<GameObject> pool = new LinkedList<GameObject>();
                pool.AddLast(aObj);
                this.mObjectPool.Add(strName, pool);
            }
            else
            {
                LinkedList<GameObject> pool = this.mObjectPool[strName];
                pool.AddLast(aObj);
            }
        }
    }
}
