using UnityEngine;

/// <summary>
/// Handles the prestige ("Fly to the Moon") reset loop.
/// Awards Ant DNA based on total ants ever accumulated.
/// </summary>
public class PrestigeManager : MonoBehaviour
{
    public static PrestigeManager Instance { get; private set; }

    [Header("Prestige Settings")]
    [Tooltip("Minimum total ants ever required to unlock the prestige button")]
    public double prestigeUnlockThreshold = 1_000_000_000;  // 1 Billion ants

    [Tooltip("DNA awarded = floor( sqrt(totalAntsEver / dnaDivisor) )")]
    public double dnaDivisor = 1_000_000;  // tune this to balance DNA income

    [Header("Prestige State")]
    [SerializeField] private int _prestigeCount = 0;
    public int PrestigeCount => _prestigeCount;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool CanPrestige()
        => CurrencyManager.Instance.TotalAntsEver >= prestigeUnlockThreshold;

    /// <summary>
    /// Calculates DNA that WOULD be earned right now (shown in UI before committing).
    /// </summary>
    public double CalculatePendingDNA()
    {
        double raw = CurrencyManager.Instance.TotalAntsEver / dnaDivisor;
        return System.Math.Floor(System.Math.Sqrt(raw));
    }

    /// <summary>
    /// Performs the full prestige reset — called when player confirms "Fly to the Moon".
    /// </summary>
    public void DoPrestige()
    {
        if (!CanPrestige())
        {
            Debug.LogWarning("Prestige attempted but threshold not met.");
            return;
        }

        double dnaEarned = CalculatePendingDNA();

        // Award DNA BEFORE reset so it isn't wiped
        CurrencyManager.Instance.AddAntDNA(dnaEarned);

        // Reset everything else
        CurrencyManager.Instance.ResetForPrestige();
        BuildingManager.Instance.ResetForPrestige();

        _prestigeCount++;

        // Apply persistent DNA bonuses
        ApplyDNABonuses();

        GameManager.Instance.saveManager.SaveGame();

        Debug.Log($"🚀 Prestige #{_prestigeCount}! Earned {dnaEarned} DNA. Total DNA: {CurrencyManager.Instance.AntDNA}");
    }

    /// <summary>
    /// Re-applies all passive bonuses that scale with total DNA owned.
    /// Called after every prestige and on game load.
    /// </summary>
    public void ApplyDNABonuses()
    {
        double dna = CurrencyManager.Instance.AntDNA;

        // +2% click power per DNA owned (diminishing returns via sqrt)
        double clickBonus = 1.0 + (System.Math.Sqrt(dna) * 0.02);
        CurrencyManager.Instance.SetAntsPerClick(clickBonus);

        // buildings keep their own multipliers — add more bonus logic here
        // e.g. BuildingManager.Instance.ApplyGlobalAPSBonus(...)
    }

    // ── Save / Load ───────────────────────────────────────────────
    public void LoadFromData(SaveData data)
    {
        _prestigeCount = data.prestigeCount;
        ApplyDNABonuses();
    }

    public void WriteToData(SaveData data)
    {
        data.prestigeCount = _prestigeCount;
    }
}
