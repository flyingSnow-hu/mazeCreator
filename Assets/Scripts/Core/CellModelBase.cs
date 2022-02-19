using System;

public enum CellState
{
    Unconnected = 0,
    Connected = 1,
}

public abstract class CellModelBase
{   
    public virtual CellState State { get; set;}
    public virtual bool IsSolution { get; set;}

    public abstract int GetWayCount();
}