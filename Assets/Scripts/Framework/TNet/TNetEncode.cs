using System;
using System.Text;

namespace TFramework.TNet
{
    public class TNetEncode
    {
        //BOOL
        public static void SerializeBOOL(bool aData, byte[] aBuffer, ref int nOffset)
        {
            byte[] temp = BitConverter.GetBytes(aData);
            temp.CopyTo(aBuffer, nOffset);
            nOffset += 1;
        }
        public static bool DeserializeBOOL(byte[] aBuffer, ref int nOffset)
        {
            bool bRes = BitConverter.ToBoolean(aBuffer, nOffset);
            nOffset += 1;
            return bRes;
        }

        //N8
        public static void SerializeN8(byte aData, byte[] aBuffer, ref int nOffset)
        {
            aBuffer[nOffset] = aData;
            nOffset += 1;
        }
        public static byte DeserializeN8(byte[] aBuffer, ref int nOffset)
        {
            byte nRes = aBuffer[nOffset];
            nOffset += 1;
            return nRes;
        }

        //N16
        public static void SerializeN16(short aData, byte[] aBuffer, ref int nOffset)
        {
            byte[] temp = BitConverter.GetBytes(aData);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(temp, 0, 2);
            }
            temp.CopyTo(aBuffer, nOffset);
            nOffset += 2;
        }
        public static short DeserializeN16(byte[] aBuffer, ref int nOffset)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(aBuffer, nOffset, 2);
            }
            short nRes = BitConverter.ToInt16(aBuffer, nOffset);
            nOffset += 2;
            return nRes;
        }

        //N32
        public static void SerializeN32(int aData, byte[] aBuffer, ref int nOffset)
        {
            byte[] temp = BitConverter.GetBytes(aData);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(temp, 0, 4);
            }
            temp.CopyTo(aBuffer, nOffset);
            nOffset += 4;
        }
        public static int DeserializeN32(byte[] aBuffer, ref int nOffset)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(aBuffer, nOffset, 4);
            }
            int nRes = BitConverter.ToInt32(aBuffer, nOffset);
            nOffset += 4;
            return nRes;
        }

        //N64
        public static void SerializeN64(long aData, byte[] aBuffer, ref int nOffset)
        {
            byte[] temp = BitConverter.GetBytes(aData);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(temp, 0, 8);
            }
            temp.CopyTo(aBuffer, nOffset);
            nOffset += 8;
        }
        public static long DeserializeN64(byte[] aBuffer, ref int nOffset)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(aBuffer, nOffset, 8);
            }
            long nRes = BitConverter.ToInt64(aBuffer, nOffset);
            nOffset += 8;
            return nRes;
        }

        //U8
        public static void SerializeU8(byte aData, byte[] aBuffer, ref int nOffset)
        {
            aBuffer[nOffset] = aData;
            nOffset += 1;
        }
        public static byte DeserializeU8(byte[] aBuffer, ref int nOffset)
        {
            byte nRes = aBuffer[nOffset];
            nOffset += 1;
            return nRes;
        }

        //U16
        public static void SerializeU16(ushort aData, byte[] aBuffer, ref int nOffset)
        {
            byte[] temp = BitConverter.GetBytes(aData);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(temp, 0, 2);
            }
            temp.CopyTo(aBuffer, nOffset);
            nOffset += 2;
        }
        public static ushort DeserializeU16(byte[] aBuffer, ref int nOffset)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(aBuffer, nOffset, 2);
            }
            ushort nRes = BitConverter.ToUInt16(aBuffer, nOffset);
            nOffset += 2;
            return nRes;
        }

        //U32
        public static void SerializeU32(uint aData, byte[] aBuffer, ref int nOffset)
        {
            byte[] temp = BitConverter.GetBytes(aData);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(temp, 0, 4);
            }
            temp.CopyTo(aBuffer, nOffset);
            nOffset += 4;
        }
        public static uint DeserializeU32(byte[] aBuffer, ref int nOffset)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(aBuffer, nOffset, 4);
            }
            uint nRes = BitConverter.ToUInt32(aBuffer, nOffset);
            nOffset += 4;
            return nRes;
        }

        //U64
        public static void SerializeU64(ulong aData, byte[] aBuffer, ref int nOffset)
        {
            byte[] temp = BitConverter.GetBytes(aData);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(temp, 0, 8);
            }
            temp.CopyTo(aBuffer, nOffset);
            nOffset += 8;
        }
        public static ulong DeserializeU64(byte[] aBuffer, ref int nOffset)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(aBuffer, nOffset, 8);
            }
            ulong nRes = BitConverter.ToUInt64(aBuffer, nOffset);
            nOffset += 8;
            return nRes;
        }

        //F32
        public static void SerializeF32(float aData, byte[] aBuffer, ref int nOffset)
        {
            byte[] temp = BitConverter.GetBytes(aData);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(temp, 0, 4);
            }
            temp.CopyTo(aBuffer, nOffset);
            nOffset += 4;
        }
        public static float DeserializeF32(byte[] aBuffer, ref int nOffset)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(aBuffer, nOffset, 4);
            }
            float fRes = BitConverter.ToSingle(aBuffer, nOffset);
            nOffset += 4;
            return fRes;
        }

        //F64
        public static void SerializeF64(double aData, byte[] aBuffer, ref int nOffset)
        {
            byte[] temp = BitConverter.GetBytes(aData);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(temp, 0, 8);
            }
            temp.CopyTo(aBuffer, nOffset);
            nOffset += 8;
        }
        public static double DeserializeF64(byte[] aBuffer, ref int nOffset)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(aBuffer, nOffset, 8);
            }
            double fRes = BitConverter.ToDouble(aBuffer, nOffset);
            nOffset += 8;
            return fRes;
        }

        //STR
        public static void SerializeSTR(string aData, byte[] aBuffer, ref int nOffset)
        {
            if (aData == null) 
            {
                aData = string.Empty; 
            }
            byte[] strBytes = Encoding.UTF8.GetBytes(aData);
            int nLen = strBytes.Length;

            byte[] lengthBytes = BitConverter.GetBytes(nLen);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(lengthBytes, 0, 4);
            }
            lengthBytes.CopyTo(aBuffer, nOffset);
            nOffset += 4;
            
            strBytes.CopyTo(aBuffer, nOffset);
            nOffset += nLen;
        }
        public static string DeserializeSTR(byte[] aBuffer, ref int nOffset)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(aBuffer, nOffset, 4);
            }
            int nLen = BitConverter.ToInt32(aBuffer, nOffset);
            nOffset += 4;

            string rRes = Encoding.UTF8.GetString(aBuffer, nOffset, nLen);
            nOffset += nLen;
            return rRes;
        }
    }
}
