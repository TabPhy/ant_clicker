using UnityEngine;
using System.Collections;

/// <summary>
/// Toggles the ant farm between day and night visuals.
/// Night mode tints everything dark blue and adds a star layer.
///
/// Setup:
///   - Attach to "NightModeController" GameObject.
///   - Assign nightOverlay (full-screen dark Image, starts hidden).
///   - Assign starsObject (a starfield sprite or particle system, starts hidden).
///   - Assign moonObject (a moon sprite that bobs in the sky, starts hidden).
///   - Toggle is exposed to the UI via IsNightMode.
/// </summary>
public class NightModeController : MonoBehaviour
{
    public static NightModeController Instance { get; private set; }

    [Header("References")]
    public UnityEngine.UI.Image nightOverlay;   // dark translucent panel
    public GameObject           starsObject;
    public GameObject           moonObject;

    [Header("Settings")]
    public Color  nightTint       = new Color(0.05f, 0.08f, 0.25f, 0.72f);
    public float  transitionTime  = 1.5f;

    public bool IsNightMode { get; private set; } = false;
    private bool _unlocked = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (nightOverlay != null) nightOverlay.color = Color.clear;
        if (starsObject  != null) starsObject.SetActive(false);
        if (moonObject   != null) moonObject.SetActive(false);
    }

    public void Activate()
    {
        _unlocked = true;
        Debug.Log("🌙 Night mode unlocked!");
        // Auto-switch to night as a treat on first unlock
        SetNightMode(true);
    }

    public void Toggle() { if (_unlocked) SetNightMode(!IsNightMode); }

    public void SetNightMode(bool night)
    {
        if (!_unlocked) return;
        IsNightMode = night;
        StopAllCoroutines();
        StartCoroutine(TransitionNight(night));
    }

    private IEnumerator TransitionNight(bool toNight)
    {
        Color targetColour = toNight ? nightTint : Color.clear;
        Color startColour  = nightOverlay != null ? nightOverlay.color : Color.clear;

        if (toNight)
        {
            if (starsObject != null) starsObject.SetActive(true);
            if (moonObject  != null) moonObject.SetActive(true);
        }

        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t  = elapsed / transitionTime;
            if (nightOverlay != null)
                nightOverlay.color = Color.Lerp(startColour, targetColour, t);
            yield return null;
        }

        if (!toNight)
        {
            if (starsObject != null) starsObject.SetActive(false);
            if (moonObject  != null) moonObject.SetActive(false);
        }

        Debug.Log(toNight ? "🌙 Night mode on." : "☀️ Day mode on.");
    }
}
