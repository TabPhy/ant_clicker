/// <summary>
/// Snapshot of the game state passed to every Milestone.Evaluate() call.
/// Avoids calling manager singletons from inside the Milestone class.
/// </summary>
public class MilestoneContext
{
    public double totalAntsEver;
    public double currentAnts;
    public double antsPerSecond;
    public double totalClicks;
    public double totalBuildingsOwned;
    public double antDNA;
    public int    prestigeCount;

    // Per-building counts — index matches BuildingManager.Buildings[]
    private int[] _buildingCounts;

    public MilestoneContext(int[] buildingCounts)
    {
        _buildingCounts = buildingCounts ?? new int[0];
    }

    public int GetBuildingCount(int index)
        => (index >= 0 && index < _buildingCounts.Length) ? _buildingCounts[index] : 0;
}
