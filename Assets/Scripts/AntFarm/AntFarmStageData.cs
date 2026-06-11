using UnityEngine;

/// <summary>
/// Defines one visual evolution stage of the ant farm background.
/// Create via right-click → AntClicker → Ant Farm Stage.
///
/// Stages unlock progressively as total-ants-ever crosses each threshold.
/// The background lerps/transitions between stages automatically.
/// </summary>
[CreateAssetMenu(fileName = "NewAntFarmStage", menuName = "AntClicker/Ant Farm Stage")]
public class AntFarmStageData : ScriptableObject
{
    [Header("Identity")]
    public string stageName = "Bare Dirt";

    [Header("Unlock Threshold")]
    [Tooltip("Unlocks when TotalAntsEver reaches this value")]
    public double unlockAtTotalAnts = 0;

    [Header("Background")]
    [Tooltip("Background sprite for this stage (the dirt/tunnel layer)")]
    public Sprite backgroundSprite;

    [Tooltip("Overlay sprite drawn on top of background (structures, foliage, etc.)")]
    public Sprite overlaySprite;

    [Tooltip("Sky/ambient colour for this stage")]
    public Color  skyColour        = new Color(0.53f, 0.81f, 0.98f);

    [Tooltip("Dirt/ground colour tint")]
    public Color  groundColour     = new Color(0.6f, 0.4f, 0.2f);

    [Header("Ants")]
    [Tooltip("Max ants visible on screen at this stage")]
    public int    maxVisibleAnts   = 5;

    [Tooltip("Ant walk speed at this stage")]
    public float  antWalkSpeed     = 40f;

    [Header("Structures")]
    [Tooltip("GameObjects to activate when this stage is reached (tunnels, buildings, etc.)")]
    public string[] structureObjectNames;

    [Header("Ambience")]
    [Tooltip("Particle system prefab name to activate (e.g. DirtDustParticles)")]
    public string ambientParticleName = "";

    [Tooltip("How fast ants carry food at this stage (cosmetic only)")]
    public float  antActivityLevel = 0.5f;  // 0–1
}
