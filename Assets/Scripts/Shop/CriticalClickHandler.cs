using UnityEngine;

/// <summary>
/// Wraps CurrencyManager.Click() with critical-hit logic.
/// ShopManager populates critClickChance and critClickMultiplier.
/// 
/// Replace direct CurrencyManager.Instance.Click() calls with
/// CriticalClickHandler.Instance.Click() once the shop is active.
/// Already wired into GameManager.OnAntFarmClicked().
/// </summary>
public class CriticalClickHandler : MonoBehaviour
{
    public static CriticalClickHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Performs a click — rolls for crit, applies correct multiplier,
    /// then adds ants. Returns whether the click was a crit.
    /// </summary>
    public bool Click(out double antsGained)
    {
        var  cm       = CurrencyManager.Instance;
        var  sm       = ShopManager.Instance;
        bool isCrit   = sm != null && Random.value < sm.critClickChance;
        float mult    = isCrit ? (sm?.critClickMultiplier ?? 5f) : 1f;

        antsGained = cm.AntsPerClick * mult;
        cm.AddAnts(antsGained);
        return isCrit;
    }
}
