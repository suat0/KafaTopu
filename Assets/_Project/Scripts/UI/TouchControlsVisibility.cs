using UnityEngine;

public class TouchControlsVisibility : MonoBehaviour
{
    [SerializeField] private GameObject controlsRoot;

    [Tooltip("Editorde de goster - fareyle test edebilmek icin")]
    [SerializeField] private bool showInEditor = true;

    void Awake()
    {
        bool visible = Application.isMobilePlatform || (Application.isEditor && showInEditor);
        controlsRoot.SetActive(visible);
    }
}
