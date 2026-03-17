using UnityEngine;
using TMPro;

// Manages UI for player height and record display
// Управляет UI для отображения высоты игрока и рекорда
public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI heightText; // Current height display // Текущая высота
    public TextMeshProUGUI recordText; // Best height display // Рекорд

    [Header("Player Tags to Track")]
    public string[] playerTags = { "Player", "PlayerRed", "PlayerYellow", "PlayerGreen" };

    private Transform _playerTransform; // Reference to active player // Ссылка на активного игрока
    private float _bestHeightAllTime = 0f; // All-time best height // Рекорд за все время

    void Start()
    {
        // Load saved best height from PlayerPrefs
        // Загружаем сохраненный рекорд
        _bestHeightAllTime = PlayerPrefs.GetFloat("BestHeight", 0f);

        FindPlayer(); 
        UpdateVisuals(0); // Initialize UI with height 0
        // Инициализация UI с высотой 0
    }

    void FindPlayer()
    {
        // Try to find player by multiple tags
        // Пытаемся найти игрока по разным тегам
        foreach (string tag in playerTags)
        {
            GameObject player = GameObject.FindGameObjectWithTag(tag);
            if (player != null)
            {
                _playerTransform = player.transform;
                return; // Stop after first found player // Останавливаем после первого найденного игрока
            }
        }
    }

    void Update()
    {
        // Re-check player if missing or inactive
        // Проверяем игрока, если отсутствует или не активен
        if (_playerTransform == null || !_playerTransform.gameObject.activeInHierarchy)
        {
            FindPlayer();
            return;
        }

        // Current height based on Y position
        // Текущая высота на основе позиции по Y
        float currentY = _playerTransform.position.y;

        // Clamp to minimum 0 (no negative heights)
        // Ограничение минимальной высоты 0
        float heightToShow = Mathf.Max(0, currentY);

        // Update best height if current exceeds record
        // Обновляем рекорд, если текущая высота больше
        if (currentY > _bestHeightAllTime)
        {
            _bestHeightAllTime = currentY;
            PlayerPrefs.SetFloat("BestHeight", _bestHeightAllTime);
        }

        // Update UI elements each frame
        // Обновляем элементы UI каждый кадр
        UpdateVisuals(heightToShow);
    }

    public void UpdateVisuals(float currentHeight)
    {
        // Convert to integer for display
        // Преобразуем в целое число для отображения
        int displayHeight = Mathf.FloorToInt(currentHeight);
        int displayBest = Mathf.FloorToInt(_bestHeightAllTime);

        if (heightText != null) 
            heightText.text = "Height: " + displayHeight + "m"; // Display current height // Отображаем текущую высоту

        if (recordText != null) 
            recordText.text = "Best: " + displayBest + "m"; // Display best height // Отображаем рекорд
    }
}