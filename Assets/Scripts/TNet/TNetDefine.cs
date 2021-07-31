using System.Net.Sockets;

namespace TNet
{
    public static class TnetDefine
    {
        public static int ReceiveBufferLen = 1024 * 1024;
        public static int SendBufferLen = 1024 * 1024;
    }

    public delegate void OnConnect();
    public delegate void OnDisconnect();
    public delegate void OnReceive(byte[] aBuffer);
    public delegate void OnException(SocketException aException);
    public delegate int GetPacketSize(byte[] aBuffer, int aSize);

    public interface ITNetMessage
    {
        void Serialize(byte[] aBuffer, int aSize, ref int nOffset);
        void Deserialize(byte[] aBuffer, int aSize, ref int nOffset);
    }

    public interface ITNetReactor
    {

    }

    public class TNetMessageHead
    {
        public int MsgID;
        public int BodySize;
        public byte IsCompressed;

        public int GetHeadSize()
        {
            return 9;
        }

        public int Serialize(byte[] aBuffer)
        {
            int nOffset = 0;
            TNetEncode.SerializeN32(this.MsgID, aBuffer, ref nOffset);
            TNetEncode.SerializeN32(this.BodySize, aBuffer, ref nOffset);
            TNetEncode.SerializeN8(this.IsCompressed, aBuffer, ref nOffset);
            return nOffset;
        }

        public int Deserialize(byte[] aBuffer)
        {
            int nOffset = 0;
            this.MsgID = TNetEncode.DeserializeN32(aBuffer, ref nOffset);
            this.BodySize = TNetEncode.DeserializeN32(aBuffer, ref nOffset);
            this.IsCompressed = TNetEncode.DeserializeN8(aBuffer, ref nOffset);
            return nOffset;
        }
    }
}
