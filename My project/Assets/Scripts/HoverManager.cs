using UnityEngine;

public class HoverManager : MonoBehaviour
{
    private HoverHighlight _currentHovered;
    public GameObject tooltip;


    private Camera _camera;
    private float radius = 2f;

    void Awake()
    {
        _camera = Camera.main;
    }
    void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.SphereCast(ray, radius, out RaycastHit hit))
        {
            // Debug.Log("Hit: " + hit.transform.gameObject.name);
            var highlight = hit.collider.GetComponent<HoverHighlight>();

            if (highlight != _currentHovered)
            {
                _currentHovered?.OnHoverExit();
                _currentHovered = highlight;
                _currentHovered?.OnHoverEnter(tooltip);
                Debug.Log("Now hovering: " + (highlight != null ? highlight.name : "nothing"));
            }
        }
        else
        {
            _currentHovered?.OnHoverExit();
            _currentHovered = null;
        }
    }
}