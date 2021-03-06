using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class SquareMazeView : MazeViewBase
{
    protected List<SquareCellView> cells;
    [SerializeField] private int columnCount = 15;
    [SerializeField] private int rowCount = 15;

    private void Start()
    {
        cells = new List<SquareCellView>(64);
        Reset();
    }

    private void CreateGrids(int columns, int rows)
    {
        var halfWidth = columns / 2f;
        var halfHeight = rows / 2f;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var grid = GameObject.Instantiate<GameObject>(CellPrefab, new Vector3(x - halfWidth + 0.5f, y - halfHeight + 0.5f, 0), Quaternion.identity, gridRoot);
                var cellView = grid.GetComponent<SquareCellView>();
                cells.Add(cellView);
                cellView.SetPos(x, y);
            }
        }
    }

    protected override void CreateCells()
    {      
        var totalCount = columnCount * rowCount;
        if (cells != null)
        {
            int i = 0;
            for (i = 0; i < cells.Count; i++)
            {
                GameObject.Destroy(cells[i].gameObject);
            }
            cells.Clear();
            CreateGrids(columnCount, rowCount);
        }
        else{
            cells = new List<SquareCellView>(totalCount);
        }

        se = new PrimSearchEngine(this);
    }

    private int GetIndex(int x, int y)
    {
        return x + y * columnCount;
    }

    public override CellModelBase[] GetNeighbours(CellModelBase cell)
    {
        var sCell = (SquareCellModel)cell;
        var neighbours = new List<CellModelBase>(4);
        if(sCell.x > 0) neighbours.Add(cells[GetIndex(sCell.x - 1, sCell.y)].Model);
        if(sCell.x < columnCount - 1) neighbours.Add(cells[GetIndex(sCell.x + 1, sCell.y)].Model);
        if(sCell.y > 0) neighbours.Add(cells[GetIndex(sCell.x, sCell.y - 1)].Model);
        if(sCell.y < rowCount - 1) neighbours.Add(cells[GetIndex(sCell.x, sCell.y + 1)].Model);
        return neighbours.ToArray();
    }

    public override CellModelBase[] GetConnectedNeighbours(CellModelBase cell)
    {
        var sCell = (SquareCellModel)cell;
        var neighbours = new List<CellModelBase>(4);
        if(sCell.x > 0 && (sCell.Way & Direction.Left) != Direction.None) neighbours.Add(cells[GetIndex(sCell.x - 1, sCell.y)].Model);
        if(sCell.x < columnCount - 1 && (sCell.Way & Direction.Right) != Direction.None) neighbours.Add(cells[GetIndex(sCell.x + 1, sCell.y)].Model);
        if(sCell.y > 0 && (sCell.Way & Direction.Bottom) != Direction.None) neighbours.Add(cells[GetIndex(sCell.x, sCell.y - 1)].Model);
        if(sCell.y < rowCount - 1 && (sCell.Way & Direction.Top) != Direction.None) neighbours.Add(cells[GetIndex(sCell.x, sCell.y + 1)].Model);
        return neighbours.ToArray();
    }

    public override void AddWay(CellModelBase cell1, CellModelBase cell2)
    {
        var sCell1 = (SquareCellModel)cell1;
        var sCell2 = (SquareCellModel)cell2;
        if(sCell1.x < sCell2.x)
        {
            // 1 ?????????
            sCell1.AddWay(Direction.Right);
            sCell2.AddWay(Direction.Left);
        }else if(sCell1.x > sCell2.x)
        {
            // 1 ?????????
            sCell1.AddWay(Direction.Left);
            sCell2.AddWay(Direction.Right);
        }else if(sCell1.y < sCell2.y)
        {
            // 1 ??????
            sCell1.AddWay(Direction.Top);
            sCell2.AddWay(Direction.Bottom);
        }else if(sCell1.y > sCell2.y)
        {
            // 1 ??????
            sCell1.AddWay(Direction.Bottom);
            sCell2.AddWay(Direction.Top);
        }
        UpdateCell(sCell1);
        UpdateCell(sCell2);
    }

    private void UpdateCell(SquareCellModel cellModel)
    {
        cells[GetIndex(cellModel.x, cellModel.y)].SetWays(cellModel.Way);
    }

    public override CellModelBase GetRandomCell()
    {
        return cells[GetIndex(URandom.Range(0, columnCount), URandom.Range(0, rowCount))].Model;
    }

    public override CellModelBase GetStart()
    {
        return cells[0].Model;
    }

    public override CellModelBase GetEnd()
    {
        return cells[cells.Count - 1].Model;
    }

    public override void SetSolution(CellModelBase cellModel, bool isSolution)
    {
        var sCell = (SquareCellModel)cellModel;
        cells[GetIndex(sCell.x, sCell.y)].SetSolution(isSolution);
    }

    // public void CreateCycle()
    // {
    //     int[] branchGroup = new int[cells.Count];
    //     branchGroup[0] = 0;
    //     int branchIndex = 1;

    //     int[] distances = new int[cells.Count];
    //     distances[0] = 1;

    //     // ?????????????????????????????????
    //     LinkedList<Vector2Int> gridLinkedList = new LinkedList<Vector2Int>();
    //     gridLinkedList.AddLast(new Vector2Int(0,0));
    //     while(gridLinkedList.Count > 0)
    //     {
    //         var last = gridLinkedList.Last.Value;
    //         var lastIndex = last.x + last.y * columnCount;
    //         gridLinkedList.RemoveLast();

    //         var grid = cells[lastIndex];
    //         for (Bound way = Bound.Left; way <= Bound.Bottom; way=(Bound)((int)way << 1))
    //         {
    //             if ((grid.Ways & way) == Bound.None) continue;

    //             var direction = Directions.Bound2Direction(way);
    //             int newX = last.x + direction.x, newY = last.y + direction.y;
    //             var newIndex = newX + newY * columnCount;
    //             if (distances[newIndex] == 0){
    //                 // ????????????????????????????????????????????????0
    //                 distances[newIndex] = distances[lastIndex] + 1;
    //                 cells[newIndex].distance.text = (distances[lastIndex] + 1).ToString();
    //                 gridLinkedList.AddLast(new Vector2Int(newX, newY));
    //                 if (cells[newIndex].state == CellState.Solution)
    //                 {
    //                     // ???????????????0
    //                     branchGroup[newIndex] = 0;
    //                     cells[newIndex].branch.text = "0";
    //                 }else if (grid.state == CellState.Solution)
    //                 {
    //                     // ????????????????????????????????????????????????
    //                     branchGroup[newIndex] = branchIndex;
    //                     cells[newIndex].branch.text = branchIndex.ToString();
    //                     branchIndex++;
    //                 }else{
    //                     // ???????????????????????????
    //                     branchGroup[newIndex] = branchGroup[lastIndex];
    //                     cells[newIndex].branch.text = (branchGroup[lastIndex]).ToString();
    //                 }
    //             }
    //         }
    //     }

    //     void TryConnect(Vector2Int crnt, Bound direction1, Bound direction2, Bound direction3) {
    //         var index = crnt.x + crnt.y * columnCount;
    //         var score = distances[index];

    //         var target1 = crnt + Directions.Bound2Direction(direction1);
    //         var dScore1 = -1; 
    //         var index1 = target1.x + target1.y * columnCount;
    //         if(target1.x >= 0 && target1.x < columnCount && target1.y >= 0 && target1.y < rowCount && branchGroup[index1] == branchGroup[index] && Directions.IsSingle(cells[index1].Ways))
    //         { 
    //             dScore1 = Mathf.Abs(distances[index1] - score);
    //             if (Directions.IsSingle(cells[index1].Ways))
    //             {
    //                 dScore1 += 100;
    //             }
    //         }

    //         var target2 = crnt + Directions.Bound2Direction(direction2);
    //         var dScore2 = -1; 
    //         var index2 = target2.x + target2.y * columnCount;
    //         if(target2.x >= 0 && target2.x < columnCount && target2.y >= 0 && target2.y < rowCount && branchGroup[index2] == branchGroup[index] && Directions.IsSingle(cells[index2].Ways))
    //         {
    //             dScore2 = Mathf.Abs(distances[index2] - score);
    //             if (Directions.IsSingle(cells[index2].Ways))
    //             {
    //                 dScore2 += 100;
    //             }
    //         }

    //         var target3 = crnt + Directions.Bound2Direction(direction3);
    //         var dScore3 = -1; 
    //         var index3 = target3.x + target3.y * columnCount;
    //         if(target3.x >= 0 && target3.x < columnCount && target3.y >= 0 && target3.y < rowCount && branchGroup[index3] == branchGroup[index] && Directions.IsSingle(cells[index3].Ways))
    //         { 
    //             dScore3 = Mathf.Abs(distances[index3] - score);
    //             if (Directions.IsSingle(cells[index3].Ways))
    //             {
    //                 dScore3 += 100;
    //             }
    //         }

    //         Debug.Log($"{dScore1};{dScore2};{dScore3}");
    //         if(dScore1 >= 0 && dScore1 >= dScore2 && dScore1 >= dScore3)
    //         {
    //             Debug.Log($"1 ?????? {crnt.x},{crnt.y}{cells[index].Ways} ??? {target1.x},{target1.y}{cells[index1].Ways}");
    //             AddWay(crnt.x, crnt.y, direction1);
    //             AddWay(target1.x, target1.y, Directions.InverseBound(direction1));
    //             Debug.Log($"  ?????? {crnt.x},{crnt.y}{cells[index].Ways} {target1.x},{target1.y}{cells[index1].Ways}");
    //         }else if(dScore2 >= 0 && dScore2 >= dScore1 && dScore2 >= dScore3)
    //         {
    //             Debug.Log($"2 ?????? {crnt.x},{crnt.y}{cells[index].Ways} ??? {target2.x},{target2.y}{cells[index2].Ways}");
    //             AddWay(crnt.x, crnt.y, direction2);
    //             AddWay(target2.x, target2.y, Directions.InverseBound(direction2));
    //             Debug.Log($"  ?????? {crnt.x},{crnt.y}{cells[index].Ways} {target2.x},{target2.y}{cells[index2].Ways}");
    //         }else if(dScore3 >= 0 && dScore3 >= dScore1 && dScore3 >= dScore2)
    //         {
    //             Debug.Log($"3 ?????? {crnt.x},{crnt.y}{cells[index].Ways} ??? {target3.x},{target3.y}{cells[index3].Ways}");
    //             AddWay(crnt.x, crnt.y, direction3);
    //             AddWay(target3.x, target3.y, Directions.InverseBound(direction3));
    //             Debug.Log($"  ?????? {crnt.x},{crnt.y}{cells[index].Ways} {target3.x},{target3.y}{cells[index3].Ways}");
    //         }
    //     };

    //     // ???????????????????????????????????????????????????????????????????????????
    //     for (int y = 0; y < rowCount; y++)
    //     {
    //         for (int x = 0; x < columnCount; x++)
    //         {
    //             var index = x + columnCount * y;
    //             var crnt = new Vector2Int(x, y);
    //             if(cells[index].Ways == Bound.Left)
    //             {
    //                 TryConnect(crnt, Bound.Top, Bound.Right, Bound.Bottom);
    //             }else if(cells[index].Ways == Bound.Top)
    //             {
    //                 TryConnect(crnt, Bound.Left, Bound.Right, Bound.Bottom);
    //             }else if(cells[index].Ways == Bound.Right)
    //             {
    //                 TryConnect(crnt, Bound.Left, Bound.Top, Bound.Bottom);
    //             }else if(cells[index].Ways == Bound.Bottom)
    //             {
    //                 TryConnect(crnt, Bound.Left, Bound.Top, Bound.Right);
    //             }
    //         }
    //     }
    // }


}
