using UnityEngine;

public interface IMazeModel
{
    bool Step();
    Vector2[] GetPoints();
}