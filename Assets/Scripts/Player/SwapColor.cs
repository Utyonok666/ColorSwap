using UnityEngine;

// Changes the player's color and tag randomly every few seconds
// Меняет цвет и тег игрока каждые несколько секунд
public class SwapColor : MonoBehaviour
{
    public SpriteRenderer playerSR;      // Sprite of the player / Сприт игрока
    public Color redColor = Color.red;   
    public Color yellowColor = Color.yellow;
    public Color greenColor = Color.green;

    private float _timer;   // Countdown timer / Таймер до следующей смены
    private int _index = 0; // Tracks current color phase / Индекс текущей фазы

    void Start()
    {
        // If no SpriteRenderer assigned, take the component from this object
        // Если спрайт не назначен, берём компонент с объекта
        if (playerSR == null) playerSR = GetComponent<SpriteRenderer>();
        SetNewPhase(); // Initialize first color
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _index = (_index + 1) % 3; // Cycle through 0,1,2
            SetNewPhase();
        }
    }

    void SetNewPhase()
    {
        _timer = Random.Range(2f, 5f); // Next change in 2–5 seconds

        if (_index == 0) Apply("PlayerRed", redColor);
        else if (_index == 1) Apply("PlayerYellow", yellowColor);
        else Apply("PlayerGreen", greenColor);
    }

    void Apply(string t, Color c)
    {
        // Change tag for color-based platform interactions
        // Меняем тег для взаимодействия с платформами по цвету
        gameObject.tag = t;

        if (playerSR != null) playerSR.color = c;

        // Notify PlatformManager that color has changed
        // Уведомляем менеджер платформ о смене цвета
        if (PlatformManager.Instance != null)
            PlatformManager.Instance.NotifyColorChange(t);
    }
}