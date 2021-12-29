using System;
using System.Collections.Generic;
using UnityEngine;

using URandom = UnityEngine.Random;


public class PrimSearchEngine:ISearchEngine
{
    public class Accumulated
    {
        public float branchThreshold = 0;
        public float straightThreshold = 0;
    }

    private Vector2Int[] directions = new Vector2Int[]{
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
    };

    private int columns;
    private int rows;
    private Vector2Int start;
    private Vector2Int end;
    private int endX;
    private int endY;
    private Func<int, int, GridState> getGridState;
    private Action<int, int, GridState> setGridState;
    private Action<int, int, Bound> addWall;
    private Action<int, int, Bound> addWay;
    private LinkedList<Vector2Int> path;
    private bool hasReachEnd = false;

    // private Dictionary<Vector2Int, Accumulated> accumulated;

    public PrimSearchEngine(int columns, int rows, 
            int startX, int startY, int endX, int endY, 
            Action<int, int, Bound> addWay,
            Func<int, int, GridState> getGridState,
            Action<int, int, GridState> setGridState
            )
    {
        this.columns = columns;
        this.rows = rows;
        this.start = new Vector2Int(startX, startY);
        this.end = new Vector2Int(endX, endY);

        this.getGridState = getGridState;
        this.setGridState = setGridState;
        this.addWay = addWay;

        path = new LinkedList<Vector2Int>();
        int x = URandom.Range(0, columns), y = URandom.Range(0, rows);

        path.AddLast(new Vector2Int(x, y));
        setGridState(x, y, GridState.Connected);
    }

    public bool DoSearch()
    {
        if (path.Count == 0) return true;

        var crntNode = path.Last;
        if (hasReachEnd){
            crntNode = path.First;
            var selected = URandom.Range(0, path.Count);
            for (int i = 0; i < selected; i++)
            {
                crntNode = crntNode.Next;
            }
        }

        var crnt = crntNode.Value;
        ShuffleDirections();
        foreach (var direction in directions)
        {
            var next = crnt + direction;
            var gridState = getGridState(next.x, next.y);
            if(gridState == GridState.Unconnected)
            {
                setGridState(next.x, next.y, GridState.Connected);
                addWay(crnt.x, crnt.y, Directions.Direction2Bound(direction));
                addWay(next.x, next.y, Directions.Direction2Bound(-direction));
                path.AddLast(next);

                if (next.x == endX && next.y == endY)
                {
                    hasReachEnd = true;
                }

                return false;
            } 
        }

        // 周边已经没有空间了
        path.Remove(crntNode);

        return false;
    }

    // private Bound GetWallType(Vector2Int direction)
    // {
    //     if(direction.x > 0){
    //         return Bound.Right;
    //     }else if (direction.x < 0)
    //     {
    //         return Bound.Left;
    //     }else if(direction.y > 0)
    //     {
    //         return Bound.Top;
    //     }else if (direction.y < 0)
    //     {
    //         return Bound.Bottom;
    //     }
    //     return Bound.None;
    // }

    public void ShuffleDirections()
    {
        int n = directions.Length;
        while (n > 1) 
        {
            int k = URandom.Range(0, n);
            Vector2Int temp = directions[n - 1];
            directions[n - 1] = directions[k];
            directions[k] = temp;
            n--;
        }
    }
}