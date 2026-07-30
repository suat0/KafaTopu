using UnityEngine;

public class KeyboardInputSource : MonoBehaviour, IInputSource
{
    [SerializeField] private KeyCode left = KeyCode.A;
    [SerializeField] private KeyCode right = KeyCode.D;
    [SerializeField] private KeyCode jump = KeyCode.W;

    public float Horizontal { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }

    public void Tick()
    {
        float axis = 0f;
        if (Input.GetKey(left)) axis -= 1f;
        if (Input.GetKey(right)) axis += 1f;

        Horizontal = axis;
        JumpPressed = Input.GetKeyDown(jump);
        JumpReleased = Input.GetKeyUp(jump);
    }
}
