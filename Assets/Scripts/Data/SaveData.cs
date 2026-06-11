using System;

/// <summary>
/// Plain serializable data container — everything that gets written to disk.
/// No MonoBehaviour, no Unity dependencies.
/// </summary>
[Serializable]
public class SaveData
{
    // ── Currency ──────────────────────────────────────────────────
    public double ants          = 0;
    public double totalAntsEver = 0;
    public double antsPerClick  = 1;
    public double antDNA        = 0;

    // ── Buildings ─────────────────────────────────────────────────
    public int[] buildingCounts = Array.Empty<int>();

    // ── Prestige ──────────────────────────────────────────────────
    public int prestigeCount = 0;

    // ── Milestones & Achievements ─────────────────────────────────
    public string[] completedMilestoneIds = Array.Empty<string>();
    public double   totalClicks           = 0;

    // ── Offline progress ─────────────────────────────────────────
    public long lastSaveTimestamp = 0;   // Unix time (seconds)

    // ── Meta ──────────────────────────────────────────────────────
    public string saveVersion = "1.1";
}
