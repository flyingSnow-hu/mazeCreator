using System;
using System.Collections.Generic;
using UnityEngine;

using URandom = UnityEngine.Random;


public class PrimSearchEngine:ISearchEngine
{
    private MazeViewBase maze;
    private LinkedList<CellModelBase> path;

    public PrimSearchEngine(MazeViewBase maze, CellModelBase start)
    {
        this.maze = maze;
        path = new LinkedList<CellModelBase>();
        path.AddLast(start);
        start.State = CellState.Connected;
    }

    public bool DoSearch()
    {
        if (path.Count == 0) return true;

        var crntNode = GetRandomNode();
        var crnt = crntNode.Value;
        CellModelBase[] neighbours = maze.GetNeighbours(crnt);
        Shuffle(neighbours);
        foreach (var neighbour in neighbours)
        {
            var cellState = neighbour.State;
            if(cellState == CellState.Unconnected)
            {
                neighbour.State = CellState.Connected;
                maze.AddWay(crnt, neighbour);
                path.AddLast(neighbour);
                return false;
            } 
        }

        // 周边已经没有空间了
        path.Remove(crntNode);

        return false;
    }

    public LinkedListNode<CellModelBase> GetRandomNode()
    {
        var ret = path.First;
        var retGrid = ret.Value;
        int retType = retGrid.GetWayCount();

        var crnt = ret.Next;
        var counts = new int[2];
        counts[retType] = 1;

        var weights = new int[2];
        int branch = 1;
        weights[0] = 0;
        weights[1] = 100 - branch;
        weights[2] = branch;

        var summed_weight = weights[retType];

        while(crnt != null)
        {
            var cell = crnt.Value;
            int branchType = Mathf.Clamp(cell.GetWayCount(), 0, weights.Length - 1);
            counts[branchType] += 1;

            bool bChange = false;
            if (branchType == retType)
            {
                if (URandom.Range(0, counts[branchType]) == 0)
                {
                    bChange = true;
                }
            }else if(counts[branchType] == 1)
            {
                var crntWeight = weights[branchType];
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
                retType = branchType;
                retGrid = cell;
            }

            crnt = crnt.Next;
        }
        return ret;
    }

    public void Shuffle(CellModelBase[] neighbours)
    {
        int n = neighbours.Length;
        while (n > 1) 
        {
            int k = URandom.Range(0, n);
            var temp = neighbours[n - 1];
            neighbours[n - 1] = neighbours[k];
            neighbours[k] = temp;
            n--;
        }
    }
}