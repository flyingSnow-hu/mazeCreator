public class SquareCellModel : CellModelBase
{
    public int x { get; private set; }
    public int y { get; private set; }
    public Direction Way { get; private set; } = Direction.None;


    public SquareCellModel(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void AddWay(Direction way)
    {
        Way |= way;
    }

    public override int GetWayCount()
    {
        switch(Way)
        {

            case Direction.Left:
            case Direction.Top:
            case Direction.Right:
            case Direction.Bottom:
                return 1;            
            
            case Direction.Left | Direction.Top | Direction.Right:
            case Direction.Top | Direction.Right| Direction.Bottom:
            case Direction.Right | Direction.Bottom| Direction.Left:
            case Direction.Bottom | Direction.Left| Direction.Top:
                return 3;

            case Direction.Left | Direction.Top | Direction.Right | Direction.Bottom:
                return 4;
                
            case Direction.None:
                return 0;

            default:
                return 2;
        }
    }
}