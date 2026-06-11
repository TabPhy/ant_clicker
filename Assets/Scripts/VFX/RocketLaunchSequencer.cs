using UnityEngine;
using System.Collections;

/// <summary>
/// The rocket launch sequence triggered by the prestige "Fly to the Moon" action.
///
/// Flow:
///   1. Countdown overlay fades in (3… 2… 1… 🚀)
///   2. Rocket flies upward off screen with engine particles
///   3. Screen flashes white
///   4. Prestige reset fires
///   5. Background fades back to Stage 0
///
/// Setup:
///   - Attach to a "RocketLaunchSequencer" GameObject.
///   - Assign rocketTransform (the rocket sprite object — starts hidden).
///   - Assign countdownText (TextMeshPro for the countdown).
///   - Assign flashPanel (full-screen white Image, alpha starts 0).
///   - Assign engineParticles (ParticleSystem for rocket exhaust).
///   - Assign launchAudioSource (AudioSource with rocket sfx clip).
/// </summary>
public class RocketLaunchSequencer : MonoBehaviour
{
    public static RocketLaunchSequencer Instance { get; private set; }

    [Header("Scene References")]
    public Transform       rocketTransform;
    public TMPro.TextMeshProUGUI countdownText;
    public UnityEngine.UI.Image  flashPanel;
    public ParticleSystem  engineParticles;
    public AudioSource     launchAudioSource;

    [Header("Timing")]
    public float countdownInterval = 0.8f;   // seconds between countdown ticks
    public float rocketRiseDuration = 2.0f;  // seconds to fly off screen
    public float flashDuration      = 0.4f;

    [Header("Rocket Movement")]
    public float rocketRiseDistance = 1200f; // units to move upward

    private bool _isLaunching = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Start hidden
        if (rocketTransform != null)  rocketTransform.gameObject.SetActive(false);
        if (countdownText   != null)  countdownText.gameObject.SetActive(false);
        if (flashPanel      != null)  flashPanel.color = new Color(1, 1, 1, 0);
    }

    // ── Entry point — called by PrestigeUIController ──────────────
    public void LaunchAndPrestige()
    {
        if (_isLaunching) return;
        StartCoroutine(LaunchSequence());
    }

    // ── Main sequence ─────────────────────────────────────────────
    private IEnumerator LaunchSequence()
    {
        _isLaunching = true;
        GameManager.Instance.PauseGame();

        // ── Step 1: Countdown ─────────────────────────────────────
        if (countdownText != null) countdownText.gameObject.SetActive(true);

        string[] ticks = { "3", "2", "1", "🚀" };
        foreach (string tick in ticks)
        {
            if (countdownText != null) countdownText.text = tick;
            yield return new WaitForSeconds(countdownInterval);
        }

        if (countdownText != null) countdownText.gameObject.SetActive(false);

        // ── Step 2: Rocket rises ──────────────────────────────────
        if (rocketTransform != null)
        {
            rocketTransform.gameObject.SetActive(true);
            engineParticles?.Play();
            launchAudioSource?.Play();

            Vector3 startPos = rocketTransform.position;
            Vector3 endPos   = startPos + Vector3.up * rocketRiseDistance;
            float   elapsed  = 0f;

            while (elapsed < rocketRiseDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t  = elapsed / rocketRiseDuration;
                // Ease in (accelerating rocket)
                float eased = t * t;
                rocketTransform.position = Vector3.Lerp(startPos, endPos, eased);
                yield return null;
            }

            rocketTransform.gameObject.SetActive(false);
            engineParticles?.Stop();
        }

        // ── Step 3: Flash ─────────────────────────────────────────
        if (flashPanel != null)
            yield return StartCoroutine(Flash());

        // ── Step 4: Prestige reset ────────────────────────────────
        PrestigeManager.Instance.DoPrestige();
        AntFarmBackground.Instance?.ResetToFirstStage();
        AntSpawner[] spawners = FindObjectsOfType<AntSpawner>();
        foreach (var s in spawners) s.ResetAnts();
        StructureManager.Instance?.ResetStructures();

        // ── Step 5: Fade flash back out ───────────────────────────
        if (flashPanel != null)
            yield return StartCoroutine(FadeFlashOut());

        GameManager.Instance.ResumeGame();
        _isLaunching = false;
    }

    private IEnumerator Flash()
    {
        float elapsed = 0f;
        while (elapsed < flashDuration / 2f)
        {
            elapsed += Time.unscaledDeltaTime;
            flashPanel.color = new Color(1, 1, 1, elapsed / (flashDuration / 2f));
            yield return null;
        }
        flashPanel.color = Color.white;
    }

    private IEnumerator FadeFlashOut()
    {
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            flashPanel.color = new Color(1, 1, 1, 1f - elapsed / flashDuration);
            yield return null;
        }
        flashPanel.color = new Color(1, 1, 1, 0);
    }
}
