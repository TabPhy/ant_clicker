/// <summary>
/// What reward a milestone grants when it fires.
/// </summary>
public enum MilestoneRewardType
{
    None,                   // purely cosmetic / story flavour
    ClickMultiplier,        // multiplies ants-per-click
    APSMultiplier,          // multiplies all buildings' APS
    FlatClickBonus,         // adds a flat amount to ants-per-click
    FlatAPSBonus,           // adds flat APS on top of buildings
    AntDNABonus,            // awards DNA immediately
    UnlockBuilding,         // forces a building to become visible early
}
