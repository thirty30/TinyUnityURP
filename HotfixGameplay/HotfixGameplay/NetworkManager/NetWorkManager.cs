using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using TFramework.TNet;
using UnityEngine;
using HotfixGameplay.Framework;

namespace HotfixGameplay
{
    public interface ITNetMessage
    {
        void Serialize(byte[] aBuffer, int aSize, ref int nOffset);
        void Deserialize(byte[] aBuffer, int aSize, ref int nOffset);
    }

    public class NetWorkManager
    {
        private TNetTCPReactor mNet = new TNetTCPReactor();

        public void Init()
        {
            mNet.SetCallback(this.OnConnect,
                this.OnDisconnect,
                this.OnReceive,
                this.OnException,
                this.GetPacketSize);
            mNet.ConnectServer("127.0.0.1", 18610);
        }

        public void Update()
        {
            this.mNet.Dispatch();
        }

        private void OnConnect()
        {
            Debug.Log("OnConnect");
        }

        private void OnDisconnect()
        {
            Debug.Log("OnDisconnect");
        }

        private void OnReceive(byte[] aBuffer)
        {
            Debug.Log("OnReceive");
            TNetMessageHead rMsgHead = new TNetMessageHead();
            rMsgHead.Deserialize(aBuffer, 0);

            Test t = new Test();
            int nOffset = rMsgHead.GetHeadSize();
            t.Deserialize(aBuffer, rMsgHead.BodySize, ref nOffset);
        }

        private void OnException(SocketException aException)
        {
            Debug.Log("OnException:" + aException.Message);
        }

        private int GetPacketSize(byte[] aBuffer, int aSize, int aIndex)
        {
            TNetMessageHead rMsgHead = new TNetMessageHead();
            int nMsgHeadLen = rMsgHead.GetHeadSize();
            if (aSize < nMsgHeadLen)
            {
                return 0;
            }
            rMsgHead.Deserialize(aBuffer, aIndex);
            return rMsgHead.BodySize + nMsgHeadLen;
        }

        private void SendMessageToServer(int aMsgID, ITNetMessage aMsg)
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
            this.mNet.WriteData(buffer, nHeadLen + rMsgHead.BodySize);
        }

        public void SendTest()
        {
            Test t = new Test();
            t.Value0 = true;
            t.Value1 = 108;
            t.Value2 = 322;
            t.Value3 = 23212321;
            t.Value4 = 5158497546841;
            t.Value5 = 255;
            t.Value6 = 65535;
            t.Value7 = 4135745215;
            t.Value8 = 5465464685435465;
            t.Value9 = 3.259689f;
            t.Value10 = 5465465.5469546f;
            t.Value11 = "niubi niubi niu  bii";
            t.Value12 = new T1();
            t.Value12.Value1 = 123123;
            t.Value12.Value2 = 3.22f;
            t.Value12.Value3 = "牛逼下班回家gogogogocccccc!!#4#$%^&*%&%&!!!!!!!!!!!!";
            t.Value13 = new List<T2>();
            T2 a = new T2();
            a.Value1 = 1;
            a.Value2 = 1;
            a.Value3 = "abc";
            a.Value4 = 11;
            T2 b = new T2();
            b.Value1 = 2;
            b.Value2 = 2;
            b.Value3 = "def";
            b.Value4 = 22;
            t.Value13.Add(a);
            t.Value13.Add(b);
            t.Value14 = new List<int>();
            t.Value14.Add(1);
            t.Value14.Add(2);
            t.Value14.Add(3);
            t.Value14.Add(3287923);
            t.Value15 = new List<string>();
            t.Value15.Add("123123");
            t.Value15.Add("sfesf");
            t.Value15.Add("ssfi89e890");
            this.SendMessageToServer(MessageID.C2S_LOGIN, t);
            //t.Value15.Add("fffffff");
            //this.SendMessageToServer(MessageID.C2S_LOGIN, t);
        }
    }
}
