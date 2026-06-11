using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines one building type (ScriptableObject — create via right-click in Project window).
/// Example buildings: Ant Egg, Ant Hill, Tunnel Network, Queen's Chamber, Underground City …
/// </summary>
[CreateAssetMenu(fileName = "NewBuilding", menuName = "AntClicker/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("Identity")]
    public string buildingName    = "Ant Egg";
    public string description     = "A humble beginning.";
    public Sprite icon;

    [Header("Economy")]
    public double baseCost        = 10;        // cost for first purchase
    public double baseAPS         = 0.1;       // ants-per-second contribution
    public double costGrowthRate  = 1.15;      // price multiplier per purchase

    [Header("Unlock")]
    public double unlockAtTotalAnts = 0;       // show building once player has earned this many ants ever
}
