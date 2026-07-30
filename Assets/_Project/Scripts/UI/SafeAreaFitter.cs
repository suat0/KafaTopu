using UnityEngine;

/// <summary>
/// Centikli / yuvarlak kose ekranlarda UI'in kesilmesini onler.
/// Bagli oldugu RectTransform'u Screen.safeArea'ya oturtur.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform rect;
    private Rect appliedSafeArea;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        Apply();
    }

    void Update()
    {
        // Cihaz dondurulunce safe area degisir
        if (Screen.safeArea != appliedSafeArea) Apply();
    }

    private void Apply()
    {
        if (Screen.width <= 0 || Screen.height <= 0) return;

        Rect safeArea = Screen.safeArea;
        appliedSafeArea = safeArea;

        Vector2 min = safeArea.position;
        Vector2 max = safeArea.position + safeArea.size;

        min.x /= Screen.width;
        min.y /= Screen.height;
        max.x /= Screen.width;
        max.y /= Screen.height;

        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
