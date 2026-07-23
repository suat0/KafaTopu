using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    void Update()
    {

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(horizontalInput * speed * Time.deltaTime, 0, 0);
    }
}