# Ant Farm Background — Scene Setup Guide
==========================================

## Layer Order (Z depth, back to front)
```
-10  Sky / Camera background colour (set per stage via AntFarmStageData.skyColour)
 -5  Background sprite  (dirt layer — backgroundRenderer)
 -4  Background scroller objects (clouds, floating leaves)
 -3  Overlay sprite     (tunnel outlines, surface details — overlayRenderer)
 -2  Structures         (ant hill, tunnels, buildings, rocket pad — managed by StructureManager)
 -1  Ant agents         (walking ants — managed by AntSpawner)
  0  VFX particles      (engine exhaust, dirt puffs)
 10  UI Canvas          (HUD, shop, floating text)
```

---

## Scene Hierarchy
```
[Scene Root]
│
├── GameManager                      ← GameManager.cs, CurrencyManager.cs,
│                                       BuildingManager.cs, PrestigeManager.cs,
│                                       SaveManager.cs, MilestoneManager.cs
│
├── AntFarmRoot
│   ├── AntFarmBackground            ← AntFarmBackground.cs
│   ├── StructureManager             ← StructureManager.cs
│   ├── AntSpawner                   ← AntSpawner.cs
│   │   └── AntPool (empty parent)   ← spawnParent reference
│   │
│   ├── BackgroundLayer              ← SpriteRenderer (backgroundRenderer)
│   ├── OverlayLayer                 ← SpriteRenderer (overlayRenderer)
│   │
│   ├── Structures (all start INACTIVE)
│   │   ├── AntHill                  ← id: "AntHill"       + IdleAnimator (Bob)
│   │   ├── TunnelNetwork            ← id: "TunnelNetwork"
│   │   ├── QueensChamber            ← id: "QueensChamber" + IdleAnimator (Breathe)
│   │   ├── UndergroundCity          ← id: "UndergroundCity"
│   │   ├── AntCivilisation          ← id: "AntCivilisation"
│   │   ├── AntParliament            ← id: "AntParliament"
│   │   ├── AntNASA                  ← id: "AntNASA"
│   │   └── MoonRocketPad            ← id: "MoonRocketPad"
│   │
│   ├── Scrollers
│   │   ├── Cloud_1                  ← BackgroundScroller (Horizontal, speed 15)
│   │   ├── Cloud_2                  ← BackgroundScroller (Horizontal, speed 22)
│   │   └── DirtDust                 ← BackgroundScroller (Vertical, speed -8)
│   │
│   └── RocketLaunchSequencer        ← RocketLaunchSequencer.cs
│       ├── RocketSprite             ← the rocket GameObject (starts hidden)
│       └── EngineParticles          ← ParticleSystem
│
└── Canvas (Screen Space – Camera)
    ├── HUD                          ← HUDController.cs
    ├── ShopPanel                    ← BuildingShopRow × 9
    ├── AchievementsPanel            ← AchievementsPanelController.cs
    ├── PrestigePanel                ← PrestigeUIController.cs
    ├── FloatingTextLayer            ← FloatingTextSpawner.cs
    ├── MilestoneToast               ← MilestoneToastController.cs
    └── FlashPanel                   ← Full-screen white Image, alpha=0
```

---

## AntFarmStageData — Recommended Stages
Create 9 ScriptableObjects (right-click → AntClicker → Ant Farm Stage):

| # | Name               | unlockAtTotalAnts | maxVisibleAnts | antWalkSpeed | skyColour        | Structures to activate                        |
|---|--------------------|-------------------|----------------|--------------|------------------|-----------------------------------------------|
| 0 | Bare Dirt          | 0                 | 3              | 30           | light blue       | (none)                                        |
| 1 | First Hill         | 100               | 8              | 35           | light blue       | AntHill                                       |
| 2 | Tunnels Dug        | 5,000             | 15             | 40           | warm blue        | TunnelNetwork                                 |
| 3 | Queen Awakens      | 50,000            | 25             | 50           | golden afternoon | QueensChamber                                 |
| 4 | Underground City   | 1,000,000         | 40             | 60           | amber dusk       | UndergroundCity                               |
| 5 | Ant Civilisation   | 50,000,000        | 60             | 70           | purple twilight  | AntCivilisation                               |
| 6 | Ant Parliament     | 500,000,000       | 80             | 80           | deep indigo      | AntParliament                                 |
| 7 | Ant NASA           | 1e10              | 90             | 90           | dark navy        | AntNASA                                       |
| 8 | Moon Launch Ready  | 1e11              | 100            | 100          | space black      | MoonRocketPad                                 |

---

## AntAgent Prefab Setup
```
AntPrefab (GameObject)
  ├── SpriteRenderer      ← attach sprites: spriteIdle, spriteWalking, spriteCarrying
  ├── AntAgent.cs
  └── (optional) IdleAnimator.cs with type=Wiggle, magnitude=1, speed=3
```
Sprite tips:
- Walking ant: 3–4 frame sprite sheet animated via Animator, or a single tilted sprite
- Carrying ant: same ant with a tiny food crumb sprite parented above it
- Scale: ~12×8px in world units works well for a 1080p portrait layout

---

## FloatingTextPrefab Setup
```
FloatingText (GameObject — no background)
  └── TextMeshPro (UGUI)
        Font: bold, ~28pt
        Colour: white with black outline (Material override)
        Alignment: Centre
        RectTransform: 200×60, anchor centre
```

---

## StructureManager — Inspector Setup
After creating the structure GameObjects in the scene, add entries to StructureManager.structures[]:

| index | id               | structureObject   | animateOnActivate |
|-------|------------------|-------------------|-------------------|
| 0     | AntHill          | AntHill (GO ref)  | true              |
| 1     | TunnelNetwork    | TunnelNetwork     | true              |
| 2     | QueensChamber    | QueensChamber     | true              |
| 3     | UndergroundCity  | UndergroundCity   | true              |
| 4     | AntCivilisation  | AntCivilisation   | true              |
| 5     | AntParliament    | AntParliament     | true              |
| 6     | AntNASA          | AntNASA           | true              |
| 7     | MoonRocketPad    | MoonRocketPad     | true              |

---

## Rocket Launch Sequencer — Inspector Setup
| Field             | Assign                                         |
|-------------------|------------------------------------------------|
| rocketTransform   | The rocket sprite GameObject                   |
| countdownText     | TMP label for "3 2 1 🚀" (centred, large font) |
| flashPanel        | Full-screen white Image on top Canvas layer    |
| engineParticles   | ParticleSystem child of the rocket             |
| launchAudioSource | AudioSource with rocket whoosh/rumble clip      |
