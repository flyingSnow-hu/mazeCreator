using System;
using UnityEngine;

public static class Directions
{  
    public static Direction Direction2Bound(Vector2Int direction)
    {
        if(direction.x > 0){
            return Direction.Right;
        }else if (direction.x < 0)
        {
            return Direction.Left;
        }else if(direction.y > 0)
        {
            return Direction.Top;
        }else if (direction.y < 0)
        {
            return Direction.Bottom;
        }
        return Direction.None;
    }
    
    public static Vector2Int Bound2Direction(Direction bound)
    {
        if((bound & Direction.Left) > 0){
            return Vector2Int.left;
        }else if ((bound & Direction.Right) > 0)
        {
            return Vector2Int.right;
        }else if ((bound & Direction.Top) > 0)
        {
            return Vector2Int.up;
        }else if ((bound & Direction.Bottom) > 0)
        {
            return Vector2Int.down;
        }
        return Vector2Int.zero;
    }
    
    public static Direction InverseBound(Direction bound)
    {
        switch(bound)
        {
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Top:
                return Direction.Bottom;
            case Direction.Bottom:
                return Direction.Top;
        }
        return Direction.None;
    }

    internal static bool IsSingle(Direction way)
    {
        return way == Direction.Left || way == Direction.Top || way == Direction.Right || way == Direction.Bottom;
    }
}