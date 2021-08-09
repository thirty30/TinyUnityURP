using UnityEngine;
using TFramework;
using TFramework.TGUI;

namespace HotfixGameplay
{
    public class HotfixMain
    {
        private static NetWorkManager NetMgr;

        public static void Init()
        {
            //初始化资源
            InitAssets();

            //加载图集
            LoadAtlas();

            //初始化UI
            InitUI();

            //初始化网络
            InitNet();

            TUIManager.GetSingleton().OpenUI("UITest");
        }

        public static void Update()
        {
            //NetMgr.Update();
            TUIManager.GetSingleton().UpdateUI();
        }

        public static void Clear()
        {
            
        }

        private static void InitNet()
        {
            //NetMgr = new NetWorkManager();
            //NetMgr.Init();
            //NetMgr.SendTest();
        }

        private static void InitAssets()
        {
            //加载Assetbundle
            AssetLoader.Init(EAssetLoadType.ASSET_BUNDLE);
            AssetLoader.LoadAB("rof_config");
            AssetLoader.LoadAB("ui");

            //配置表
            RofManager.GetSingleton().Initialize("rof_config");
        }

        private static void LoadAtlas()
        {
            //AtlasManager.GetSingleton().LoadAtlas("AtlasLogic", "Assets/Atlas/AtlasTown.spriteatlas");
        }

        private static void InitUI()
        {
            TUIManager.GetSingleton().Initialize();
            TUIManager.GetSingleton().RegisterUI("UITest", "Assets/UI/UITest.prefab");
            
        }
    }
}
