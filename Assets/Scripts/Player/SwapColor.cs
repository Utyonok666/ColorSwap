using UnityEngine;

public class SwapColor : MonoBehaviour
{
    public SpriteRenderer playerSR;
    public Color redColor = Color.red;
    public Color yellowColor = Color.yellow;
    public Color greenColor = Color.green;

    private float _timer;
    private int _index = 0;

    void Start()
    {
        if (playerSR == null) playerSR = GetComponent<SpriteRenderer>();
        SetNewPhase();
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _index = (_index + 1) % 3;
            SetNewPhase();
        }
    }

    void SetNewPhase()
    {
        _timer = Random.Range(2f, 5f);
        
        if (_index == 0) Apply("PlayerRed", redColor);
        else if (_index == 1) Apply("PlayerYellow", yellowColor);
        else Apply("PlayerGreen", greenColor);
    }

    void Apply(string t, Color c)
    {
        gameObject.tag = t;
        if (playerSR != null) playerSR.color = c;
        if (PlatformManager.Instance != null) PlatformManager.Instance.NotifyColorChange(t);
    }
}