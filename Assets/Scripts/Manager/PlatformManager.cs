using UnityEngine;

// Manages all platforms and updates them based on player color
// Управляет платформами и обновляет их в зависимости от цвета игрока
public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance; // Singleton instance (Синглтон)

    void Awake()
    {
        Instance = this; // Assign singleton reference (Назначаем синглтон)
    }

    // Notify all platforms of the current player color
    // Уведомляем все платформы о текущем цвете игрока
    public void NotifyColorChange(string playerTag)
    {
        // Find all platform objects (Находим все объекты платформ)
        PlatformColor[] platforms = FindObjectsByType<PlatformColor>(FindObjectsSortMode.None);

        // Update each platform according to player color
        // Обновляем каждую платформу в зависимости от цвета игрока
        foreach (var p in platforms) p.UpdateState(playerTag);
    }
}