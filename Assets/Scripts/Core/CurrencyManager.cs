using UnityEngine;
using System;

/// <summary>
/// Manages all currencies: Ants (main) and Ant DNA (prestige).
/// Uses double precision so numbers can grow to astronomical sizes.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    // ── Main currency ──────────────────────────────────────────────
    [Header("Ants (main currency)")]
    [SerializeField] private double _ants = 0;
    [SerializeField] private double _totalAntsEver = 0;   // used for milestone checks
    [SerializeField] private double _antsPerClick = 1;
    [SerializeField] private double _antsPerSecond = 0;

    // ── Prestige currency ──────────────────────────────────────────
    [Header("Ant DNA (prestige currency)")]
    [SerializeField] private double _antDNA = 0;

    // ── Events ─────────────────────────────────────────────────────
    public event Action OnCurrencyChanged;

    // ── Public accessors ───────────────────────────────────────────
    public double Ants           => _ants;
    public double TotalAntsEver  => _totalAntsEver;
    public double AntsPerClick   => _antsPerClick;
    public double AntsPerSecond  => _antsPerSecond;
    public double AntDNA         => _antDNA;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Click ──────────────────────────────────────────────────────
    public void Click()
    {
        AddAnts(_antsPerClick);
    }

    // ── Passive income (called every frame from BuildingManager) ───
    public void AddPassiveIncome(double apsThisFrame)
    {
        AddAnts(apsThisFrame);
    }

    // ── Generic add / spend ────────────────────────────────────────
    public void AddAnts(double amount)
    {
        _ants           += amount;
        _totalAntsEver  += amount;
        OnCurrencyChanged?.Invoke();
    }

    public bool SpendAnts(double amount)
    {
        if (_ants < amount) return false;
        _ants -= amount;
        OnCurrencyChanged?.Invoke();
        return true;
    }

    public void AddAntDNA(double amount)
    {
        _antDNA += amount;
        OnCurrencyChanged?.Invoke();
    }

    // ── Per-click / per-second modifiers ──────────────────────────
    public void SetAntsPerClick(double value)  => _antsPerClick  = value;
    public void SetAntsPerSecond(double value) => _antsPerSecond = value;
    public void AddToAntsPerClick(double bonus)  => _antsPerClick  += bonus;
    public void AddToAntsPerSecond(double bonus) => _antsPerSecond += bonus;

    // ── Prestige reset ─────────────────────────────────────────────
    public void ResetForPrestige()
    {
        _ants          = 0;
        _totalAntsEver = 0;
        _antsPerClick  = 1;
        _antsPerSecond = 0;
        // DNA is kept intentionally
        OnCurrencyChanged?.Invoke();
    }

    // ── Save / Load helpers ────────────────────────────────────────
    public void LoadFromData(SaveData data)
    {
        _ants          = data.ants;
        _totalAntsEver = data.totalAntsEver;
        _antsPerClick  = data.antsPerClick;
        _antDNA        = data.antDNA;
    }

    public void WriteToData(SaveData data)
    {
        data.ants          = _ants;
        data.totalAntsEver = _totalAntsEver;
        data.antsPerClick  = _antsPerClick;
        data.antDNA        = _antDNA;
    }
}
