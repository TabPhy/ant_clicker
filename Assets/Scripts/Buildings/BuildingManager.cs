using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Owns all Building instances, ticks passive income, and recalculates total APS.
/// Assign BuildingData ScriptableObjects in the Inspector.
/// </summary>
public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    [Header("Building Definitions (assign ScriptableObjects here)")]
    public BuildingData[] buildingDataList;

    // Runtime building list — populated from buildingDataList
    private List<Building> _buildings = new List<Building>();
    public  IReadOnlyList<Building> Buildings => _buildings;

    private double _totalAPS = 0;
    public  double TotalAPS  => _totalAPS;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void InitializeBuildings()
    {
        _buildings.Clear();
        foreach (var data in buildingDataList)
            _buildings.Add(new Building(data));

        RecalculateAPS();
    }

    /// <summary>Called every frame by GameManager.</summary>
    public void Tick(float deltaTime)
    {
        // Check unlocks
        double totalAnts = CurrencyManager.Instance.TotalAntsEver;
        foreach (var b in _buildings)
            b.CheckUnlock(totalAnts);

        // Passive income
        if (_totalAPS > 0)
            CurrencyManager.Instance.AddPassiveIncome(_totalAPS * deltaTime);
    }

    public bool PurchaseBuilding(int index)
    {
        if (index < 0 || index >= _buildings.Count) return false;
        bool success = _buildings[index].TryPurchase();
        if (success) RecalculateAPS();
        return success;
    }

    public void RecalculateAPS()
    {
        _totalAPS = 0;
        foreach (var b in _buildings)
            _totalAPS += b.CurrentAPS;

        CurrencyManager.Instance.SetAntsPerSecond(_totalAPS);
    }

    // ── Prestige reset ────────────────────────────────────────────
    public void ResetForPrestige()
    {
        foreach (var b in _buildings)
            b.ResetForPrestige();
        RecalculateAPS();
    }

    // ── Save / Load helpers ───────────────────────────────────────
    public void LoadFromData(SaveData data)
    {
        for (int i = 0; i < _buildings.Count && i < data.buildingCounts.Length; i++)
            _buildings[i].LoadCount(data.buildingCounts[i]);
        RecalculateAPS();
    }

    public void WriteToData(SaveData data)
    {
        data.buildingCounts = new int[_buildings.Count];
        for (int i = 0; i < _buildings.Count; i++)
            data.buildingCounts[i] = _buildings[i].Count;
    }
}
