using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Runtime state for one building type.
/// Handles purchasing, APS contribution, and milestone upgrades.
/// </summary>
public class Building
{
    public BuildingData Data     { get; private set; }
    public int          Count    { get; private set; }
    public bool         Unlocked { get; private set; }

    // current APS this building produces (Count * baseAPS * all multipliers)
    public double CurrentAPS => Data.baseAPS * Count * _apsMultiplier;

    private double _apsMultiplier = 1.0;

    public Building(BuildingData data)
    {
        Data     = data;
        Count    = 0;
        Unlocked = data.unlockAtTotalAnts <= 0;
    }

    // cost of the NEXT purchase — respects global shop discounts
    public double NextCost()
        => Math.Floor(Data.baseCost
            * Math.Pow(Data.costGrowthRate, Count)
            * BuildingCostModifier.GlobalCostMultiplier);

    public bool TryPurchase()
    {
        double cost = NextCost();
        if (!CurrencyManager.Instance.SpendAnts(cost)) return false;
        Count++;
        return true;
    }

    public void ApplyAPSMultiplier(double multiplier) => _apsMultiplier *= multiplier;

    public void CheckUnlock(double totalAntsEver)
    {
        if (!Unlocked && totalAntsEver >= Data.unlockAtTotalAnts)
            Unlocked = true;
    }

    // ── Prestige reset ────────────────────────────────────────────
    public void ResetForPrestige()
    {
        Count          = 0;
        _apsMultiplier = 1.0;
        Unlocked       = Data.unlockAtTotalAnts <= 0;
    }

    // ── Save / Load ───────────────────────────────────────────────
    public void LoadCount(int count) => Count = count;
}
