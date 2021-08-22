using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework.TGUI
{
    public class TUI3DRoot : MBSingleton<TUI3DRoot>
    {
        public Camera UICamera;
        public Canvas UICanvas;

        public void AddChild(GameObject obj)
        {
            obj.transform.SetParent(this.UICanvas.transform, false);
        }

        public GameObject AddChild(string assetPath)
        {
            GameObject prefab = AssetLoader.LoadAsset<GameObject>(assetPath);
            GameObject ui = GameObject.Instantiate(prefab);
            this.AddChild(ui);
            return ui;
        }

        public void Clear()
        {
            for (int i = 0; i < this.UICanvas.transform.childCount; i++)
            {
                GameObject.Destroy(this.UICanvas.transform.GetChild(i).gameObject);
            }
        }
    }
}

