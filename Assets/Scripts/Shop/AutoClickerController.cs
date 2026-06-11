using UnityEngine;

/// <summary>
/// Periodically clicks the ant farm automatically.
/// Unlocked by purchasing the "Auto-Clicker" shop item.
/// Speed improved by "Auto-Clicker Speed Boost" shop item.
///
/// Attach to the GameManager or AntFarm root.
/// </summary>
public class AutoClickerController : MonoBehaviour
{
    public static AutoClickerController Instance { get; private set; }

    [Header("Settings")]
    public float defaultInterval = 5f;   // seconds between auto-clicks (before upgrades)

    private bool  _active   = false;
    private float _interval = 5f;
    private float _timer    = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _interval = defaultInterval;
    }

    private void Update()
    {
        if (!_active) return;

        _timer += Time.deltaTime;
        if (_timer >= _interval)
        {
            _timer = 0f;
            PerformAutoClick();
        }
    }

    private void PerformAutoClick()
    {
        GameManager.Instance.OnAntFarmClicked();

        // Spawn floating text at a random position near the centre of the farm
        Vector3 pos = Camera.main != null
            ? Camera.main.ViewportToWorldPoint(new Vector3(0.5f + Random.Range(-0.1f, 0.1f), 0.4f, 10f))
            : Vector3.zero;

        var spawner = FindObjectOfType<FloatingTextSpawner>();
        spawner?.Spawn("🤖 +" + BigNumberFormatter.FormatShort(CurrencyManager.Instance.AntsPerClick),
                       pos, UnityEngine.Color.cyan);
    }

    public void Activate()
    {
        _active = true;
        Debug.Log($"🤖 Auto-clicker activated (every {_interval}s)");
    }

    public void SetInterval(float interval)
    {
        _interval = Mathf.Max(0.1f, interval);
        Debug.Log($"🤖 Auto-clicker interval → {_interval}s");
    }
}
