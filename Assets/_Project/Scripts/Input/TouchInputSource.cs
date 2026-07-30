using UnityEngine;

public class TouchInputSource : MonoBehaviour, IInputSource
{
    [SerializeField] private TouchButton leftButton;
    [SerializeField] private TouchButton rightButton;
    [SerializeField] private TouchButton jumpButton;

    public float Horizontal { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }

    private bool jumpHeldLastFrame;

    public void Tick()
    {
        float axis = 0f;
        if (leftButton != null && leftButton.IsPressed) axis -= 1f;
        if (rightButton != null && rightButton.IsPressed) axis += 1f;

        Horizontal = axis;

        bool held = jumpButton != null && jumpButton.IsPressed;

        // Klavyedeki GetKeyDown/GetKeyUp ile ayni kenarlari uret
        JumpPressed = held && !jumpHeldLastFrame;
        JumpReleased = !held && jumpHeldLastFrame;
        jumpHeldLastFrame = held;
    }
}
