using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework.TGUI
{
    public class TUIRef : MonoBehaviour
    {
        public List<GameObject> RefList = new List<GameObject>();

        public T GetRefComponent<T>(int a_idx)
        {
            if (a_idx >= this.RefList.Count || a_idx < 0)
            {
                return default(T);
            }
            return this.RefList[a_idx].GetComponent<T>();
        }

        public GameObject GetRef(int a_idx)
        {
            if (a_idx >= this.RefList.Count || a_idx < 0)
            {
                return null;
            }
            return this.RefList[a_idx];
        }
    }
}



