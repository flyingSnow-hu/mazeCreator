using System;

public enum CellState
{
    Unconnected = 0,
    Connected = 1,
    Solution = 2,
    NotSolution = 3,
}

public abstract class CellModelBase
{   
    public virtual CellState State { get; set;}

    public abstract int GetWayCount();
}