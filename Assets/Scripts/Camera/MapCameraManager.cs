using UnityEngine;

// Map camera mode controller (Управление режимом карты)
public class MapCameraManager : MonoBehaviour
{
    [Header("Settings (Настройки)")]
    public float freeMoveSpeed = 40f; // Free camera movement speed (Скорость свободного движения)
    public KeyCode mapKey = KeyCode.M; // Toggle map key (Кнопка открытия карты)
    public float normalSize = 9f; // Default camera size (Обычный размер камеры)
    public float mapViewingSize = 25f; // Zoomed-out map size (Размер камеры в режиме карты)
    public float zoomSpeed = 5f; // Zoom interpolation speed (Скорость зума)

    [Header("Bounds (Границы)")]
    public float minX = -100f; public float maxX = 100f; // X limits (Ограничения по X)
    public float minY = -100f; public float maxY = 100f; // Y limits (Ограничения по Y)

    private CameraControl camControl; // Reference to follow camera (Ссылка на камеру слежения)
    private Camera mainCamera; // Main camera reference (Основная камера)
    private bool isMapActive = false; // Map mode state (Активен ли режим карты)

    void Start()
    {
        // Cache components (Получение компонентов)
        camControl = GetComponent<CameraControl>();
        mainCamera = GetComponent<Camera>();

        // Store initial camera size (Сохранить начальный размер камеры)
        if (mainCamera != null && mainCamera.orthographic)
            normalSize = mainCamera.orthographicSize;
    }

    void Update()
    {
        // Toggle map mode (Переключение режима карты)
        if (Input.GetKeyDown(mapKey)) ToggleMap();
        if (mainCamera == null) return;

        // Smooth zoom between normal and map view (Плавный переход зума)
        float targetSize = isMapActive ? mapViewingSize : normalSize;
        mainCamera.orthographicSize = Mathf.Lerp(
            mainCamera.orthographicSize,
            targetSize,
            Time.unscaledDeltaTime * zoomSpeed
        );

        // Free camera movement in map mode (Свободное движение камеры в режиме карты)
        if (isMapActive)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            // Adjust movement speed based on zoom (Скорость зависит от зума)
            float zoomMod = mainCamera.orthographicSize / normalSize;

            Vector3 move = new Vector3(h, v, 0) * freeMoveSpeed * Time.unscaledDeltaTime * zoomMod;
            transform.position += move;

            // Clamp camera inside bounds (Ограничение камеры в пределах карты)
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minX, maxX),
                Mathf.Clamp(transform.position.y, minY, maxY),
                transform.position.z
            );
        }
    }

    // Toggle between follow mode and map mode (Переключение между камерой и картой)
    void ToggleMap()
    {
        if (camControl == null) return;

        isMapActive = !isMapActive;

        // Enable/disable follow camera (Включить/выключить следование за игроком)
        camControl.enabled = !isMapActive;

        if (camControl.target != null)
        {
            // Disable player input in map mode (Отключить управление игроком)
            var pMove = camControl.target.GetComponent<Move>();
            if (pMove != null) pMove.SetInputState(!isMapActive);

            // Stop player movement when entering map (Остановить игрока при входе в карту)
            var rb = camControl.target.GetComponent<Rigidbody2D>();
            if (isMapActive && rb != null) rb.linearVelocity = Vector2.zero;
        }

        // Snap camera back to player when exiting map (Вернуть камеру к игроку)
        if (!isMapActive) camControl.TeleportToTarget();
    }
}