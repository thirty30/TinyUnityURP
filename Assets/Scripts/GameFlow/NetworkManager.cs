using UnityEngine;
using TFramework;
using TFramework.TNet;
using System.Net.Sockets;

public class NetworkManager : CSingleton<NetworkManager>
{
    private TNetTCPReactor mNet = new TNetTCPReactor();

    public bool Initialize()
    {
        this.mNet.SetCallback(this.OnConnect, this.OnDisconnect, this.OnReceive, this.OnException, this.GetPacketSize);
        return this.mNet.InitSocket();
    }

    public bool ConnectServer(string aAddress, int aPort)
    {
        return this.mNet.ConnectServer(aAddress, aPort);
    }

    public void Update()
    {
        this.mNet.Dispatch();
    }

    private void OnConnect()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixNetWork", "OnConnect", null, null);
    }

    private void OnDisconnect()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixNetWork", "OnDisconnect", null, null);
    }

    private void OnReceive(byte[] aBuffer)
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixNetWork", "OnReceive", null, aBuffer);
    }

    private void OnException(SocketException aException)
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke("HotfixGameplay.HotfixNetWork", "OnException", null, aException);
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

    public void SendData(byte[] aBuffer, int aSize)
    {
        this.mNet.WriteData(aBuffer, aSize);
    }

}
