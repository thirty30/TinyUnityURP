using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TNet;
namespace Game.Net
{
public class CommonBool : ITNetMessage
{
public bool Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeBOOL(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeBOOL(aBuffer, ref nOffset);

}
}
public class CommonN8 : ITNetMessage
{
public byte Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeN8(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeN8(aBuffer, ref nOffset);

}
}
public class CommonN16 : ITNetMessage
{
public short Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeN16(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeN16(aBuffer, ref nOffset);

}
}
public class CommonN32 : ITNetMessage
{
public int Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeN32(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeN32(aBuffer, ref nOffset);

}
}
public class CommonN64 : ITNetMessage
{
public long Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeN64(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeN64(aBuffer, ref nOffset);

}
}
public class CommonU8 : ITNetMessage
{
public byte Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeU8(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeU8(aBuffer, ref nOffset);

}
}
public class CommonU16 : ITNetMessage
{
public ushort Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeU16(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeU16(aBuffer, ref nOffset);

}
}
public class CommonU32 : ITNetMessage
{
public uint Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeU32(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeU32(aBuffer, ref nOffset);

}
}
public class CommonU64 : ITNetMessage
{
public ulong Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeU64(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeU64(aBuffer, ref nOffset);

}
}
public class CommonF32 : ITNetMessage
{
public float Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeF32(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeF32(aBuffer, ref nOffset);

}
}
public class CommonF64 : ITNetMessage
{
public double Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeF64(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeF64(aBuffer, ref nOffset);

}
}
public class CommonStr : ITNetMessage
{
public string Value;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeSTR(this.Value, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value = TNetEncode.DeserializeSTR(aBuffer, ref nOffset);

}
}
public class T1 : ITNetMessage
{
public int Value1;
public float Value2;
public string Value3;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeN32(this.Value1, aBuffer, ref nOffset);
TNetEncode.SerializeF32(this.Value2, aBuffer, ref nOffset);
TNetEncode.SerializeSTR(this.Value3, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value1 = TNetEncode.DeserializeN32(aBuffer, ref nOffset);
this.Value2 = TNetEncode.DeserializeF32(aBuffer, ref nOffset);
this.Value3 = TNetEncode.DeserializeSTR(aBuffer, ref nOffset);

}
}
public class T2 : ITNetMessage
{
public int Value1;
public float Value2;
public string Value3;
public int Value4;
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeN32(this.Value1, aBuffer, ref nOffset);
TNetEncode.SerializeF32(this.Value2, aBuffer, ref nOffset);
TNetEncode.SerializeSTR(this.Value3, aBuffer, ref nOffset);
TNetEncode.SerializeN32(this.Value4, aBuffer, ref nOffset);

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value1 = TNetEncode.DeserializeN32(aBuffer, ref nOffset);
this.Value2 = TNetEncode.DeserializeF32(aBuffer, ref nOffset);
this.Value3 = TNetEncode.DeserializeSTR(aBuffer, ref nOffset);
this.Value4 = TNetEncode.DeserializeN32(aBuffer, ref nOffset);

}
}
public class Test : ITNetMessage
{
public bool Value0;
public byte Value1;
public short Value2;
public int Value3;
public long Value4;
public byte Value5;
public ushort Value6;
public uint Value7;
public ulong Value8;
public float Value9;
public double Value10;
public string Value11;
public T1 Value12 = new T1();
public List<T2> Value13 = new List<T2>();
public List<int> Value14 = new List<int>();
public List<string> Value15 = new List<string>();
public void Serialize(byte[] aBuffer, int aSize, ref int nOffset)
{
TNetEncode.SerializeBOOL(this.Value0, aBuffer, ref nOffset);
TNetEncode.SerializeN8(this.Value1, aBuffer, ref nOffset);
TNetEncode.SerializeN16(this.Value2, aBuffer, ref nOffset);
TNetEncode.SerializeN32(this.Value3, aBuffer, ref nOffset);
TNetEncode.SerializeN64(this.Value4, aBuffer, ref nOffset);
TNetEncode.SerializeU8(this.Value5, aBuffer, ref nOffset);
TNetEncode.SerializeU16(this.Value6, aBuffer, ref nOffset);
TNetEncode.SerializeU32(this.Value7, aBuffer, ref nOffset);
TNetEncode.SerializeU64(this.Value8, aBuffer, ref nOffset);
TNetEncode.SerializeF32(this.Value9, aBuffer, ref nOffset);
TNetEncode.SerializeF64(this.Value10, aBuffer, ref nOffset);
TNetEncode.SerializeSTR(this.Value11, aBuffer, ref nOffset);
this.Value12.Serialize(aBuffer, aSize - nOffset, ref nOffset);
int nValue13Count = this.Value13.Count;
TNetEncode.SerializeN32(nValue13Count, aBuffer, ref nOffset);
for (int i = 0; i < nValue13Count; i++){this.Value13[i].Serialize(aBuffer, aSize - nOffset, ref nOffset);}
int nValue14Count = this.Value14.Count;
TNetEncode.SerializeN32(nValue14Count, aBuffer, ref nOffset);
for (int i = 0; i < nValue14Count; i++){TNetEncode.SerializeN32(this.Value14[i], aBuffer, ref nOffset);}
int nValue15Count = this.Value15.Count;
TNetEncode.SerializeN32(nValue15Count, aBuffer, ref nOffset);
for (int i = 0; i < nValue15Count; i++){TNetEncode.SerializeSTR(this.Value15[i], aBuffer, ref nOffset);}

}
public void Deserialize(byte[] aBuffer, int aSize, ref int nOffset)
{
this.Value0 = TNetEncode.DeserializeBOOL(aBuffer, ref nOffset);
this.Value1 = TNetEncode.DeserializeN8(aBuffer, ref nOffset);
this.Value2 = TNetEncode.DeserializeN16(aBuffer, ref nOffset);
this.Value3 = TNetEncode.DeserializeN32(aBuffer, ref nOffset);
this.Value4 = TNetEncode.DeserializeN64(aBuffer, ref nOffset);
this.Value5 = TNetEncode.DeserializeU8(aBuffer, ref nOffset);
this.Value6 = TNetEncode.DeserializeU16(aBuffer, ref nOffset);
this.Value7 = TNetEncode.DeserializeU32(aBuffer, ref nOffset);
this.Value8 = TNetEncode.DeserializeU64(aBuffer, ref nOffset);
this.Value9 = TNetEncode.DeserializeF32(aBuffer, ref nOffset);
this.Value10 = TNetEncode.DeserializeF64(aBuffer, ref nOffset);
this.Value11 = TNetEncode.DeserializeSTR(aBuffer, ref nOffset);
this.Value12.Deserialize(aBuffer, aSize - nOffset, ref nOffset);
int nValue13Count = TNetEncode.DeserializeN32(aBuffer, ref nOffset);
for(int i = 0; i < nValue13Count; i++)
{
T2 temp = new T2();temp.Deserialize(aBuffer, aSize - nOffset, ref nOffset);this.Value13.Add(temp);
}
int nValue14Count = TNetEncode.DeserializeN32(aBuffer, ref nOffset);
for(int i = 0; i < nValue14Count; i++)
{
this.Value14.Add(TNetEncode.DeserializeN32(aBuffer, ref nOffset));
}
int nValue15Count = TNetEncode.DeserializeN32(aBuffer, ref nOffset);
for(int i = 0; i < nValue15Count; i++)
{
this.Value15.Add(TNetEncode.DeserializeSTR(aBuffer, ref nOffset));
}

}
}
public class MessageID
{
public const int C2S_LOGIN = 1;
public const int S2C_LOGIN_RESP = 2;
}

}
