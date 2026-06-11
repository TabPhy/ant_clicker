/// <summary>
/// Global cost modifier for buildings — reduced by shop items.
/// Static so it can be read from Building.NextCost() without a manager reference.
/// </summary>
public static class BuildingCostModifier
{
    /// <summary>Multiplied into every building's next cost. Starts at 1.0 (no discount).</summary>
    public static double GlobalCostMultiplier { get; set; } = 1.0;
}
