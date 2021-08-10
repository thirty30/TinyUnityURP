using HotfixGameplay.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixGameplay
{
    public class HotfixNetworkHandler : HotfixSingleton<HotfixNetworkHandler>
    {
        private delegate void MessageHandler(byte[] aBuffer, int aSize, int aIndex);

        private Dictionary<int, MessageHandler> mHandlers = new Dictionary<int, MessageHandler>();

        public void ReceiveMessage(int aMsgID, byte[] aBuffer, int aSize, int aIndex)
        {
            if (this.mHandlers.ContainsKey(aMsgID) == false)
            {
                Debug.LogError("Cannot find the message handler! msgid: " + aMsgID.ToString());
                return;
            }

            this.mHandlers[aMsgID](aBuffer, aSize, aIndex);
        }

        public void InitMessageHandler()
        {
            //注册消息回调函数
            this.mHandlers.Add(MessageID.S2C_LOGIN_RESP, this.HandlerLogin);    //登录消息
        }

        private void HandlerLogin(byte[] aBuffer, int aSize, int aIndex)
        {
            Debug.Log("Login! The message size: " + aSize.ToString());
            Test t = new Test();
            t.Deserialize(aBuffer, aSize, ref aIndex);

            Main.GetSingleton().EnterGame();
        }
    }
}


