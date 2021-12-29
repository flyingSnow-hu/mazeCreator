using UnityEngine;

public static class Directions
{  
    public static Bound Direction2Bound(Vector2Int direction)
    {
        if(direction.x > 0){
            return Bound.Right;
        }else if (direction.x < 0)
        {
            return Bound.Left;
        }else if(direction.y > 0)
        {
            return Bound.Top;
        }else if (direction.y < 0)
        {
            return Bound.Bottom;
        }
        return Bound.None;
    }
    
    public static Vector2Int Bound2Direction(Bound bound)
    {
        if((bound & Bound.Left) > 0){
            return Vector2Int.left;
        }else if ((bound & Bound.Right) > 0)
        {
            return Vector2Int.right;
        }else if ((bound & Bound.Top) > 0)
        {
            return Vector2Int.up;
        }else if ((bound & Bound.Bottom) > 0)
        {
            return Vector2Int.down;
        }
        return Vector2Int.zero;
    }
}