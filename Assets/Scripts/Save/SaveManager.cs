using UnityEngine;
using System;
using System.IO;

/// <summary>
/// Saves and loads game state to/from a JSON file on disk.
/// Also handles offline income calculation on load.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("Max hours of offline income to award (prevents abuse)")]
    public float maxOfflineHours = 12f;

    private string SavePath => Path.Combine(Application.persistentDataPath, "antclicker_save.json");

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Save ──────────────────────────────────────────────────────
    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.lastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        CurrencyManager.Instance.WriteToData(data);
        BuildingManager.Instance.WriteToData(data);
        PrestigeManager.Instance.WriteToData(data);
        MilestoneManager.Instance.WriteToData(data);

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"💾 Game saved to {SavePath}");
    }

    // ── Load ──────────────────────────────────────────────────────
    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found — starting fresh.");
            // Still need to initialise milestones with empty completed set
            MilestoneManager.Instance.LoadFromData(new SaveData());
            return;
        }

        try
        {
            string json   = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            CurrencyManager.Instance.LoadFromData(data);
            BuildingManager.Instance.LoadFromData(data);
            PrestigeManager.Instance.LoadFromData(data);
            MilestoneManager.Instance.LoadFromData(data);

            AwardOfflineIncome(data);
            Debug.Log("✅ Game loaded successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load save: {e.Message}");
            MilestoneManager.Instance.LoadFromData(new SaveData());
        }
    }

    // ── Offline income ────────────────────────────────────────────
    private void AwardOfflineIncome(SaveData data)
    {
        if (data.lastSaveTimestamp <= 0) return;

        long now         = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long secondsAway = now - data.lastSaveTimestamp;
        long maxSeconds  = (long)(maxOfflineHours * 3600f);
        long capped      = Math.Min(secondsAway, maxSeconds);

        if (capped <= 0) return;

        double income = BuildingManager.Instance.TotalAPS * capped;
        if (income <= 0) return;

        CurrencyManager.Instance.AddAnts(income);
        Debug.Log($"⏰ Offline income: +{BigNumberFormatter.Format(income)} ants ({capped}s away)");
    }

    // ── Delete save (debug / settings) ───────────────────────────
    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("🗑️ Save file deleted.");
        }
    }
}
