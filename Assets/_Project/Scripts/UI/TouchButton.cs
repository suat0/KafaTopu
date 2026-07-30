using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Ekran butonu. Unity'nin Button'i "tiklama" olayi verir; bize ise tusun
/// basili tutulup tutulmadigi lazim - hareket icin tek tiklama yetmez.
/// </summary>
public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
    }

    void OnDisable()
    {
        // Kontroller gizlenirken parmak basili kalmis gibi gorunmesin
        IsPressed = false;
    }
}
