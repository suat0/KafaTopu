using System;

/// <summary>
/// Mac suresini tutar. Bilerek MonoBehaviour degil: Unity'ye bagimli olmadigi
/// icin birim testi yazilabilir (bkz. docs/ROADMAP.md, Faz 8).
/// </summary>
public class MatchTimer
{
    public float Duration { get; }
    public float Remaining { get; private set; }
    public bool IsFinished => Remaining <= 0f;

    public MatchTimer(float duration)
    {
        if (duration <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), "Mac suresi pozitif olmali.");
        }

        Duration = duration;
        Remaining = duration;
    }

    public void Reset()
    {
        Remaining = Duration;
    }

    /// <summary>
    /// Sureyi azaltir. Sadece surenin bittigi cagride true doner, sonrakilerde false.
    /// </summary>
    public bool Tick(float deltaTime)
    {
        if (deltaTime < 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime), "Zaman geriye akmaz.");
        }

        if (IsFinished) return false;

        Remaining -= deltaTime;

        if (Remaining <= 0f)
        {
            Remaining = 0f;
            return true;
        }

        return false;
    }
}
