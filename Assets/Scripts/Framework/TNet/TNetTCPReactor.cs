using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System;

namespace TFramework.TNet
{
    public class TNetTCPReactor : ITNetReactor
    {
        private Socket mSocket;
        private byte[] mReceiveBuffer;
        private int mReceiveLen;
        private byte[] mSendBuffer;
        private int mSendLen;
        private Thread mNetWorker;
        private object mLockObj;
        private List<byte[]> mMessageObjs;
        private List<byte[]> mReadyToDeal;

        //call back
        private OnConnect mCallbackConnect;
        private OnDisconnect mCallbackDisconnect;
        private OnReceive mCallbackReceive;
        private OnException mCallbackException;
        private GetPacketSize mCallbackGetPacketSize;

        public TNetTCPReactor()
        {
            this.mReceiveBuffer = new byte[TnetDefine.ReceiveBufferLen];
            this.mReceiveLen = 0;
            this.mSendBuffer = new byte[TnetDefine.SendBufferLen];
            this.mSendLen = 0;
            this.mNetWorker = new Thread(NetWorkerLoop);
            this.mLockObj = new object();
            this.mMessageObjs = new List<byte[]>();
            this.mReadyToDeal = new List<byte[]>();

            this.mCallbackConnect = null;
            this.mCallbackDisconnect = null;
            this.mCallbackReceive = null;
            this.mCallbackException = null;
            this.mCallbackGetPacketSize = null;
        }

        public void SetCallback(OnConnect aCallbackConnect,
            OnDisconnect aCallbackDisconnect,
            OnReceive aCallbackReceive,
            OnException aCallbackException,
            GetPacketSize aCallbackGetPacketSize)
        {
            this.mCallbackConnect = aCallbackConnect;
            this.mCallbackDisconnect = aCallbackDisconnect;
            this.mCallbackReceive = aCallbackReceive;
            this.mCallbackException = aCallbackException;
            this.mCallbackGetPacketSize = aCallbackGetPacketSize;
        }

        public bool InitSocket()
        {
            try
            {
                this.mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (SocketException ex)
            {
                this.mCallbackException?.Invoke(ex);
                return false;
            }
            return true;
        }

        public bool ConnectServer(string aIP, int aPort)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(aIP), aPort);
                this.mSocket.Connect(ep);
                this.mCallbackConnect?.Invoke();
                this.mNetWorker.Start(this);
            }
            catch (SocketException ex)
            {
                this.mCallbackException?.Invoke(ex);
                return false;
            }
            return true;
        }

        public void Dispatch()
        {
            //处理消息
            this.ReceiveData();

            //发送数据
            this.SendData();
        }

        private static void NetWorkerLoop(object aObj)
        {
            TNetTCPReactor reactor = aObj as TNetTCPReactor;
            while (true)
            {
                //接收网络I/O数据
                if (reactor.ReadData() == false)
                {
                    return;
                }
            }
        }

        public bool ReadData()
        {
            if (this.mSocket.Connected == false)
            {
                return false;
            }

            try
            {
                int nLeftLen = this.mReceiveBuffer.Length - this.mReceiveLen;
                int nRecvLen = this.mSocket.Receive(this.mReceiveBuffer, this.mReceiveLen, nLeftLen, SocketFlags.None);
                this.mReceiveLen += nRecvLen;

                if (nRecvLen == 0 && this.mReceiveLen == this.mReceiveBuffer.Length)
                {
                    int nNewLen = this.mReceiveBuffer.Length * 2;
                    byte[] tempBuffer = new byte[nNewLen];
                    this.mReceiveBuffer.CopyTo(tempBuffer, 0);
                    this.mReceiveBuffer = tempBuffer;
                    return true;
                }

                int nOffset = 0;
                while (true)
                {
                    if (this.mReceiveLen == 0)
                    {
                        break;
                    }

                    int nPacketSize = 0;
                    if (this.mCallbackGetPacketSize == null)
                    {
                        nPacketSize = this.mReceiveLen;
                    }
                    else
                    {
                        nPacketSize = this.mCallbackGetPacketSize.Invoke(this.mReceiveBuffer, this.mReceiveLen, nOffset);
                    }

                    if (nPacketSize > 0 && nPacketSize <= this.mReceiveLen)
                    {
                        byte[] msg = new byte[nPacketSize];
                        Array.Copy(this.mReceiveBuffer, nOffset, msg, 0, nPacketSize);
                        nOffset += nPacketSize;
                        this.mReceiveLen -= nPacketSize;

                        lock (this.mLockObj)
                        {
                            this.mMessageObjs.Add(msg);
                        }
                        continue;
                    }

                    if (nOffset > 0)
                    {
                        byte[] tempBuffer = new byte[this.mReceiveLen];
                        Array.Copy(this.mReceiveBuffer, nOffset, tempBuffer, 0, this.mReceiveLen);
                        Array.Copy(tempBuffer, 0, this.mReceiveBuffer, 0, this.mReceiveLen);
                    }
                    break;
                }
            }
            catch (SocketException ex)
            {
                this.mSocket.Close();
                this.mCallbackException?.Invoke(ex);
                this.mCallbackDisconnect?.Invoke();
                return false;
            }
            return true;
        }

        private void ReceiveData()
        {
            if (this.mMessageObjs.Count <= 0)
            {
                return;
            }

            lock(this.mLockObj)
            {
                foreach (byte[] msg in this.mMessageObjs)
                {
                    this.mReadyToDeal.Add(msg);
                }
                this.mMessageObjs.Clear();
            }

            foreach (byte[] msg in this.mReadyToDeal)
            {
                this.mCallbackReceive?.Invoke(msg);
            }
            this.mReadyToDeal.Clear();
        }

        public void WriteData(byte[] aData, int aSize)
        {
            int nCurLen = this.mSendBuffer.Length;
            int nLeftSize = nCurLen - this.mSendLen;
            if (aSize > nLeftSize)
            {
                int nRatio = 2;
                nLeftSize = nCurLen * nRatio - this.mSendLen;
                while (aSize > nLeftSize)
                {
                    nRatio *= 2;
                    nLeftSize = nCurLen * nRatio - this.mSendLen;
                }
                int nNewLen = nCurLen * nRatio;
                byte[] tempBuffer = new byte[nNewLen];
                Array.Copy(this.mSendBuffer, tempBuffer, this.mSendLen);
                this.mSendBuffer = tempBuffer;
            }
            Array.Copy(aData, 0, this.mSendBuffer, this.mSendLen, aSize);
            this.mSendLen += aSize;
        }

        private void SendData()
        {
            if (this.mSendLen <= 0 || this.mSocket.Connected == false)
            {
                return;
            }

            try
            {
                this.mSocket.Send(this.mSendBuffer, this.mSendLen, SocketFlags.None);
                this.mSendLen = 0;
            }
            catch (SocketException ex)
            {
                this.mSocket.Close();
                this.mCallbackException?.Invoke(ex);
                this.mCallbackDisconnect?.Invoke();
            }
        }
    }
}
