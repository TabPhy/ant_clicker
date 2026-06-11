using UnityEngine;
using System.Collections;

/// <summary>
/// Master controller for the ant farm background.
/// Watches TotalAntsEver and transitions between AntFarmStageData stages.
///
/// Scene setup:
///   - Create an empty GameObject "AntFarmBackground" and attach this script.
///   - Assign backgroundRenderer (a SpriteRenderer for the dirt layer).
///   - Assign overlayRenderer   (a SpriteRenderer drawn above dirt).
///   - Assign mainCamera        (sky colour applied to camera background).
///   - Assign stages[]          (ScriptableObjects, in ascending threshold order).
///   - Assign antSpawner and structureManager references.
/// </summary>
public class AntFarmBackground : MonoBehaviour
{
    public static AntFarmBackground Instance { get; private set; }

    [Header("Renderers")]
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer overlayRenderer;
    public Camera         mainCamera;

    [Header("Stages (assign in ascending threshold order)")]
    public AntFarmStageData[] stages;

    [Header("Transition")]
    public float transitionDuration = 2.0f;

    [Header("References")]
    public AntSpawner       antSpawner;
    public StructureManager structureManager;

    // ── Runtime state ─────────────────────────────────────────────
    private int   _currentStageIndex = -1;
    private float _checkInterval     = 2f;
    private float _checkTimer        = 0f;

    public AntFarmStageData CurrentStage =>
        (_currentStageIndex >= 0 && _currentStageIndex < stages.Length)
            ? stages[_currentStageIndex] : null;

    // ── Lifecycle ─────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        EvaluateStage(instant: true);
    }

    private void Update()
    {
        _checkTimer += Time.deltaTime;
        if (_checkTimer >= _checkInterval)
        {
            _checkTimer = 0f;
            EvaluateStage(instant: false);
        }
    }

    // ── Stage evaluation ──────────────────────────────────────────
    private void EvaluateStage(bool instant)
    {
        double totalAnts = CurrencyManager.Instance.TotalAntsEver;
        int    newIndex  = 0;

        for (int i = stages.Length - 1; i >= 0; i--)
        {
            if (totalAnts >= stages[i].unlockAtTotalAnts)
            {
                newIndex = i;
                break;
            }
        }

        if (newIndex == _currentStageIndex) return;

        _currentStageIndex = newIndex;
        AntFarmStageData stage = stages[newIndex];

        if (instant)
            ApplyStageInstant(stage);
        else
        {
            StopAllCoroutines();
            StartCoroutine(TransitionToStage(stage));
        }

        Debug.Log($"🐜 Ant farm stage → {stage.stageName}");
        antSpawner?.OnStageChanged(stage);
        structureManager?.ActivateStructuresForStage(stage);
    }

    // ── Instant apply ─────────────────────────────────────────────
    private void ApplyStageInstant(AntFarmStageData stage)
    {
        if (backgroundRenderer != null)
        {
            backgroundRenderer.sprite = stage.backgroundSprite;
            backgroundRenderer.color  = stage.groundColour;
        }

        if (overlayRenderer != null)
        {
            overlayRenderer.sprite = stage.overlaySprite;
            overlayRenderer.color  = Color.white;
        }

        if (mainCamera != null)
            mainCamera.backgroundColor = stage.skyColour;
    }

    // ── Crossfade transition ──────────────────────────────────────
    private IEnumerator TransitionToStage(AntFarmStageData stage)
    {
        float elapsed    = 0f;
        Color startGround = backgroundRenderer != null ? backgroundRenderer.color : Color.white;
        Color startSky    = mainCamera         != null ? mainCamera.backgroundColor : Color.white;

        if (overlayRenderer != null && stage.overlaySprite != null)
        {
            overlayRenderer.sprite = stage.overlaySprite;
            overlayRenderer.color  = new Color(1, 1, 1, 0);
        }

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t  = elapsed / transitionDuration;

            if (backgroundRenderer != null)
                backgroundRenderer.color = Color.Lerp(startGround, stage.groundColour, t);

            if (mainCamera != null)
                mainCamera.backgroundColor = Color.Lerp(startSky, stage.skyColour, t);

            if (overlayRenderer != null)
                overlayRenderer.color = new Color(1, 1, 1, t);

            yield return null;
        }

        ApplyStageInstant(stage);
    }

    // ── Prestige reset ────────────────────────────────────────────
    public void ResetToFirstStage()
    {
        StopAllCoroutines();
        _currentStageIndex = -1;
        EvaluateStage(instant: true);
    }
}
