using UnityEngine;

/// <summary>
/// Defines one milestone / achievement (ScriptableObject).
/// Create via right-click → AntClicker → Milestone Data.
///
/// Milestones are one-time: they fire once, apply their reward, and stay "completed".
/// They survive prestige resets (stored in save data by ID).
/// </summary>
[CreateAssetMenu(fileName = "NewMilestone", menuName = "AntClicker/Milestone Data")]
public class MilestoneData : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique string ID used in save data. Never change after shipping.")]
    public string id = "milestone_unique_id";

    public string title       = "First Steps";
    public string description = "Spawn your very first ant.";
    public string flavourText = "\"Every empire begins with a single grain of sand.\"";
    public Sprite icon;

    [Header("Trigger")]
    public MilestoneType trackingType   = MilestoneType.TotalAntsEver;
    public double        threshold      = 1;

    [Tooltip("Only used when trackingType == SpecificBuildingCount")]
    public int           buildingIndex  = 0;

    [Header("Reward")]
    public MilestoneRewardType rewardType  = MilestoneRewardType.None;
    public double              rewardValue = 0;   // multiplier (e.g. 1.1 = +10%), flat value, or building index

    [Header("Display")]
    [Tooltip("Show a toast/popup when this fires (uncheck for minor milestones)")]
    public bool showPopup = true;

    [Tooltip("Show in the achievements panel even before it is unlocked")]
    public bool visibleBeforeUnlock = true;
}
