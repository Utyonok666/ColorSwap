using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI recordText;

    [Header("Player Tags to Track")]
    public string[] playerTags = { "Player", "PlayerRed", "PlayerYellow", "PlayerGreen" };
    
    private Transform _playerTransform;
    private float _bestHeightAllTime = 0f;

    void Start()
    {
        // 1. Загружаем рекорд из памяти (Best)
        _bestHeightAllTime = PlayerPrefs.GetFloat("BestHeight", 0f);
        
        FindPlayer();
        UpdateVisuals(0); // На старте высота 0
    }

    void FindPlayer()
    {
        foreach (string tag in playerTags)
        {
            GameObject player = GameObject.FindGameObjectWithTag(tag);
            if (player != null)
            {
                _playerTransform = player.transform;
                return; 
            }
        }
    }

    void Update()
    {
        if (_playerTransform == null || !_playerTransform.gameObject.activeInHierarchy)
        {
            FindPlayer();
            return;
        }

        // 2. Текущая высота — это позиция игрока по Y прямо сейчас
        float currentY = _playerTransform.position.y;
        
        // Ограничиваем, чтобы высота не была отрицательной (если упал ниже нуля)
        float heightToShow = Mathf.Max(0, currentY);

        // 3. Если текущая высота больше рекорда — обновляем рекорд
        if (currentY > _bestHeightAllTime)
        {
            _bestHeightAllTime = currentY;
            PlayerPrefs.SetFloat("BestHeight", _bestHeightAllTime);
        }

        // 4. Обновляем текст каждый кадр
        UpdateVisuals(heightToShow);
    }

    public void UpdateVisuals(float currentHeight)
    {
        int displayHeight = Mathf.FloorToInt(currentHeight);
        int displayBest = Mathf.FloorToInt(_bestHeightAllTime);

        if (heightText != null) 
            heightText.text = "Height: " + displayHeight + "m";
        
        if (recordText != null) 
            recordText.text = "Best: " + displayBest + "m";
    }
}