/// <summary>
/// PlayerController input'un nereden geldigini bilmez. Klavye, dokunmatik ve AI
/// ayni arayuzun arkasinda durur; boylece AI ayri bir kod dali olmaz ve mobil
/// destegi (Faz 5) yeni bir implementasyondan ibaret kalir.
/// </summary>
public interface IInputSource
{
    float Horizontal { get; }

    /// <summary>Bu karede ziplamaya basildi.</summary>
    bool JumpPressed { get; }

    /// <summary>Bu karede ziplama birakildi (kisa zipla icin gerekli).</summary>
    bool JumpReleased { get; }

    /// <summary>Her karede bir kez, PlayerController.Update icinden cagrilir.</summary>
    void Tick();
}
