using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Random weather events that play over the ant farm.
/// Each event has a visual (overlay + particles), a duration,
/// and an optional gameplay effect (e.g. rain boosts APS briefly).
///
/// Setup:
///   - Attach to "WeatherController" GameObject.
///   - Assign weatherPanel (an Image that tints the screen).
///   - Assign rainParticles, stormParticles, snowParticles (ParticleSystems).
///   - Assign weatherLabel (optional TMP label showing "⛈ Thunderstorm!").
/// </summary>
public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance { get; private set; }

    public enum WeatherType { Sunny, Rain, Thunderstorm, Snow, Heatwave, Fog }

    [System.Serializable]
    public class WeatherEvent
    {
        public WeatherType type;
        public string      emoji;
        public string      label;
        public Color       tint;
        public float       minDuration;
        public float       maxDuration;
        [Tooltip("APS multiplier while this weather is active (1 = no change)")]
        public float       apsMultiplier = 1f;
        public float       weight        = 1f;   // relative spawn probability
    }

    [Header("References")]
    public UnityEngine.UI.Image       weatherPanel;
    public TMPro.TextMeshProUGUI      weatherLabel;
    public ParticleSystem             rainParticles;
    public ParticleSystem             stormParticles;
    public ParticleSystem             snowParticles;

    [Header("Weather Events")]
    public List<WeatherEvent> weatherEvents = new List<WeatherEvent>
    {
        new WeatherEvent { type=WeatherType.Rain,        emoji="🌧",  label="Rain",       tint=new Color(0.5f,0.6f,0.8f,0.25f), minDuration=20, maxDuration=45, apsMultiplier=1.2f, weight=3 },
        new WeatherEvent { type=WeatherType.Thunderstorm,emoji="⛈",  label="Thunderstorm",tint=new Color(0.2f,0.2f,0.4f,0.45f), minDuration=10, maxDuration=25, apsMultiplier=0.8f, weight=1 },
        new WeatherEvent { type=WeatherType.Snow,        emoji="❄️",  label="Snow",        tint=new Color(0.9f,0.95f,1f,0.2f),  minDuration=30, maxDuration=60, apsMultiplier=0.9f, weight=2 },
        new WeatherEvent { type=WeatherType.Heatwave,    emoji="🔥",  label="Heatwave",    tint=new Color(1f,0.6f,0.2f,0.2f),   minDuration=15, maxDuration=35, apsMultiplier=1.5f, weight=1 },
        new WeatherEvent { type=WeatherType.Fog,         emoji="🌫",  label="Fog",         tint=new Color(0.8f,0.8f,0.8f,0.35f), minDuration=20, maxDuration=40, apsMultiplier=1.0f, weight=2 },
    };

    [Header("Timing")]
    public float minIdleTime = 30f;
    public float maxIdleTime = 120f;

    private bool         _active         = false;
    private WeatherEvent _currentWeather = null;
    private double       _apsBonus       = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (weatherPanel != null) weatherPanel.color = Color.clear;
        if (weatherLabel != null) weatherLabel.gameObject.SetActive(false);
    }

    public void Activate()
    {
        if (_active) return;
        _active = true;
        StartCoroutine(WeatherLoop());
        Debug.Log("🌤 Weather system activated!");
    }

    private IEnumerator WeatherLoop()
    {
        while (_active)
        {
            // Idle period — sunny / no effect
            float idle = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(idle);

            // Pick a random weather event by weight
            WeatherEvent picked = PickWeather();
            float duration = Random.Range(picked.minDuration, picked.maxDuration);

            yield return StartCoroutine(PlayWeather(picked, duration));
        }
    }

    private IEnumerator PlayWeather(WeatherEvent weather, float duration)
    {
        _currentWeather = weather;

        // Apply APS effect
        if (weather.apsMultiplier != 1f)
        {
            _apsBonus = CurrencyManager.Instance.AntsPerSecond * (weather.apsMultiplier - 1.0);
            CurrencyManager.Instance.AddToAntsPerSecond(_apsBonus);
        }

        // Show label
        if (weatherLabel != null)
        {
            weatherLabel.text = $"{weather.emoji} {weather.label}!";
            weatherLabel.gameObject.SetActive(true);
        }

        // Fade in panel tint
        yield return StartCoroutine(FadePanel(Color.clear, weather.tint, 1.5f));

        // Activate particles
        ToggleParticles(weather.type, true);

        // Wait
        yield return new WaitForSeconds(duration);

        // Fade out
        yield return StartCoroutine(FadePanel(weather.tint, Color.clear, 1.5f));
        ToggleParticles(weather.type, false);

        if (weatherLabel != null)
            weatherLabel.gameObject.SetActive(false);

        // Remove APS bonus
        if (weather.apsMultiplier != 1f && _apsBonus != 0)
        {
            CurrencyManager.Instance.AddToAntsPerSecond(-_apsBonus);
            _apsBonus = 0;
        }

        _currentWeather = null;
    }

    private IEnumerator FadePanel(Color from, Color to, float dur)
    {
        if (weatherPanel == null) yield break;
        float elapsed = 0f;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            weatherPanel.color = Color.Lerp(from, to, elapsed / dur);
            yield return null;
        }
        weatherPanel.color = to;
    }

    private void ToggleParticles(WeatherType type, bool on)
    {
        if (rainParticles  != null) { if (type == WeatherType.Rain || type == WeatherType.Thunderstorm) { if (on) rainParticles.Play(); else rainParticles.Stop(); } }
        if (stormParticles != null) { if (type == WeatherType.Thunderstorm) { if (on) stormParticles.Play(); else stormParticles.Stop(); } }
        if (snowParticles  != null) { if (type == WeatherType.Snow) { if (on) snowParticles.Play(); else snowParticles.Stop(); } }
    }

    private WeatherEvent PickWeather()
    {
        float totalWeight = 0;
        foreach (var e in weatherEvents) totalWeight += e.weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var e in weatherEvents)
        {
            cumulative += e.weight;
            if (roll <= cumulative) return e;
        }
        return weatherEvents[0];
    }
}
