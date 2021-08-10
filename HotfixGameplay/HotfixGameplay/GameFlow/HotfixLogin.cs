using TFramework;
using TFramework.TGUI;

namespace HotfixGameplay
{
    public class HotfixLogin
    {
        public static void Init()
        {
            HotfixNetworkHandler.GetSingleton().InitMessageHandler();

            AssetLoader.LoadAB("ui_login");
            TUIManager.GetSingleton().RegisterUI("UILogin", "Assets/UI/UILogin.prefab");
            TUIManager.GetSingleton().OpenUI("UILogin");
        }

        public static void Update()
        {

        }

        public static void Clear()
        {

        }
    }
}


