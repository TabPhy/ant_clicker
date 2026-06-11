/// <summary>
/// What kind of value a milestone tracks.
/// Add new types here as the game grows.
/// </summary>
public enum MilestoneType
{
    TotalAntsEver,          // lifetime ants produced
    AntsAtOnce,             // current ant count
    AntsPerSecond,          // current APS
    TotalClicks,            // number of times the ant farm was clicked
    BuildingCount,          // total buildings owned (all types combined)
    SpecificBuildingCount,  // count of one specific building (use buildingIndex)
    PrestigeCount,          // number of times player has prestiged
    AntDNAOwned,            // current DNA balance
}
