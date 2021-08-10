using System;
using System.Collections.Generic;
using System.Net.Sockets;
using TFramework.TNet;
using UnityEngine;

namespace HotfixGameplay
{
    public interface ITNetMessage
    {
        void Serialize(byte[] aBuffer, int aSize, ref int nOffset);
        void Deserialize(byte[] aBuffer, int aSize, ref int nOffset);
    }

    public class HotfixNetWork
    {
        public static void OnConnect()
        {

        }

        public static void OnDisconnect()
        {

        }

        public static void OnReceive(byte[] aBuffer)
        {
            TNetMessageHead rMsgHead = new TNetMessageHead();
            rMsgHead.Deserialize(aBuffer, 0);

            int nOffset = rMsgHead.GetHeadSize();
            HotfixNetworkHandler.GetSingleton().ReceiveMessage(rMsgHead.MsgID, aBuffer, rMsgHead.BodySize, nOffset);
        }

        public static void OnException(SocketException aException)
        {
            Debug.Log("HotfixNetwork Exception: " + aException.Message);
        }

        public static void SendMessageToServer(int aMsgID, ITNetMessage aMsg)
        {
            int nBodyOffset = 0;
            byte[] body = new byte[1024];
            aMsg.Serialize(body, 1024, ref nBodyOffset);

            TNetMessageHead rMsgHead = new TNetMessageHead();
            rMsgHead.MsgID = aMsgID;
            rMsgHead.BodySize = nBodyOffset;
            rMsgHead.IsCompressed = 0;
            int nHeadLen = rMsgHead.GetHeadSize();

            byte[] buffer = new byte[1024];
            rMsgHead.Serialize(buffer, 0);

            Array.Copy(body, 0, buffer, nHeadLen, rMsgHead.BodySize);

            NetworkManager.GetSingleton().SendData(buffer, nHeadLen + rMsgHead.BodySize);
        }
    }
}
