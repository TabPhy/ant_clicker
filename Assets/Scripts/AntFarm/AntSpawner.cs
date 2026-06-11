using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the pool of visible ant agents on screen.
/// Gradually increases visible ant count as the stage upgrades.
/// Uses object pooling so ants are never Instantiated/Destroyed at runtime.
///
/// Scene setup:
///   - Attach to the same GameObject as AntFarmBackground (or a child).
///   - Assign antPrefab (GameObject with AntAgent component).
///   - Assign spawnParent (an empty Transform to keep the hierarchy tidy).
///   - Pool size = max ants across all stages (set poolSize accordingly).
/// </summary>
public class AntSpawner : MonoBehaviour
{
    [Header("Prefab & Pool")]
    public GameObject antPrefab;
    public Transform  spawnParent;
    public int        poolSize   = 100;   // set to max ants you'll ever show

    [Header("Spawn Bounds (world units)")]
    public float minX = -200f;
    public float maxX =  200f;
    public float minY = -300f;
    public float maxY =  -20f;

    [Header("Spawn Rate")]
    [Tooltip("Seconds between activating one new ant when ramping up")]
    public float antSpawnDelay = 0.3f;

    // ── Pool ──────────────────────────────────────────────────────
    private List<AntAgent> _pool   = new List<AntAgent>();
    private int            _active = 0;
    private int            _targetActive = 0;

    // ── Lifecycle ─────────────────────────────────────────────────
    private void Start()
    {
        BuildPool();
    }

    private void BuildPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject go  = Instantiate(antPrefab, spawnParent);
            AntAgent   ant = go.GetComponent<AntAgent>();
            ant.SetBounds(minX, maxX, minY, maxY);
            go.SetActive(false);
            _pool.Add(ant);
        }
    }

    // ── Called by AntFarmBackground when stage changes ─────────────
    public void OnStageChanged(AntFarmStageData stage)
    {
        _targetActive = stage.maxVisibleAnts;
        float speed   = stage.antWalkSpeed;
        float activity = stage.antActivityLevel;

        // Update speed/activity on all already-active ants
        for (int i = 0; i < _active && i < _pool.Count; i++)
        {
            _pool[i].walkSpeed     = speed;
            _pool[i].activityLevel = activity;
        }

        // Gradually spawn new ants up to target
        StopAllCoroutines();
        StartCoroutine(RampUpAnts(speed, activity));
    }

    private IEnumerator RampUpAnts(float speed, float activity)
    {
        while (_active < _targetActive && _active < _pool.Count)
        {
            AntAgent ant       = _pool[_active];
            ant.walkSpeed      = speed;
            ant.activityLevel  = activity;
            ant.RandomisePosition();
            ant.gameObject.SetActive(true);
            _active++;
            yield return new WaitForSeconds(antSpawnDelay);
        }
    }

    // ── Click spawn — burst a few extra ants on tap ───────────────
    public void OnFarmClicked(Vector3 clickWorldPos)
    {
        int burst = Mathf.Min(3, _pool.Count - _active);
        for (int i = 0; i < burst; i++)
        {
            AntAgent ant      = _pool[_active];
            ant.walkSpeed     = _pool[0].walkSpeed;
            ant.activityLevel = _pool[0].activityLevel;
            // Spawn near click position
            ant.transform.position = clickWorldPos + (Vector3)Random.insideUnitCircle * 20f;
            ant.gameObject.SetActive(true);
            _active++;
        }
    }

    // ── Prestige reset ────────────────────────────────────────────
    public void ResetAnts()
    {
        StopAllCoroutines();
        foreach (var ant in _pool)
            ant.gameObject.SetActive(false);
        _active       = 0;
        _targetActive = 0;
    }
}
