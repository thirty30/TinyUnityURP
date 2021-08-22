using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework.TGUI
{
    public class TUIManager : CSingleton<TUIManager>
    {
        private Dictionary<string, GameObject> mUIDic;
        private List<TUIBasePage> mInitializedList;
        private List<TUIBasePage> mReadyToAddList;
        private List<TUIBasePage> mReadyToDelList;

        public void Initialize()
        {
            this.mUIDic = new Dictionary<string, GameObject>();
            this.mInitializedList = new List<TUIBasePage>();
            this.mReadyToAddList = new List<TUIBasePage>();
            this.mReadyToDelList = new List<TUIBasePage>();
        }

        public bool RegisterUI(string aKey, string aDir)
        {
            if (this.mUIDic.ContainsKey(aKey) == true)
            {
                Debug.LogError("The key of registered UI was existed!");
                return false;
            }

            GameObject prefab = AssetLoader.LoadAsset<GameObject>(aDir);
            if (prefab == null)
            {
                Debug.LogError("Can't find the UI prefab!");
                return false;
            }
            this.mUIDic.Add(aKey, prefab);
            return true;
        }

        public void UnRegisterUI(string aKey)
        {
            if (this.mUIDic.ContainsKey(aKey) == false)
            {
                return;
            }
            this.mUIDic.Remove(aKey);
        }

        public void Update()
        {
            foreach (TUIBasePage uiPage in this.mReadyToAddList)
            {
                this.mInitializedList.Add(uiPage);
            }
            this.mReadyToAddList.Clear();

            foreach (TUIBasePage uiPage in this.mReadyToDelList)
            {
                this.mInitializedList.Remove(uiPage);
                GameObject.Destroy(uiPage.gameObject);
            }
            this.mReadyToDelList.Clear();

            foreach (TUIBasePage uiPage in this.mInitializedList)
            {
                if (uiPage.gameObject.activeSelf == true)
                {
                    uiPage.OnUpdate();
                }
            }
        }

        public void OpenUI(string aUIName, params object[] parms)
        {
            if (this.mUIDic.ContainsKey(aUIName) == false)
            {
                Debug.LogError("UI is not existed!");
                return;
            }

            GameObject prefab = this.mUIDic[aUIName];
            GameObject UIObj = GameObject.Instantiate(prefab);
            TUIBasePage uiPage = UIObj.GetComponent<TUIBasePage>();
            EUILayer layerType = uiPage.UILayer;
            switch (layerType)
            {
                case EUILayer.GLOBLE:
                    UIObj.transform.SetParent(TUIRoot.GetSingleton().GlobalUIRoot.transform, false);
                    this.SortUILayer(TUIRoot.GetSingleton().GlobalUIRoot);
                    break;
                case EUILayer.NORMAL:
                    UIObj.transform.SetParent(TUIRoot.GetSingleton().NormalUIRoot.transform, false);
                    this.SortUILayer(TUIRoot.GetSingleton().NormalUIRoot);
                    break;
                case EUILayer.POPUP:
                    UIObj.transform.SetParent(TUIRoot.GetSingleton().PopUIRoot.transform, false);
                    this.SortUILayer(TUIRoot.GetSingleton().PopUIRoot);
                    break;
                default:
                    return;
            }
            uiPage.UIName = aUIName;
            uiPage.OnInitialize(parms);
            uiPage.OnShow();
            this.mReadyToAddList.Add(uiPage);
        }

        private void SortUILayer(GameObject aLayerNode)
        {
            TUIBasePage[] pages = aLayerNode.GetComponentsInChildren<TUIBasePage>(true);
            for (int i = 0; i < pages.Length - 1; i++)
            {
                for (int j = i + 1; j < pages.Length; j++)
                {
                    if (pages[i].UIOrder > pages[j].UIOrder)
                    {
                        TUIBasePage temp = pages[i];
                        pages[i] = pages[j];
                        pages[j] = temp;
                    }
                }
            }
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i].transform.SetSiblingIndex(i);
            }
        }

        public void CloseUI(string aUIName)
        {
            foreach (TUIBasePage uiPage in this.mInitializedList)
            {
                if (uiPage.UIName == aUIName)
                {
                    this.CloseUI(uiPage);
                }
            }
        }

        public void CloseUI(TUIBasePage aUIObj)
        {
            aUIObj.OnDestroy();
            this.mReadyToDelList.Add(aUIObj);
        }

        public bool IsUIInitialized(string aUIName)
        {
            foreach (TUIBasePage uiPage in this.mInitializedList)
            {
                if (uiPage.UIName == aUIName)
                {
                    return true;
                }
            }
            return false;
        }

        public void ShowUI(string aUIName)
        {
            foreach (TUIBasePage uiPage in this.mInitializedList)
            {
                if (uiPage.UIName == aUIName && uiPage.gameObject.activeSelf == false)
                {
                    uiPage.OnShow();
                    uiPage.gameObject.SetActive(true);
                }
            }
        }

        public void HideUI(string aUIName)
        {
            foreach (TUIBasePage uiPage in this.mInitializedList)
            {
                if (uiPage.UIName == aUIName && uiPage.gameObject.activeSelf == true)
                {
                    uiPage.OnHide();
                    uiPage.gameObject.SetActive(false);
                }
            }
        }

        public TUIBasePage GetUIObject(string aUIName)
        {
            foreach (TUIBasePage uiPage in this.mInitializedList)
            {
                if (uiPage.UIName == aUIName)
                {
                    return uiPage;
                }
            }

            foreach (TUIBasePage uiPage in this.mReadyToAddList)
            {
                if (uiPage.UIName == aUIName)
                {
                    return uiPage;
                }
            }

            return null;
        }
    }
}
