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
    public delegate int GetPacketSize(byte[] aBuffer, int aSize, int aIndex);

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
            return sizeof(int) + sizeof(int) + sizeof(byte);
        }

        public void Serialize(byte[] aBuffer, int aIndex)
        {
            TNetEncode.SerializeN32(this.MsgID, aBuffer, ref aIndex);
            TNetEncode.SerializeN32(this.BodySize, aBuffer, ref aIndex);
            TNetEncode.SerializeN8(this.IsCompressed, aBuffer, ref aIndex);
        }

        public void Deserialize(byte[] aBuffer, int aIndex)
        {
            this.MsgID = TNetEncode.DeserializeN32(aBuffer, ref aIndex);
            this.BodySize = TNetEncode.DeserializeN32(aBuffer, ref aIndex);
            this.IsCompressed = TNetEncode.DeserializeN8(aBuffer, ref aIndex);
        }
    }
}
