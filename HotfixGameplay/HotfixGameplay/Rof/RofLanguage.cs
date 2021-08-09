using System;
using System.Text;
using System.Collections.Generic;
namespace Rof
{
public class RofLanguageRow
{
public int ID { get; private set; }
public string Chinese { get; private set; }
public string English { get; private set; }
public int ReadBody(byte[] rData, int nOffset)
{
if (BitConverter.IsLittleEndian){Array.Reverse(rData, nOffset, 4);}
this.ID = (int)BitConverter.ToUInt32(rData, nOffset); nOffset += 4;
if (BitConverter.IsLittleEndian){Array.Reverse(rData, nOffset, 4);}
int nChineseLen = (int)BitConverter.ToUInt32(rData, nOffset); nOffset += 4;
this.Chinese = Encoding.UTF8.GetString(rData, nOffset, nChineseLen); nOffset += nChineseLen;
if (BitConverter.IsLittleEndian){Array.Reverse(rData, nOffset, 4);}
int nEnglishLen = (int)BitConverter.ToUInt32(rData, nOffset); nOffset += 4;
this.English = Encoding.UTF8.GetString(rData, nOffset, nEnglishLen); nOffset += nEnglishLen;
return nOffset;
}
}
public class RofLanguageTable
{
private int mColNum;
private int mRowNum;
private Dictionary<int, RofLanguageRow> mIDMap;
private Dictionary<int, int> mRowMap;
public int RowNum { get { return this.mRowNum; } }
public int ColNum { get { return this.mColNum; } }
public void Init(byte[] rTotalBuffer)
{
mIDMap = new Dictionary<int, RofLanguageRow>();
this.mRowMap = new Dictionary<int, int>();
int nOffset = 64;
if (BitConverter.IsLittleEndian) { Array.Reverse(rTotalBuffer, nOffset, 4); }
this.mRowNum = (int)BitConverter.ToUInt32(rTotalBuffer, nOffset); nOffset += 4;
if (BitConverter.IsLittleEndian) { Array.Reverse(rTotalBuffer, nOffset, 4); }
this.mColNum = (int)BitConverter.ToUInt32(rTotalBuffer, nOffset); nOffset += 4;
for (int i = 0; i < this.mColNum; i++)
{
int nNameLen = (int)rTotalBuffer[nOffset];
nOffset += 1 + nNameLen;
int nTypeLen = (int)rTotalBuffer[nOffset];
nOffset += 1 + nTypeLen;
}
for (int i = 0; i < this.mRowNum; i++)
{
if (BitConverter.IsLittleEndian) { Array.Reverse(rTotalBuffer, nOffset, 4); }
int nID = (int)BitConverter.ToUInt32(rTotalBuffer, nOffset);
if (BitConverter.IsLittleEndian) { Array.Reverse(rTotalBuffer, nOffset, 4); }
RofLanguageRow rModel = new RofLanguageRow();
nOffset = rModel.ReadBody(rTotalBuffer, nOffset);
this.mIDMap.Add(nID, rModel);
this.mRowMap.Add(i, nID);
}
}
public RofLanguageRow GetDataByID(int nID)
{
if (this.mIDMap.ContainsKey(nID) == false)
{
return null;
}
return this.mIDMap[nID];
}
public RofLanguageRow GetDataByRow(int nIndex)
	{
if (mRowMap.ContainsKey(nIndex) == false)
{
return null;
}
int nID = mRowMap[nIndex];
return mIDMap[nID];
}
}
}
