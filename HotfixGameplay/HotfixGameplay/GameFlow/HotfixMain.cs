using UnityEngine;
using TFramework;
using TFramework.TGUI;
using System.Collections;

namespace HotfixGameplay
{
    public class HotfixMain
    {
        public static void Init()
        {
            PreloadAssets();
            PreloadUI();
            TUIManager.GetSingleton().OpenUI("UIMain");
        }

        public static void Update()
        {

        }

        public static void Clear()
        {
            
        }

        //预加载各种资源
        private static void PreloadAssets()
        {
            //加载Assetbundle
            AssetLoader.LoadAB("rof_config");
            AssetLoader.LoadAB("ui");

            //图集
            //AtlasManager.GetSingleton().LoadAtlas("AtlasLogic", "Assets/Atlas/AtlasTown.spriteatlas");

            //配置表
            RofManager.GetSingleton().Initialize("rof_config");
        }

        //预加载UI
        private static void PreloadUI()
        {
            TUIManager.GetSingleton().RegisterUI("UIMain", "Assets/UI/UIMain.prefab");
            
        }

    }
}
