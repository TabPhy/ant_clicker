/// <summary>
/// Every possible effect a shop item can have on the game.
/// Items can apply multiple effects — each ShopItemData holds a list of these.
/// </summary>
public enum ShopItemEffect
{
    // ── Quality of Life ───────────────────────────────────────────
    IncreaseOfflineIncomeHours,     // extends max offline cap (e.g. 12h → 24h)
    AutoSaveIntervalReduction,      // saves more frequently
    DoubleOfflineIncome,            // multiplies offline income rate
    ShowExactNumbers,               // toggles precise big-number display in HUD
    UnlockAutoClicker,              // periodically clicks the farm automatically
    AutoClickerSpeedBoost,          // makes auto-clicker faster
    UnlockBulkBuy,                  // enables ×10 / ×100 buy buttons in shop
    UnlockAntInspector,             // tap any ant to see its "stats" (fun flavour)

    // ── Production Boosts ─────────────────────────────────────────
    GlobalAPSMultiplier,            // multiplies all buildings' APS
    ClickMultiplier,                // multiplies ants-per-click
    CriticalClickChance,            // % chance a click is worth ×5
    CriticalClickMultiplier,        // changes the ×5 crit value
    ReduceBuildingCosts,            // all building prices cheaper by X%
    FoodHoard,                      // flat APS bonus (flavour: ants found food stash)
    QueenFertility,                 // flat click bonus (flavour: queen lays faster)

    // ── Fun / Cosmetic ────────────────────────────────────────────
    UnlockFireAnts,                 // adds red fire ant sprites to the pool
    UnlockCarpenterAnts,            // adds large carpenter ant sprites
    UnlockArmyAnts,                 // ants march in formation visually
    UnlockGoldenAnt,                // rare golden ant wanders the farm (clickable for bonus)
    UnlockAntHat,                   // ants wear tiny top hats (cosmetic)
    UnlockDiscoBall,                // disco ball appears at "party" milestone (cosmetic)
    UnlockRainbowTrails,            // ants leave rainbow trails
    UnlockNightMode,                // toggles night-time visual on the farm
    UnlockWeatherSystem,            // random weather events (rain, sun, thunder)
    UnlockAntSongs,                 // ants "sing" (audio blip) on click
}
