public struct CellId
{
    private int value;

    public static implicit operator CellId(int v)
    {
        return new CellId {value=v};
    }

    public static implicit operator int(CellId v)
    {
        return v.value;
    }
}