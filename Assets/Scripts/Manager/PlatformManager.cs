using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void NotifyColorChange(string playerTag)
    {
        PlatformColor[] platforms = FindObjectsByType<PlatformColor>(FindObjectsSortMode.None);
        foreach (var p in platforms) p.UpdateState(playerTag);
    }
}