using UnityEngine;
using System.Collections;

/// <summary>
/// When purchased, a disco ball descends from the top of the farm,
/// spins, and triggers a "party mode" — ants move faster and colorful
/// light flashes pulse across the background for 30 seconds.
///
/// After party mode, the disco ball stays visible as a cosmetic.
///
/// Setup:
///   - Attach to an empty "DiscoBallController" GameObject.
///   - Assign discoBallObject (starts hidden, appears on activate).
///   - Assign partyLightPanel (full-screen coloured Image, alpha 0 at start).
///   - Assign the AntSpawner so we can boost ant speed.
/// </summary>
public class DiscoBallController : MonoBehaviour
{
    public static DiscoBallController Instance { get; private set; }

    [Header("References")]
    public GameObject              discoBallObject;
    public UnityEngine.UI.Image    partyLightPanel;
    public AntSpawner              antSpawner;

    [Header("Party Settings")]
    public float partyDuration     = 30f;
    public float antSpeedMultiplier = 3f;
    public float flashInterval     = 0.15f;

    private bool _isPartying = false;

    private readonly Color[] _partyColours =
    {
        new Color(1f, 0.2f, 0.5f, 0.18f),   // pink
        new Color(0.2f, 0.8f, 1f, 0.18f),   // cyan
        new Color(1f, 0.9f, 0.1f, 0.18f),   // yellow
        new Color(0.5f, 0.2f, 1f, 0.18f),   // purple
        new Color(0.2f, 1f, 0.4f, 0.18f),   // green
    };

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (discoBallObject != null) discoBallObject.SetActive(false);
        if (partyLightPanel != null) partyLightPanel.color = Color.clear;
    }

    public void Activate()
    {
        if (discoBallObject != null) discoBallObject.SetActive(true);

        // Attach a spin animator to the ball
        var spin = discoBallObject.GetComponent<IdleAnimator>();
        if (spin == null)
        {
            spin = discoBallObject.AddComponent<IdleAnimator>();
            spin.type      = IdleAnimator.AnimationType.Rotate;
            spin.speed     = 2f;
            spin.magnitude = 180f;   // full spin
        }

        if (!_isPartying)
            StartCoroutine(PartyMode());

        Debug.Log("🪩 Disco ball activated! PARTY TIME!");
    }

    private IEnumerator PartyMode()
    {
        _isPartying = true;
        float elapsed     = 0f;
        int   colourIndex = 0;

        // Boost ant speed temporarily
        var stage = AntFarmBackground.Instance?.CurrentStage;
        if (stage != null && antSpawner != null)
        {
            var boostedStage = ScriptableObject.CreateInstance<AntFarmStageData>();
            boostedStage.maxVisibleAnts  = stage.maxVisibleAnts;
            boostedStage.antWalkSpeed    = stage.antWalkSpeed * antSpeedMultiplier;
            boostedStage.antActivityLevel = 1f;
            antSpawner.OnStageChanged(boostedStage);
        }

        while (elapsed < partyDuration)
        {
            elapsed += flashInterval;

            if (partyLightPanel != null)
                partyLightPanel.color = _partyColours[colourIndex % _partyColours.Length];

            colourIndex++;
            yield return new WaitForSeconds(flashInterval);
        }

        // Return to normal
        if (partyLightPanel != null) partyLightPanel.color = Color.clear;
        if (stage != null && antSpawner != null)
            antSpawner.OnStageChanged(stage);

        _isPartying = false;
        Debug.Log("🪩 Party over. Back to work, ants.");
    }
}
