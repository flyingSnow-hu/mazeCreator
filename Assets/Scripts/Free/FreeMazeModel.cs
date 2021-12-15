using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using URandom = UnityEngine.Random;

public class FreeMazeModel : IMazeModel
{
    List<Vector2> points;

    public FreeMazeModel()
    {        
        var count = 80;
        points = new List<Vector2>(count + 2);
        points.Add(Vector2.zero);
        points.Add(Vector2.one);
        for (int i = 0; i < count; i++)
        {
            points.Add(new Vector2(URandom.Range(0,1f), URandom.Range(0,1f)));
        }
    }

    public Vector2[] GetPoints()
    {
        return points.ToArray();
    }
    public bool Step()
    {
        return true;
    }
}
