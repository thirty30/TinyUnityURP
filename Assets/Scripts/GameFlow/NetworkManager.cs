using UnityEngine;
using TFramework;
using TFramework.TNet;
using System.Net.Sockets;
using ILRuntime.CLR.Method;

public class NetworkManager : CSingleton<NetworkManager>
{
    private TNetTCPReactor mNet = new TNetTCPReactor();
    private IMethod mMethodOnConnect;
    private IMethod mMethodOnDisconnect;
    private IMethod mMethodOnReceive;
    private IMethod mMethodOnException;

    private object[] mReceiveParm = new object[1];
    private object[] mExceptionParm = new object[1];

    public bool Initialize()
    {
        this.mNet.SetCallback(this.OnConnect, this.OnDisconnect, this.OnReceive, this.OnException, this.GetPacketSize);

        ILRuntime.Runtime.Enviorment.AppDomain rAppDomain = HotfixManager.GetSingleton().ILRuntimeApp;
        this.mMethodOnConnect = rAppDomain.LoadedTypes["HotfixGameplay.HotfixNetWork"].GetMethod("OnConnect", 0);
        this.mMethodOnDisconnect = rAppDomain.LoadedTypes["HotfixGameplay.HotfixNetWork"].GetMethod("OnDisconnect", 0);
        this.mMethodOnReceive = rAppDomain.LoadedTypes["HotfixGameplay.HotfixNetWork"].GetMethod("OnReceive", 1);
        this.mMethodOnException = rAppDomain.LoadedTypes["HotfixGameplay.HotfixNetWork"].GetMethod("OnException", 1);

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
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke(this.mMethodOnConnect, null, null);
    }

    private void OnDisconnect()
    {
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke(this.mMethodOnDisconnect, null, null);
    }

    private void OnReceive(byte[] aBuffer)
    {
        this.mReceiveParm[0] = aBuffer;
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke(this.mMethodOnReceive, null, this.mReceiveParm);
    }

    private void OnException(SocketException aException)
    {
        this.mExceptionParm[0] = aException;
        HotfixManager.GetSingleton().ILRuntimeApp.Invoke(this.mMethodOnException, null, this.mExceptionParm);
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
