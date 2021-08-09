namespace Rof
{
public class NNKV
{
public int Key { get; private set; }
public double Value { get; private set; }
public NNKV(int aKey, double aValue)
{
this.Key = aKey;
this.Value = aValue;
}
}
}
