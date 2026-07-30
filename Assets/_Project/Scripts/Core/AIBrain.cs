using System;

/// <summary>
/// AI'in karar mantigi. Bilerek statik ve Unity'den bagimsiz: MonoBehaviour
/// olmadigi icin birim testi yazilabilir (bkz. docs/ROADMAP.md, Faz 8).
/// </summary>
public static class AIBrain
{
    /// <summary>
    /// Topun verilen yuksekliğe indigi andaki x'ini tahmin eder.
    /// Top o yuksekliğe hic inmiyorsa mevcut x'i doner.
    /// </summary>
    public static float PredictLandingX(float ballX, float ballY, float velocityX, float velocityY,
                                        float targetY, float gravity)
    {
        if (gravity <= 0f) return ballX;

        // ballY + vy*t - 0.5*g*t^2 = targetY  ->  ikinci dereceden denklem
        float discriminant = velocityY * velocityY + 2f * gravity * (ballY - targetY);
        if (discriminant < 0f) return ballX;

        // Buyuk kok: topun asagi inerken hedefi kestigi an
        float t = (velocityY + (float)Math.Sqrt(discriminant)) / gravity;
        if (t <= 0f) return ballX;

        return ballX + velocityX * t;
    }

    /// <summary>Hedefe dogru -1, 0 veya 1. Deadzone titremeyi onler.</summary>
    public static float DecideHorizontal(float selfX, float targetX, float deadzone)
    {
        float difference = targetX - selfX;
        if (Math.Abs(difference) <= deadzone) return 0f;

        return difference > 0f ? 1f : -1f;
    }

    public static bool DecideJump(float selfX, float selfY, float ballX, float ballY,
                                  float horizontalRange, float minHeightAbove)
    {
        bool closeEnough = Math.Abs(ballX - selfX) <= horizontalRange;
        bool highEnough = (ballY - selfY) >= minHeightAbove;

        return closeEnough && highEnough;
    }
}
