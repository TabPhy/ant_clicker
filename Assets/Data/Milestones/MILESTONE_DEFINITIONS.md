# Milestone ScriptableObject Definitions
# =========================================
# Unity can't create ScriptableObjects from plain text, but this file
# is your authoritative design sheet. When Unity is installed:
#   1. Right-click in Project → AntClicker → Milestone Data
#   2. Fill in the fields below per row
#   3. Drag all assets into MilestoneManager.milestoneDataList[] IN ORDER
#
# Columns: id | title | description | flavourText | trackingType | threshold | buildingIndex | rewardType | rewardValue | showPopup | visibleBeforeUnlock

# ── FIRST STEPS ────────────────────────────────────────────────────────────────
# id                          title                     description                              trackingType        threshold    rewardType          rewardValue
first_ant                    "First Ant"               "Spawn your very first ant."             TotalAntsEver       1            None                0
ten_ants                     "Ten Ants"                "Reach 10 ants."                         TotalAntsEver       10           FlatClickBonus      1
hundred_ants                 "Century Colony"          "Reach 100 ants total."                  TotalAntsEver       100          FlatClickBonus      5
first_click                  "Clicker"                 "Click the ant farm for the first time." TotalClicks         1            None                0
hundred_clicks               "Dedicated Clicker"       "Click 100 times."                       TotalClicks         100          ClickMultiplier     1.1
thousand_clicks              "Obsessed"                "Click 1,000 times."                     TotalClicks         1000         ClickMultiplier     1.1
ten_thousand_clicks          "Mandible Workout"        "Click 10,000 times."                    TotalClicks         10000        ClickMultiplier     1.25

# ── EARLY COLONY ───────────────────────────────────────────────────────────────
first_thousand               "Thousand March"          "Produce 1,000 ants ever."               TotalAntsEver       1000         FlatAPSBonus        1
first_building               "Construction Crew"       "Buy your first building."               BuildingCount       1            None                0
five_buildings               "Expanding Fast"          "Own 5 buildings total."                 BuildingCount       5            APSMultiplier       1.05
ten_buildings                "Busy Colony"             "Own 10 buildings total."                BuildingCount       10           APSMultiplier       1.1
first_anthill                "Anthill Achieved"        "Build your first Ant Hill."             SpecificBuilding    1            FlatAPSBonus        2
first_aps                    "Passive Income"          "Reach 1 ant per second."                AntsPerSecond       1            None                0
ten_aps                      "Production Line"         "Reach 10 ants per second."              AntsPerSecond       10           FlatClickBonus      10
hundred_aps                  "Factory Floor"           "Reach 100 ants per second."             AntsPerSecond       100          APSMultiplier       1.1

# ── GROWING EMPIRE ─────────────────────────────────────────────────────────────
million_ants                 "Millionaire Colony"      "Produce 1 Million ants ever."           TotalAntsEver       1000000      ClickMultiplier     1.5
billion_ants                 "Billionaire Queen"       "Produce 1 Billion ants ever."           TotalAntsEver       1000000000   APSMultiplier       1.25
first_tunnel                 "Underground Railroad"    "Build your first Tunnel Network."       SpecificBuilding    1            FlatAPSBonus        10
first_queens_chamber         "The Queen Awakens"       "Build a Queen's Chamber."               SpecificBuilding    1            APSMultiplier       1.15
twenty_five_buildings        "Urban Sprawl"            "Own 25 buildings total."                BuildingCount       25           APSMultiplier       1.1
fifty_buildings              "Mega Colony"             "Own 50 buildings total."                BuildingCount       50           APSMultiplier       1.2
thousand_aps                 "Unstoppable"             "Reach 1,000 ants per second."           AntsPerSecond       1000         AntDNABonus         1
ten_thousand_aps             "Ant Superpower"          "Reach 10,000 ants per second."          AntsPerSecond       10000        AntDNABonus         5

# ── CIVILISATION ───────────────────────────────────────────────────────────────
underground_city             "City Below"              "Build your first Underground City."     SpecificBuilding    1            APSMultiplier       1.2
ant_civilisation             "We Are Civilised"        "Build an Ant Civilisation."             SpecificBuilding    1            ClickMultiplier     2.0
ant_parliament               "Democracy of Ants"       "Establish an Ant Parliament."           SpecificBuilding    1            AntDNABonus         10
trillion_ants                "Trillion-Ant Empire"     "Produce 1 Trillion ants ever."          TotalAntsEver       1e12         APSMultiplier       1.5
hundred_buildings            "Infinite Colony"         "Own 100 buildings total."               BuildingCount       100          APSMultiplier       1.3
million_aps                  "Ant Singularity"         "Reach 1 Million ants per second."       AntsPerSecond       1000000      AntDNABonus         50

# ── SPACE RACE ─────────────────────────────────────────────────────────────────
ant_nasa                     "One Small Step"          "Build Ant NASA."                        SpecificBuilding    1            AntDNABonus         25
moon_rocket                  "T-Minus Everything"      "Build the Moon Rocket."                 SpecificBuilding    1            AntDNABonus         100
ready_for_moon               "Ad Astra, Ant Astra"     "Reach 1 Billion ants ever."             TotalAntsEver       1e9          None                0

# ── PRESTIGE MILESTONES ────────────────────────────────────────────────────────
first_prestige               "First Moon Landing"      "Fly to the Moon for the first time."   PrestigeCount       1            ClickMultiplier     2.0
five_prestiges               "Frequent Flyer"          "Fly to the Moon 5 times."              PrestigeCount       5            APSMultiplier       2.0
ten_prestiges                "Moon Regular"            "Fly to the Moon 10 times."             PrestigeCount       10           AntDNABonus         500
first_dna                    "Double Helix"            "Earn your first Ant DNA."              AntDNAOwned         1            None                0
hundred_dna                  "DNA Rich"                "Accumulate 100 Ant DNA."               AntDNAOwned         100          ClickMultiplier     1.5
thousand_dna                 "DNA Millionaire"         "Accumulate 1,000 Ant DNA."             AntDNAOwned         1000         APSMultiplier       2.0
