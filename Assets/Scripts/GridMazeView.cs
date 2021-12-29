using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMazeView : MonoBehaviour
{
    [SerializeField] private GameObject Grid;
    [SerializeField] private Transform gridRoot;
    [SerializeField] private int columnCount = 15;
    [SerializeField] private int rowCount = 15;

    private List<Grid> grids;
    private ISearchEngine se;
    private float nextTime = 99999999;

    private void Start()
    {
        grids = new List<Grid>(64);
        CreateGrids(columnCount, rowCount);
        se = new PrimSearchEngine(columnCount, rowCount,
            0, 0, columnCount - 1, rowCount - 1,
            AddWay, GetGridState, SetGridState);
    }

    private void CreateGrids(int columns, int rows)
    {
        var halfWidth = columns / 2f;
        var halfHeight = rows / 2f;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var grid = GameObject.Instantiate<GameObject>(Grid, new Vector3(x - halfWidth + 0.5f, y - halfHeight + 0.5f, 0), Quaternion.identity, gridRoot);
                grids.Add(grid.GetComponent<Grid>());
            }
        }
    }

    private void SetGridState(int x, int y, GridState state)
    {
        if (x >= 0 && x < columnCount && y >= 0 && y < rowCount)
        {
            grids[y * columnCount + x].state = state;
        }
    }

    private GridState GetGridState(int x, int y)
    {
        if (x >= 0 && x < columnCount && y >= 0 && y < rowCount)
        {
            return grids[y * columnCount + x].state;
        }
        return GridState.Connected;
    }

    private void AddWay(int x, int y, Bound way)
    {
        if (x >= 0 && x < columnCount && y >= 0 && y < rowCount)
        {
            grids[y * columnCount + x].AddWay(way);
        }
    }

    public void OnResetClick()
    {
        Reset();
    }

    public void OnStepClick()
    {
        se.DoSearch();
        nextTime = 999999;
    }

    public void OnEndClick()
    {
        nextTime = Time.time + 0.5f;
    }

    public void OnSolveClick()
    {
        // x, y, direction
        LinkedList<Vector3Int> gridLinkedList = new LinkedList<Vector3Int>();
        gridLinkedList.AddLast(new Vector3Int(0,0,1));
        while(gridLinkedList.Count > 0)
        {
            var last = gridLinkedList.Last.Value;
            if(last.x == columnCount - 1 && last.y == rowCount - 1) return;

            var grid = grids[last.x + last.y * columnCount];
            var way = (Bound)last.z;
            while(way <= Bound.Bottom)
            {
                if ((grid.Ways & way) != Bound.None) 
                {
                    var direction = Directions.Bound2Direction(way);
                    int newX = last.x + direction.x, newY = last.y + direction.y;
                    if (grids[newX + newY * columnCount].state == GridState.Connected)
                    {
                        last.z = (int)way; gridLinkedList.Last.Value = last;
                        gridLinkedList.AddLast(new Vector3Int(newX, newY, 1));
                        grids[newX + newY * columnCount].state = GridState.Solution;
                        goto OuterLoop;
                    }
                }
                way = (Bound)((int)way << 1);
            }

            // 回溯
            gridLinkedList.RemoveLast();
            grid.state = GridState.NotSolution;

            OuterLoop:;
        }
    }

    private void Reset()
    {        
        int i = 0;
        for (i = 0; i < grids.Count; i++)
        {
            GameObject.Destroy(grids[i].gameObject);
        }

        var totalCount = columnCount * rowCount;
        grids = new List<Grid>(totalCount);
        CreateGrids(columnCount, rowCount);
        se = new PrimSearchEngine(columnCount, rowCount,
            0, 0, columnCount - 1, rowCount - 1,
            AddWay, GetGridState, SetGridState);
        nextTime = 999999;
    }

    private void Update()
    {
        if (Time.time > nextTime)
        {
            se.DoSearch();
            nextTime = Time.time + 0.01f;
        }
    }
}
