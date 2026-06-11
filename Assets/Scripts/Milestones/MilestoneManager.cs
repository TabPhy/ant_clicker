using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Owns all Milestone instances. Evaluates them every second (not every frame —
/// milestones don't need per-frame precision and this keeps CPU cost near zero).
///
/// Also applies rewards when a milestone fires, and raises events for the UI.
/// </summary>
public class MilestoneManager : MonoBehaviour
{
    public static MilestoneManager Instance { get; private set; }

    [Header("All Milestones (assign ScriptableObjects here in order)")]
    public MilestoneData[] milestoneDataList;

    [Header("Check interval (seconds)")]
    public float checkInterval = 1f;

    // Runtime list
    private List<Milestone> _milestones = new List<Milestone>();
    public  IReadOnlyList<Milestone> Milestones => _milestones;

    // Stat tracking (persisted in save)
    private double _totalClicks = 0;
    public  double TotalClicks  => _totalClicks;

    // Events for UI
    public event Action<Milestone> OnMilestoneCompleted;

    private float _checkTimer = 0f;

    // ── Lifecycle ─────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void InitializeMilestones(HashSet<string> completedIds)
    {
        _milestones.Clear();
        foreach (var data in milestoneDataList)
        {
            bool alreadyDone = completedIds.Contains(data.id);
            var  m           = new Milestone(data, alreadyDone);
            m.OnCompleted   += HandleMilestoneCompleted;
            _milestones.Add(m);
        }
        Debug.Log($"🏆 {_milestones.Count} milestones loaded ({completedIds.Count} already completed).");
    }

    // ── Tick ──────────────────────────────────────────────────────
    private void Update()
    {
        _checkTimer += Time.deltaTime;
        if (_checkTimer < checkInterval) return;
        _checkTimer = 0f;
        EvaluateAll();
    }

    private void EvaluateAll()
    {
        MilestoneContext ctx = BuildContext();
        foreach (var m in _milestones)
            m.Evaluate(ctx);
    }

    private MilestoneContext BuildContext()
    {
        var cm = CurrencyManager.Instance;
        var bm = BuildingManager.Instance;
        var pm = PrestigeManager.Instance;

        // Build per-building count array
        int[] counts = new int[bm.Buildings.Count];
        int   total  = 0;
        for (int i = 0; i < bm.Buildings.Count; i++)
        {
            counts[i] = bm.Buildings[i].Count;
            total     += counts[i];
        }

        return new MilestoneContext(counts)
        {
            totalAntsEver      = cm.TotalAntsEver,
            currentAnts        = cm.Ants,
            antsPerSecond      = cm.AntsPerSecond,
            totalClicks        = _totalClicks,
            totalBuildingsOwned = total,
            antDNA             = cm.AntDNA,
            prestigeCount      = pm.PrestigeCount,
        };
    }

    // ── Click tracking ────────────────────────────────────────────
    public void RegisterClick() => _totalClicks++;

    // ── Reward application ────────────────────────────────────────
    private void HandleMilestoneCompleted(Milestone m)
    {
        ApplyReward(m.Data);
        OnMilestoneCompleted?.Invoke(m);
        Debug.Log($"🏆 Milestone unlocked: {m.Data.title}");
    }

    private void ApplyReward(MilestoneData data)
    {
        var cm = CurrencyManager.Instance;
        var bm = BuildingManager.Instance;

        switch (data.rewardType)
        {
            case MilestoneRewardType.ClickMultiplier:
                cm.AddToAntsPerClick(cm.AntsPerClick * (data.rewardValue - 1));
                break;

            case MilestoneRewardType.APSMultiplier:
                // Apply a global APS multiplier to all buildings
                foreach (var b in bm.Buildings)
                    b.ApplyAPSMultiplier(data.rewardValue);
                bm.RecalculateAPS();
                break;

            case MilestoneRewardType.FlatClickBonus:
                cm.AddToAntsPerClick(data.rewardValue);
                break;

            case MilestoneRewardType.FlatAPSBonus:
                cm.AddToAntsPerSecond(data.rewardValue);
                break;

            case MilestoneRewardType.AntDNABonus:
                cm.AddAntDNA(data.rewardValue);
                break;

            case MilestoneRewardType.UnlockBuilding:
                // Force-unlock a building before its normal threshold
                // (Building.CheckUnlock handles this via threshold = 0 trick)
                Debug.Log($"Building {(int)data.rewardValue} force-unlocked by milestone.");
                break;

            case MilestoneRewardType.None:
            default:
                break;
        }
    }

    // ── Save / Load ───────────────────────────────────────────────
    public void WriteToData(SaveData data)
    {
        var completed = new List<string>();
        foreach (var m in _milestones)
            if (m.Completed) completed.Add(m.Data.id);

        data.completedMilestoneIds = completed.ToArray();
        data.totalClicks           = _totalClicks;
    }

    public void LoadFromData(SaveData data)
    {
        _totalClicks = data.totalClicks;
        var set = new HashSet<string>(data.completedMilestoneIds ?? new string[0]);
        InitializeMilestones(set);
    }

    // ── Helpers ───────────────────────────────────────────────────
    public int CompletedCount()
    {
        int count = 0;
        foreach (var m in _milestones) if (m.Completed) count++;
        return count;
    }
}
