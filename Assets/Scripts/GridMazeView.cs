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
            AddWay, GetWay, GetGridState, SetGridState);
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

    private Bound GetWay(int x, int y)
    {
        if (x >= 0 && x < columnCount && y >= 0 && y < rowCount)
        {
            return grids[y * columnCount + x].Ways;
        }
        return Bound.None;
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

    public void CreateCycle()
    {
        int[] branchGroup = new int[grids.Count];
        branchGroup[0] = 0;
        int branchIndex = 1;

        int[] distances = new int[grids.Count];
        distances[0] = 1;

        // 标记每个格子的最短距离
        LinkedList<Vector2Int> gridLinkedList = new LinkedList<Vector2Int>();
        gridLinkedList.AddLast(new Vector2Int(0,0));
        while(gridLinkedList.Count > 0)
        {
            var last = gridLinkedList.Last.Value;
            var lastIndex = last.x + last.y * columnCount;
            gridLinkedList.RemoveLast();

            var grid = grids[lastIndex];
            for (Bound way = Bound.Left; way <= Bound.Bottom; way=(Bound)((int)way << 1))
            {
                if ((grid.Ways & way) == Bound.None) continue;

                var direction = Directions.Bound2Direction(way);
                int newX = last.x + direction.x, newY = last.y + direction.y;
                var newIndex = newX + newY * columnCount;
                if (distances[newIndex] == 0){
                    // 发现一个没有访问过的格子，距离是0
                    distances[newIndex] = distances[lastIndex] + 1;
                    grids[newIndex].distance.text = (distances[lastIndex] + 1).ToString();
                    gridLinkedList.AddLast(new Vector2Int(newX, newY));
                    if (grids[newIndex].state == GridState.Solution)
                    {
                        // 主线编组是0
                        branchGroup[newIndex] = 0;
                        grids[newIndex].branch.text = "0";
                    }else if (grid.state == GridState.Solution)
                    {
                        // 从主线分出去的格子启用新岔路编组
                        branchGroup[newIndex] = branchIndex;
                        grids[newIndex].branch.text = branchIndex.ToString();
                        branchIndex++;
                    }else{
                        // 否则沿用上一个编组
                        branchGroup[newIndex] = branchGroup[lastIndex];
                        grids[newIndex].branch.text = (branchGroup[lastIndex]).ToString();
                    }
                }
            }
        }

        void TryConnect(Vector2Int crnt, Bound direction1, Bound direction2, Bound direction3) {
            var index = crnt.x + crnt.y * columnCount;
            var score = distances[index];

            var target1 = crnt + Directions.Bound2Direction(direction1);
            var dScore1 = -1; 
            var index1 = target1.x + target1.y * columnCount;
            if(target1.x >= 0 && target1.x < columnCount && target1.y >= 0 && target1.y < rowCount && branchGroup[index1] == branchGroup[index] && Directions.IsSingle(grids[index1].Ways))
            { 
                dScore1 = Mathf.Abs(distances[index1] - score);
                if (Directions.IsSingle(grids[index1].Ways))
                {
                    dScore1 += 100;
                }
            }

            var target2 = crnt + Directions.Bound2Direction(direction2);
            var dScore2 = -1; 
            var index2 = target2.x + target2.y * columnCount;
            if(target2.x >= 0 && target2.x < columnCount && target2.y >= 0 && target2.y < rowCount && branchGroup[index2] == branchGroup[index] && Directions.IsSingle(grids[index2].Ways))
            {
                dScore2 = Mathf.Abs(distances[index2] - score);
                if (Directions.IsSingle(grids[index2].Ways))
                {
                    dScore2 += 100;
                }
            }

            var target3 = crnt + Directions.Bound2Direction(direction3);
            var dScore3 = -1; 
            var index3 = target3.x + target3.y * columnCount;
            if(target3.x >= 0 && target3.x < columnCount && target3.y >= 0 && target3.y < rowCount && branchGroup[index3] == branchGroup[index] && Directions.IsSingle(grids[index3].Ways))
            { 
                dScore3 = Mathf.Abs(distances[index3] - score);
                if (Directions.IsSingle(grids[index3].Ways))
                {
                    dScore3 += 100;
                }
            }

            Debug.Log($"{dScore1};{dScore2};{dScore3}");
            if(dScore1 >= 0 && dScore1 >= dScore2 && dScore1 >= dScore3)
            {
                Debug.Log($"1 连接 {crnt.x},{crnt.y}{grids[index].Ways} 和 {target1.x},{target1.y}{grids[index1].Ways}");
                AddWay(crnt.x, crnt.y, direction1);
                AddWay(target1.x, target1.y, Directions.InverseBound(direction1));
                Debug.Log($"  结果 {crnt.x},{crnt.y}{grids[index].Ways} {target1.x},{target1.y}{grids[index1].Ways}");
            }else if(dScore2 >= 0 && dScore2 >= dScore1 && dScore2 >= dScore3)
            {
                Debug.Log($"2 连接 {crnt.x},{crnt.y}{grids[index].Ways} 和 {target2.x},{target2.y}{grids[index2].Ways}");
                AddWay(crnt.x, crnt.y, direction2);
                AddWay(target2.x, target2.y, Directions.InverseBound(direction2));
                Debug.Log($"  结果 {crnt.x},{crnt.y}{grids[index].Ways} {target2.x},{target2.y}{grids[index2].Ways}");
            }else if(dScore3 >= 0 && dScore3 >= dScore1 && dScore3 >= dScore2)
            {
                Debug.Log($"3 连接 {crnt.x},{crnt.y}{grids[index].Ways} 和 {target3.x},{target3.y}{grids[index3].Ways}");
                AddWay(crnt.x, crnt.y, direction3);
                AddWay(target3.x, target3.y, Directions.InverseBound(direction3));
                Debug.Log($"  结果 {crnt.x},{crnt.y}{grids[index].Ways} {target3.x},{target3.y}{grids[index3].Ways}");
            }
        };

        // 把每个死路和最近的分数相差最大且非主线的格子连起来
        for (int y = 0; y < rowCount; y++)
        {
            for (int x = 0; x < columnCount; x++)
            {
                var index = x + columnCount * y;
                var crnt = new Vector2Int(x, y);
                if(grids[index].Ways == Bound.Left)
                {
                    TryConnect(crnt, Bound.Top, Bound.Right, Bound.Bottom);
                }else if(grids[index].Ways == Bound.Top)
                {
                    TryConnect(crnt, Bound.Left, Bound.Right, Bound.Bottom);
                }else if(grids[index].Ways == Bound.Right)
                {
                    TryConnect(crnt, Bound.Left, Bound.Top, Bound.Bottom);
                }else if(grids[index].Ways == Bound.Bottom)
                {
                    TryConnect(crnt, Bound.Left, Bound.Top, Bound.Right);
                }
            }
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
            AddWay, GetWay, GetGridState, SetGridState);
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
