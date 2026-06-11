using System;

/// <summary>
/// Runtime wrapper around MilestoneData — tracks completion state.
/// </summary>
public class Milestone
{
    public MilestoneData Data      { get; private set; }
    public bool          Completed { get; private set; }

    public event Action<Milestone> OnCompleted;

    public Milestone(MilestoneData data, bool alreadyCompleted = false)
    {
        Data      = data;
        Completed = alreadyCompleted;
    }

    /// <summary>
    /// Check whether the current game state satisfies this milestone's threshold.
    /// Called every frame by MilestoneManager.
    /// </summary>
    public void Evaluate(MilestoneContext ctx)
    {
        if (Completed) return;

        double current = GetCurrentValue(ctx);
        if (current >= Data.threshold)
            Complete();
    }

    private double GetCurrentValue(MilestoneContext ctx)
    {
        return Data.trackingType switch
        {
            MilestoneType.TotalAntsEver         => ctx.totalAntsEver,
            MilestoneType.AntsAtOnce            => ctx.currentAnts,
            MilestoneType.AntsPerSecond         => ctx.antsPerSecond,
            MilestoneType.TotalClicks           => ctx.totalClicks,
            MilestoneType.BuildingCount         => ctx.totalBuildingsOwned,
            MilestoneType.SpecificBuildingCount => ctx.GetBuildingCount(Data.buildingIndex),
            MilestoneType.PrestigeCount         => ctx.prestigeCount,
            MilestoneType.AntDNAOwned           => ctx.antDNA,
            _                                   => 0,
        };
    }

    private void Complete()
    {
        Completed = true;
        OnCompleted?.Invoke(this);
    }

    /// <summary>Force-complete (used when loading a save).</summary>
    public void MarkCompleted() => Completed = true;
}
