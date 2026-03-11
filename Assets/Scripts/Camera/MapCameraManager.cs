using UnityEngine;

public class MapCameraManager : MonoBehaviour
{
    [Header("Настройки")]
    public float freeMoveSpeed = 40f; 
    public KeyCode mapKey = KeyCode.M;
    public float normalSize = 9f;
    public float mapViewingSize = 25f; 
    public float zoomSpeed = 5f;

    [Header("Границы")]
    public float minX = -100f; public float maxX = 100f;
    public float minY = -100f; public float maxY = 100f;

    private CameraControl camControl;
    private Camera mainCamera;
    private bool isMapActive = false;

    void Start()
    {
        camControl = GetComponent<CameraControl>();
        mainCamera = GetComponent<Camera>();
        if (mainCamera != null && mainCamera.orthographic) normalSize = mainCamera.orthographicSize;
    }

    void Update()
    {
        if (Input.GetKeyDown(mapKey)) ToggleMap();
        if (mainCamera == null) return;

        float targetSize = isMapActive ? mapViewingSize : normalSize;
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize, Time.unscaledDeltaTime * zoomSpeed);

        if (isMapActive)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            float zoomMod = mainCamera.orthographicSize / normalSize;
            Vector3 move = new Vector3(h, v, 0) * freeMoveSpeed * Time.unscaledDeltaTime * zoomMod;
            transform.position += move;

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minX, maxX),
                Mathf.Clamp(transform.position.y, minY, maxY),
                transform.position.z
            );
        }
    }

    void ToggleMap()
    {
        if (camControl == null) return;
        isMapActive = !isMapActive;
        camControl.enabled = !isMapActive;

        if (camControl.target != null)
        {
            var pMove = camControl.target.GetComponent<Move>();
            if (pMove != null) pMove.SetInputState(!isMapActive);
            
            var rb = camControl.target.GetComponent<Rigidbody2D>();
            if (isMapActive && rb != null) rb.linearVelocity = Vector2.zero;
        }

        if (!isMapActive) camControl.TeleportToTarget();
    }
}