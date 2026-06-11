using UnityEngine;
using System.Collections;

/// <summary>
/// A rare golden ant that wanders the farm.
/// Clicking it awards a large ant bonus.
/// Spawns on a random interval; despawns after a timeout if not clicked.
///
/// Setup:
///   - Attach to an empty "GoldenAntController" GameObject.
///   - Assign goldenAntPrefab (a shiny golden AntAgent variant).
///   - Spawn happens automatically once Activate() is called.
/// </summary>
public class GoldenAntController : MonoBehaviour
{
    public static GoldenAntController Instance { get; private set; }

    [Header("Prefab")]
    public GameObject goldenAntPrefab;
    public Transform  spawnParent;

    [Header("Timing")]
    public float minSpawnInterval = 60f;    // seconds between spawns
    public float maxSpawnInterval = 180f;
    public float despawnAfter     = 20f;    // disappears if not clicked

    [Header("Reward")]
    [Tooltip("Multiplier applied to current APS for the reward. e.g. 100 = 100s of APS")]
    public float rewardAPSSeconds = 100f;

    private bool      _active        = false;
    private GameObject _currentGoldenAnt = null;
    private Coroutine  _spawnRoutine  = null;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Activate()
    {
        if (_active) return;
        _active = true;
        _spawnRoutine = StartCoroutine(SpawnLoop());
        Debug.Log("✨ Golden ant system activated!");
    }

    private IEnumerator SpawnLoop()
    {
        while (_active)
        {
            float wait = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(wait);

            if (_currentGoldenAnt == null)
                yield return StartCoroutine(SpawnGoldenAnt());
        }
    }

    private IEnumerator SpawnGoldenAnt()
    {
        if (goldenAntPrefab == null) yield break;

        // Spawn at random position within the farm
        Vector3 pos = Camera.main != null
            ? Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.1f, 0.9f), Random.Range(0.2f, 0.7f), 10f))
            : Vector3.zero;

        _currentGoldenAnt = Instantiate(goldenAntPrefab, pos, Quaternion.identity, spawnParent);

        // Attach click handler via the GoldenAntClickable component
        var clickable = _currentGoldenAnt.AddComponent<GoldenAntClickable>();
        clickable.Init(this);

        Debug.Log("✨ Golden ant appeared!");

        // Despawn after timeout
        yield return new WaitForSeconds(despawnAfter);

        if (_currentGoldenAnt != null)
        {
            Destroy(_currentGoldenAnt);
            _currentGoldenAnt = null;
            Debug.Log("✨ Golden ant escaped...");
        }
    }

    public void OnGoldenAntClicked()
    {
        double reward = CurrencyManager.Instance.AntsPerSecond * rewardAPSSeconds;
        if (reward < 1) reward = 1000;

        CurrencyManager.Instance.AddAnts(reward);

        // Big floating text
        if (_currentGoldenAnt != null)
        {
            var spawner = FindObjectOfType<FloatingTextSpawner>();
            spawner?.Spawn($"✨ +{BigNumberFormatter.FormatShort(reward)} ants!",
                           _currentGoldenAnt.transform.position,
                           new Color(1f, 0.85f, 0f));    // gold colour

            Destroy(_currentGoldenAnt);
            _currentGoldenAnt = null;
        }

        Debug.Log($"✨ Golden ant clicked! Reward: +{BigNumberFormatter.Format(reward)} ants");
    }
}

/// <summary>
/// Attached at runtime to the golden ant GameObject — detects clicks/taps.
/// </summary>
public class GoldenAntClickable : MonoBehaviour
{
    private GoldenAntController _controller;

    public void Init(GoldenAntController controller)
        => _controller = controller;

    private void OnMouseDown()
        => _controller?.OnGoldenAntClicked();
}
