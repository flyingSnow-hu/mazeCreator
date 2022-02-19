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
        nextTime = Time.time + 999999;
    }

    public abstract CellModelBase GetRandomCell();

    public void OnEndClick()
    {
        nextTime = Time.time + 0.5f;
    }

    protected void Reset()
    {        
        nextTime = Time.time + 999999;        
        CreateCells();
    }

    public abstract CellModelBase[] GetNeighbours(CellModelBase cell);
    public abstract CellModelBase[] GetConnectedNeighbours(CellModelBase cell);
    public abstract CellModelBase GetStart();
    public abstract CellModelBase GetEnd();

    public abstract void AddWay(CellModelBase cell1, CellModelBase cell2);
    public abstract void SetSolution(CellModelBase cellModel, bool isSolution);

    protected abstract void CreateCells();

    private void Update()
    {
        if (Time.time > nextTime)
        {
            if (se.DoSearch())
            {
                nextTime = Time.time + 999999;
            }else{
                nextTime = Time.time + 0.01f;
            }
        }
    }

    public void OnSolveClick()
    {
        LinkedList<CellModelBase> gridLinkedList = new LinkedList<CellModelBase>();
        Dictionary<CellModelBase, CellModelBase> followed = new Dictionary<CellModelBase, CellModelBase>();
        
        var start = GetStart();
        gridLinkedList.AddLast(start);
        while(gridLinkedList.Count > 0)
        {
            var top = gridLinkedList.Last.Value;
            gridLinkedList.RemoveLast();
            if(top == GetEnd()) 
            {
                // 找到了解法
                var pt = top;
                while(pt != start)
                {
                    SetSolution(pt, true);
                    pt = followed[pt];
                }
                SetSolution(start, true);
                return;
            }
            
            foreach (var neighbour in GetConnectedNeighbours(top))
            {
                if (!followed.ContainsKey(neighbour))
                // {
                //     followed[neighbour] = top;
                // }else
                {
                    gridLinkedList.AddLast(neighbour);
                    followed.Add(neighbour, top);
                }
            }
        }
    }
}