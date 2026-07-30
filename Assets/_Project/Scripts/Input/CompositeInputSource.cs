using UnityEngine;

/// <summary>
/// Birden fazla input kaynagini birlestirir. Ayni build hem masaustunde hem
/// mobilde calissin diye: masaustunde dokunmatik butonlara basilmaz, mobilde
/// klavye tuslarina - hangisi aktifse o gecerli olur.
///
/// Listedeki kaynaklarin bileseni kapali (enabled = false) olmali; onlari bu
/// sinif dogrudan Tick() ile suruyor, boylece PlayerController'in "ilk etkin
/// IInputSource" secimi yanlislikla onlara denk gelmiyor.
/// </summary>
public class CompositeInputSource : MonoBehaviour, IInputSource
{
    [Tooltip("IInputSource uygulayan bilesenler")]
    [SerializeField] private MonoBehaviour[] sources;

    public float Horizontal { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }

    public void Tick()
    {
        float axis = 0f;
        bool pressed = false;
        bool released = false;

        foreach (MonoBehaviour behaviour in sources)
        {
            if (!(behaviour is IInputSource source)) continue;

            source.Tick();

            // Hangi kaynak daha guclu bir yon veriyorsa o kazanir
            if (Mathf.Abs(source.Horizontal) > Mathf.Abs(axis)) axis = source.Horizontal;

            pressed |= source.JumpPressed;
            released |= source.JumpReleased;
        }

        Horizontal = axis;
        JumpPressed = pressed;
        JumpReleased = released;
    }
}
