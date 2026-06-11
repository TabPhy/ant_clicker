using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Owns all ShopItems, applies their effects on purchase, and exposes
/// derived game-state values (critChance, autoClickerRate, etc.) to
/// other systems that need them.
///
/// Attach to the GameManager GameObject alongside other managers.
/// Assign all ShopItemData ScriptableObjects in the Inspector.
/// </summary>
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("All Shop Items (assign ScriptableObjects here)")]
    public ShopItemData[] allItems;

    // ── Runtime list ──────────────────────────────────────────────
    private List<ShopItem> _items = new List<ShopItem>();
    public  IReadOnlyList<ShopItem> Items => _items;

    // ── Derived state — read by other systems ─────────────────────
    [HideInInspector] public float  maxOfflineHours       = 12f;
    [HideInInspector] public bool   offlineIncomeDoubled  = false;
    [HideInInspector] public bool   bulkBuyUnlocked       = false;
    [HideInInspector] public bool   autoClickerUnlocked   = false;
    [HideInInspector] public float  autoClickerInterval   = 5f;    // seconds between auto-clicks
    [HideInInspector] public bool   antInspectorUnlocked  = false;
    [HideInInspector] public bool   showExactNumbers      = false;
    [HideInInspector] public float  critClickChance       = 0f;    // 0–1
    [HideInInspector] public float  critClickMultiplier   = 5f;

    // Visual unlocks — read by AntSpawner / VFX systems
    [HideInInspector] public bool   fireAntsUnlocked      = false;
    [HideInInspector] public bool   carpenterAntsUnlocked = false;
    [HideInInspector] public bool   armyAntsUnlocked      = false;
    [HideInInspector] public bool   goldenAntUnlocked     = false;
    [HideInInspector] public bool   antHatUnlocked        = false;
    [HideInInspector] public bool   discoBallUnlocked     = false;
    [HideInInspector] public bool   rainbowTrailsUnlocked = false;
    [HideInInspector] public bool   nightModeUnlocked     = false;
    [HideInInspector] public bool   weatherSystemUnlocked = false;
    [HideInInspector] public bool   antSongsUnlocked      = false;

    // ── Events ────────────────────────────────────────────────────
    public event Action<ShopItem> OnItemPurchased;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Init (called after save load) ─────────────────────────────
    public void InitializeShop(HashSet<string> purchasedIds)
    {
        _items.Clear();
        foreach (var data in allItems)
        {
            bool bought = purchasedIds.Contains(data.id);
            var  item   = new ShopItem(data, bought);
            item.OnPurchased += HandlePurchase;
            _items.Add(item);
        }

        // Re-apply all purchased effects silently on load
        foreach (var item in _items)
            if (item.Purchased)
                ApplyEffects(item.Data, silent: true);

        Debug.Log($"🛒 Shop initialised — {purchasedIds.Count} items already purchased.");
    }

    // ── Purchase ──────────────────────────────────────────────────
    public bool PurchaseItem(int index)
    {
        if (index < 0 || index >= _items.Count) return false;
        return _items[index].TryPurchase();
    }

    private void HandlePurchase(ShopItem item)
    {
        ApplyEffects(item.Data, silent: false);
        OnItemPurchased?.Invoke(item);
        GameManager.Instance.saveManager.SaveGame();
        Debug.Log($"🛒 Purchased: {item.Data.itemName}");
    }

    // ── Effect application ────────────────────────────────────────
    private void ApplyEffects(ShopItemData data, bool silent)
    {
        var cm = CurrencyManager.Instance;
        var bm = BuildingManager.Instance;
        var sm = GameManager.Instance.saveManager;

        foreach (var entry in data.effects)
        {
            switch (entry.effect)
            {
                // QoL
                case ShopItemEffect.IncreaseOfflineIncomeHours:
                    maxOfflineHours += (float)entry.value;
                    sm.maxOfflineHours = maxOfflineHours;
                    break;
                case ShopItemEffect.DoubleOfflineIncome:
                    offlineIncomeDoubled = true;
                    break;
                case ShopItemEffect.ShowExactNumbers:
                    showExactNumbers = true;
                    break;
                case ShopItemEffect.UnlockAutoClicker:
                    autoClickerUnlocked = true;
                    AutoClickerController.Instance?.Activate();
                    break;
                case ShopItemEffect.AutoClickerSpeedBoost:
                    autoClickerInterval = Mathf.Max(0.1f, autoClickerInterval - (float)entry.value);
                    AutoClickerController.Instance?.SetInterval(autoClickerInterval);
                    break;
                case ShopItemEffect.UnlockBulkBuy:
                    bulkBuyUnlocked = true;
                    break;
                case ShopItemEffect.UnlockAntInspector:
                    antInspectorUnlocked = true;
                    break;

                // Production
                case ShopItemEffect.GlobalAPSMultiplier:
                    foreach (var b in bm.Buildings)
                        b.ApplyAPSMultiplier(entry.value);
                    bm.RecalculateAPS();
                    break;
                case ShopItemEffect.ClickMultiplier:
                    cm.AddToAntsPerClick(cm.AntsPerClick * (entry.value - 1));
                    break;
                case ShopItemEffect.CriticalClickChance:
                    critClickChance = Mathf.Clamp01(critClickChance + (float)entry.value);
                    break;
                case ShopItemEffect.CriticalClickMultiplier:
                    critClickMultiplier += (float)entry.value;
                    break;
                case ShopItemEffect.ReduceBuildingCosts:
                    BuildingCostModifier.GlobalCostMultiplier *= (1.0 - entry.value);
                    break;
                case ShopItemEffect.FoodHoard:
                    cm.AddToAntsPerSecond(entry.value);
                    break;
                case ShopItemEffect.QueenFertility:
                    cm.AddToAntsPerClick(entry.value);
                    break;

                // Fun / Cosmetic
                case ShopItemEffect.UnlockFireAnts:
                    fireAntsUnlocked = true;
                    AntSpawnerExtensions.NotifyAntTypeUnlocked("fire");
                    break;
                case ShopItemEffect.UnlockCarpenterAnts:
                    carpenterAntsUnlocked = true;
                    AntSpawnerExtensions.NotifyAntTypeUnlocked("carpenter");
                    break;
                case ShopItemEffect.UnlockArmyAnts:
                    armyAntsUnlocked = true;
                    AntSpawnerExtensions.NotifyAntTypeUnlocked("army");
                    break;
                case ShopItemEffect.UnlockGoldenAnt:
                    goldenAntUnlocked = true;
                    GoldenAntController.Instance?.Activate();
                    break;
                case ShopItemEffect.UnlockAntHat:
                    antHatUnlocked = true;
                    AntSpawnerExtensions.NotifyAntTypeUnlocked("hat");
                    break;
                case ShopItemEffect.UnlockDiscoBall:
                    discoBallUnlocked = true;
                    DiscoBallController.Instance?.Activate();
                    break;
                case ShopItemEffect.UnlockRainbowTrails:
                    rainbowTrailsUnlocked = true;
                    break;
                case ShopItemEffect.UnlockNightMode:
                    nightModeUnlocked = true;
                    NightModeController.Instance?.Activate();
                    break;
                case ShopItemEffect.UnlockWeatherSystem:
                    weatherSystemUnlocked = true;
                    WeatherController.Instance?.Activate();
                    break;
                case ShopItemEffect.UnlockAntSongs:
                    antSongsUnlocked = true;
                    break;
            }
        }
    }

    // ── Save / Load ───────────────────────────────────────────────
    public void WriteToData(SaveData data)
    {
        var ids = new List<string>();
        foreach (var item in _items)
            if (item.Purchased) ids.Add(item.Data.id);
        data.purchasedShopItemIds = ids.ToArray();
    }

    public void LoadFromData(SaveData data)
    {
        var set = new HashSet<string>(data.purchasedShopItemIds ?? new string[0]);
        InitializeShop(set);
    }

    // ── Helpers ───────────────────────────────────────────────────
    public int PurchasedCount()
    {
        int n = 0;
        foreach (var i in _items) if (i.Purchased) n++;
        return n;
    }
}
