using UnityEngine;

/// <summary>
/// Central game manager — owns game state and coordinates all systems.
/// Attach to a persistent "GameManager" GameObject in your first scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public CurrencyManager  currencyManager;
    public BuildingManager  buildingManager;
    public PrestigeManager  prestigeManager;
    public SaveManager      saveManager;
    public MilestoneManager milestoneManager;

    [Header("Game State")]
    public bool isGamePaused = false;

    private void Awake()
    {
        // Singleton — persists across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        saveManager.LoadGame();
        buildingManager.InitializeBuildings();
        // MilestoneManager.InitializeMilestones is called inside SaveManager.LoadGame
        Debug.Log("🐜 Ant Clicker — Game Started!");
    }

    private void Update()
    {
        if (!isGamePaused)
            buildingManager.Tick(Time.deltaTime);
    }

    // ── Click — routed through here so MilestoneManager can track it ──
    public void OnAntFarmClicked()
    {
        CurrencyManager.Instance.Click();
        MilestoneManager.Instance.RegisterClick();
    }

    public void PauseGame()  => isGamePaused = true;
    public void ResumeGame() => isGamePaused = false;

    private void OnApplicationPause(bool paused)
    {
        if (paused) saveManager.SaveGame();
    }

    private void OnApplicationQuit()
    {
        saveManager.SaveGame();
    }
}
