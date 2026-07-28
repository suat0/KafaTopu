/// <summary>
/// Iki tarafin golunu tutar. MatchTimer gibi bu da saf C#: test edilebilir olsun diye.
/// Eski ScoreData ScriptableObject'inin yerini alir - SO'da runtime durumu tutmak
/// Play Mode bitince degerin bellekte kalmasina yol aciyordu.
/// </summary>
public class MatchScore
{
    public int Left { get; private set; }
    public int Right { get; private set; }

    public bool IsDraw => Left == Right;

    /// <summary>Beraberlikte anlamsizdir; once IsDraw kontrol edilmeli.</summary>
    public Side Winner => Left > Right ? Side.Left : Side.Right;

    public void Add(Side side)
    {
        if (side == Side.Left) Left++;
        else Right++;
    }

    public int Get(Side side)
    {
        return side == Side.Left ? Left : Right;
    }

    public void Reset()
    {
        Left = 0;
        Right = 0;
    }
}
