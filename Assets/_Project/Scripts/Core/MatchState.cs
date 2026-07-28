public enum MatchState
{
    /// <summary>Oyuncular yerinde, geri sayim isliyor.</summary>
    KickOff = 0,

    /// <summary>Mac akiyor, sure isliyor, input acik.</summary>
    Playing = 1,

    /// <summary>Gol atildi, kisa kutlama; ardindan KickOff.</summary>
    GoalScored = 2,

    /// <summary>Sure bitti, sonuc ekrani.</summary>
    MatchEnd = 3
}
