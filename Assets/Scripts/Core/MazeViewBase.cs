using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MazeViewBase:MonoBehaviour
{
    [SerializeField] protected GameObject CellPrefab;
    [SerializeField] protected Transform gridRoot;
    protected ISearchEngine se;
    private float nextTime = 99999999;

    
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

    private void Reset()
    {        
        nextTime = 999999;        
        // se = new PrimSearchEngine(AddWay, cellModels[0]);
        CreateCells();
    }

    public abstract CellModelBase[] GetNeighbours(CellModelBase cell);

    public abstract void AddWay(CellModelBase cell1, CellModelBase cell2);

    protected abstract void CreateCells();

    private void Update()
    {
        if (Time.time > nextTime)
        {
            se.DoSearch();
            nextTime = Time.time + 0.01f;
        }
    }
}