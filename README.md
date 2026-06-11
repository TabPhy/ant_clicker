# 🐜 Ant Clicker

An idle clicker game where you build ant civilisations — from a single egg all the way to the moon.

---

## Project Structure

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs          — Singleton orchestrator, game loop
│   ├── CurrencyManager.cs      — Ant & DNA currency, click handler
│   └── BigNumberFormatter.cs   — Formats huge numbers (1.23 Billion etc.)
├── Buildings/
│   ├── Building.cs             — Runtime state for one building type
│   └── BuildingManager.cs      — Owns all buildings, ticks passive APS income
├── Prestige/
│   └── PrestigeManager.cs      — "Fly to the Moon" reset + DNA bonuses
├── Save/
│   └── SaveManager.cs          — JSON save/load + offline income
├── UI/
│   ├── HUDController.cs        — Top HUD (ant count, APS, click button)
│   ├── BuildingShopRow.cs      — One row in the building shop panel
│   └── PrestigeUIController.cs — Prestige button + confirmation panel
└── Data/
    ├── BuildingData.cs         — ScriptableObject definition for buildings
    └── SaveData.cs             — Plain serialisable save container
```

---

## Setup in Unity

### 1. Create a new 2D (URP) project and open it

### 2. Install required packages (via Package Manager)
- **TextMeshPro** — for all text labels
- **DOTween** (Asset Store, free) — for animations

### 3. Set up the Scene hierarchy

```
[Scene]
 └── GameManager          (Empty GameObject)
      ├── GameManager.cs
      ├── CurrencyManager.cs
      ├── BuildingManager.cs
      ├── PrestigeManager.cs
      └── SaveManager.cs

 └── Canvas (Screen Space – Overlay)
      ├── HUD              → HUDController.cs
      ├── ShopPanel        → BuildingShopRow.cs (one per building)
      └── PrestigePanel    → PrestigeUIController.cs
```

### 4. Create BuildingData ScriptableObjects
Right-click in Project → **Create → AntClicker → Building Data**

Create one per building tier:

| # | Name                | Base Cost  | Base APS | Unlock At |
|---|---------------------|------------|----------|-----------|
| 0 | Ant Egg             | 10         | 0.1      | 0         |
| 1 | Ant Hill            | 100        | 0.5      | 50        |
| 2 | Tunnel Network      | 1,000      | 3        | 500       |
| 3 | Queen's Chamber     | 10,000     | 15       | 5,000     |
| 4 | Underground City    | 100,000    | 80       | 50,000    |
| 5 | Ant Civilisation    | 1,000,000  | 400      | 500,000   |
| 6 | Ant Parliament      | 10,000,000 | 2,000    | 5,000,000 |
| 7 | Ant NASA            | 1e9        | 10,000   | 1e8       |
| 8 | Moon Rocket         | 1e12       | 50,000   | 1e11      |

Assign all ScriptableObjects to `BuildingManager.buildingDataList[]` in the Inspector.

### 5. Wire up UI references
- Drag `HUDController` onto your HUD panel; assign TextMeshPro labels and the click Button
- Duplicate a `BuildingShopRow` prefab for each building; set the `buildingIndex` field
- Assign `PrestigeUIController` to your prestige panel; wire all buttons

---

## Prestige ("Fly to the Moon")
- Unlocks once you've **ever earned 1 Billion ants** (configurable in `PrestigeManager`)
- Resets: ants, buildings, APS
- Keeps: Ant DNA, prestige count
- DNA earned = `floor( sqrt(totalAntsEver / 1,000,000) )`
- DNA bonus: +2% click power per DNA (via sqrt scaling)
- Future ideas: unlock new biomes, ant types, cosmetics per prestige

---

## Save System
- Saves automatically on app pause/quit
- JSON stored at `Application.persistentDataPath/antclicker_save.json`
- Offline income: capped at 12 hours of APS production
- Delete save: call `SaveManager.Instance.DeleteSave()` (hook to a settings button)

---

## Monetisation (implement in Phase 4)
| Type              | Implementation                                         |
|-------------------|--------------------------------------------------------|
| Rewarded Ads      | "Watch ad → 2hr offline income now" button in HUD     |
| Remove Ads IAP    | $2.99 one-time — hide banner/interstitial ads          |
| Starter Pack      | $0.99 — small DNA + ants boost on first session        |
| Cosmetic IAPs     | Golden ants, glowing tunnels, alternate ant skins      |
| Speed Boost IAPs  | 2× production for 24 hrs                               |

---

## Roadmap
- [x] Core currency system
- [x] Building system with APS
- [x] Prestige / DNA system  
- [x] Save / load + offline income
- [x] Basic UI controllers
- [ ] Animated ant farm background (evolves with progress)
- [ ] Milestone popups & achievement system
- [ ] Rewarded ads integration (AdMob SDK)
- [ ] IAP integration (Unity IAP)
- [ ] Particle effects on click
- [ ] Rocket launch prestige animation
- [ ] App Store / Play Store submission
