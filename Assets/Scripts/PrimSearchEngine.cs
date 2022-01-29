using System;
using System.Collections.Generic;
using UnityEngine;

using URandom = UnityEngine.Random;


public class PrimSearchEngine:ISearchEngine
{
    // public class Accumulated
    // {
    //     public float branchThreshold = 0;
    //     public float straightThreshold = 0;
    // }

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
    private Func<int, int, Bound> getWay;
    private LinkedList<Vector2Int> path;

    public PrimSearchEngine(int columns, int rows, 
            int startX, int startY, int endX, int endY, 
            Action<int, int, Bound> addWay,
            Func<int, int, Bound> getWay,
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
        this.getWay = getWay;

        path = new LinkedList<Vector2Int>();
        int x = URandom.Range(0, columns), y = URandom.Range(0, rows);
        // int x = startX, y = startY;

        path.AddLast(new Vector2Int(x, y));
        setGridState(x, y, GridState.Connected);
    }

    public bool DoSearch()
    {
        if (path.Count == 0) return true;

        var crntNode = GetRandomNode();
        var crnt = crntNode.Value;
        ShuffleDirections();
        // CentripetalDirections(crnt);
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

                return false;
            } 
        }

        // 周边已经没有空间了
        path.Remove(crntNode);

        return false;
    }

    public LinkedListNode<Vector2Int> GetRandomNode()
    {
        var ret = path.First;
        var retGrid = ret.Value;
        int retType = (int)getWay(retGrid.x, retGrid.y);

        var crnt = ret.Next;
        var counts = new int[16];
        counts[retType] = 1;

        var weights = new int[16];        
        int branch = 1;
        weights[1] = weights[2] = weights[4] = weights[8] = 100 - branch;
        weights[7] = weights[11] = weights[13] = weights[14] = branch;
        for (int i = 0; i < 16; i++)
        {
            if (weights[i] == 0) weights[i] = branch;
        }

        var summed_weight = weights[retType];

        while(crnt != null)
        {
            var grid = crnt.Value;
            int type = (int)getWay(grid.x, grid.y);
            counts[type] += 1;

            bool bChange = false;
            if (type == retType)
            {
                if (URandom.Range(0, counts[type]) == 0)
                {
                    bChange = true;
                }
            }else if(counts[type] == 1)
            {
                var crntWeight = weights[type];
                var oldWeight = summed_weight;
                summed_weight += crntWeight; 
                if (URandom.Range(0, summed_weight) < crntWeight)
                {
                    bChange = true;
                }
            }

            if (bChange)
            {
                ret = crnt;
                retType = type;
                retGrid = grid;
            }

            crnt = crnt.Next;
        }
        return ret;
    }

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

    public void CentripetalDirections(Vector2Int pos)
    {
        Vector2Int direction = new Vector2Int(columns << 2, rows <<2) - pos;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // 横向优先
            if(direction.x < 0)
            {
                directions[0] = Vector2Int.left;
                directions[3] = Vector2Int.right;
            }else
            {
                directions[0] = Vector2Int.right;
                directions[3] = Vector2Int.left;
            }

            if(direction.y < 0)
            {
                directions[1] = Vector2Int.down;
                directions[2] = Vector2Int.up;
            }else
            {
                directions[1] = Vector2Int.up;
                directions[2] = Vector2Int.down;
            }
        }else
        {
            // 纵向优先
            if(direction.y < 0)
            {
                directions[0] = Vector2Int.down;
                directions[3] = Vector2Int.up;
            }else
            {
                directions[0] = Vector2Int.up;
                directions[3] = Vector2Int.down;
            }

            if(direction.x < 0)
            {
                directions[1] = Vector2Int.left;
                directions[2] = Vector2Int.right;
            }else
            {
                directions[1] = Vector2Int.right;
                directions[2] = Vector2Int.left;
            }
        }
    }
}