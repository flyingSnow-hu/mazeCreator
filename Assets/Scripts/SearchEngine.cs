using System;
using System.Collections.Generic;
using UnityEngine;


public class SearchEngine
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

    private Dictionary<Vector2Int, Accumulated> accumulated;

    public SearchEngine(int columns, int rows, 
            int startX, int startY, int endX, int endY, 
            Action<int, int, Bound> addWall,
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
        this.addWall = addWall;
        this.addWay = addWay;

        path = new LinkedList<Vector2Int>();
        path.AddLast(new Vector2Int(startX, startY));
        setGridState(startX, startY, GridState.Way);

        accumulated = new Dictionary<Vector2Int, Accumulated>(columns * rows);
    }

    public void SetProbabilities(Vector2Int pos, Accumulated a)
    {
        if(accumulated.ContainsKey(pos))
        {
            accumulated[pos] = a;
        }else
        {
            accumulated.Add(pos, a);
        }
    }

    public Accumulated GetProbabilities(Vector2Int pos)
    {
        if(!accumulated.ContainsKey(pos))
        {
            accumulated.Add(pos, new Accumulated());
        }
        return accumulated[pos];
    }

    public bool DoSearch()
    {
        if (path.Count == 0) return true;

        var crnt = path.Last.Value;
        Accumulated probabilities = GetProbabilities(crnt);
        ShuffleDirections();
        foreach (var direction in directions)
        {
            var next = crnt + direction;
            var gridState = getGridState(next.x, next.y);
            if(gridState == GridState.Unchecked)
            {
                var nextProb = GetProbabilities(next);
                setGridState(next.x, next.y, GridState.Way);
                addWay(crnt.x, crnt.y, GetWallType(direction));
                addWay(next.x, next.y, GetWallType(-direction));
                // 积累分岔概率，随着路线伸长而增加，在分岔的位置清零
                // 积累转向概率，随着路线伸长而增加，在转向的位置清零
                if(UnityEngine.Random.value < probabilities.branchThreshold)
                {
                    // 倾向于分叉
                    path.AddFirst(next);
                    SetProbabilities(next, new Accumulated());
                }else
                {
                    // 倾向于长路线
                    path.AddLast(next);
                    nextProb.branchThreshold = GetProbabilities(crnt).branchThreshold + 0.1f;
                }
                return false;
            } 
        }

        // 需要回溯
        setGridState(crnt.x, crnt.y, GridState.Blocked);
        path.RemoveLast();

        return false;
    }

    private Bound GetWallType(Vector2Int direction)
    {
        if(direction.x > 0){
            return Bound.Right;
        }else if (direction.x < 0)
        {
            return Bound.Left;
        }else if(direction.y > 0)
        {
            return Bound.Top;
        }else if (direction.y < 0)
        {
            return Bound.Bottom;
        }
        return Bound.None;
    }

    public void ShuffleDirections()
    {
        int n = directions.Length;
        while (n > 1) 
        {
            int k = UnityEngine.Random.Range(0, n);
            Vector2Int temp = directions[n - 1];
            directions[n - 1] = directions[k];
            directions[k] = temp;
            n--;
        }
    }
}