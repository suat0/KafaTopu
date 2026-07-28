public enum Side
{
    Left = 0,
    Right = 1
}

public static class SideExtensions
{
    public static Side Opposite(this Side side)
    {
        return side == Side.Left ? Side.Right : Side.Left;
    }
}
