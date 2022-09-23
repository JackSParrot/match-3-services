using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MatchWidth : MonoBehaviour
{
    [SerializeField]
    float _sceneWidth = 10;
    [SerializeField]
    Vector2 _sizeLimits = new Vector2(5, 20);
    Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        UpdateSize();
    }

    void UpdateSize()
    {
        float unitsPerPixel = _sceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        _camera.orthographicSize = Mathf.Clamp(desiredHalfHeight, _sizeLimits.x, _sizeLimits.y);
    }

#if UNITY_EDITOR
    [SerializeField]
    bool _simulate = false;
    void Update()
    {
        if (Application.isEditor && _simulate)
        {
            UpdateSize();
        }
    }
#endif
}