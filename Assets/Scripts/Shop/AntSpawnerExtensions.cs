using UnityEngine;

/// <summary>
/// Static notification bridge between ShopManager and AntSpawner.
/// When a new ant type is unlocked, AntSpawner listens to this event
/// and starts mixing in the new sprites.
///
/// Usage: AntSpawnerExtensions.NotifyAntTypeUnlocked("fire");
/// In AntSpawner.Start(): AntSpawnerExtensions.OnAntTypeUnlocked += HandleNewAntType;
/// </summary>
public static class AntSpawnerExtensions
{
    public static event System.Action<string> OnAntTypeUnlocked;

    public static void NotifyAntTypeUnlocked(string antType)
    {
        OnAntTypeUnlocked?.Invoke(antType);
        Debug.Log($"🐜 New ant type unlocked: {antType}");
    }
}
