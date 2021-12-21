using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMazeView : MonoBehaviour
{
    [SerializeField] private GameObject Grid;
    [SerializeField] private Transform gridRoot;
    private List<Grid> grids;
    private SearchEngine se;
    private int columnCount = 15;
    private int rowCount = 15;

    private float lastTime = 99999999;

    private void Start()
    {
        grids = new List<Grid>(64);
        CreateGrids(columnCount, rowCount);
        se = new SearchEngine(columnCount, rowCount,
            0, 0, columnCount - 1, rowCount - 1,
            AddWall, AddWay, GetGridState, SetGridState);
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
        return GridState.Blocked;
    }

    private void AddWall(int x, int y, Bound wall)
    {
        if (x >= 0 && x < columnCount && y >= 0 && y < rowCount)
        {
            grids[y * columnCount + x].AddWall(wall);
        }
    }

    private void AddWay(int x, int y, Bound way)
    {
        if (x >= 0 && x < columnCount && y >= 0 && y < rowCount)
        {
            grids[y * columnCount + x].AddWay(way);
        }
    }

    public void OnNextClick()
    {
        lastTime = 0;
        // se.DoSearch();
    }

    private void Update()
    {
        if (Time.time > lastTime + 0.01f)
        {
            if (se.DoSearch())
            {
                lastTime = Time.time + 999999;
            }else{
                lastTime = Time.time;
            }
        }
    }
}
