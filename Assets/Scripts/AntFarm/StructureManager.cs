using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages all Structure GameObjects in the ant farm scene.
/// Structures are child GameObjects (tunnels, buildings, the moon rocket, etc.)
/// that appear/grow when certain stages are reached.
///
/// How it works:
///   - All structure GameObjects start INACTIVE in the scene.
///   - AntFarmBackground calls StructureManager.ActivateStructuresForStage().
///   - This script activates them with a "pop in" scale animation.
///
/// Setup:
///   - Attach to a "StructureManager" GameObject.
///   - Populate the structures list in the Inspector, giving each a unique id
///     that matches what you put in AntFarmStageData.structureObjectNames[].
/// </summary>
public class StructureManager : MonoBehaviour
{
    public static StructureManager Instance { get; private set; }

    [System.Serializable]
    public class StructureEntry
    {
        [Tooltip("Must match the id in AntFarmStageData.structureObjectNames[]")]
        public string      id;
        public GameObject  structureObject;
        [Tooltip("Play a pop-in animation when this structure activates")]
        public bool        animateOnActivate = true;
    }

    [Header("All Structures")]
    public List<StructureEntry> structures = new List<StructureEntry>();

    [Header("Pop-in Animation")]
    public float popDuration  = 0.35f;
    public float overshoot    = 1.2f;   // peak scale before settling at 1

    private HashSet<string> _activated = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Make sure all start hidden
        foreach (var e in structures)
            if (e.structureObject != null)
                e.structureObject.SetActive(false);
    }

    // ── Called by AntFarmBackground on stage transition ───────────
    public void ActivateStructuresForStage(AntFarmStageData stage)
    {
        if (stage.structureObjectNames == null) return;

        foreach (string id in stage.structureObjectNames)
        {
            if (_activated.Contains(id)) continue;  // already shown

            StructureEntry entry = structures.Find(e => e.id == id);
            if (entry == null || entry.structureObject == null) continue;

            _activated.Add(id);

            if (entry.animateOnActivate)
                StartCoroutine(PopIn(entry.structureObject));
            else
                entry.structureObject.SetActive(true);
        }
    }

    // ── Pop-in scale coroutine ────────────────────────────────────
    private IEnumerator PopIn(GameObject go)
    {
        go.SetActive(true);
        Transform t = go.transform;
        Vector3 originalScale = t.localScale;
        t.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed      += Time.deltaTime;
            float progress = elapsed / popDuration;

            // Ease out with overshoot (simple approximation)
            float scale = progress < 0.7f
                ? Mathf.Lerp(0f, overshoot, progress / 0.7f)
                : Mathf.Lerp(overshoot, 1f, (progress - 0.7f) / 0.3f);

            t.localScale = originalScale * scale;
            yield return null;
        }

        t.localScale = originalScale;
    }

    // ── Prestige reset — hide all structures ─────────────────────
    public void ResetStructures()
    {
        StopAllCoroutines();
        _activated.Clear();
        foreach (var e in structures)
            if (e.structureObject != null)
                e.structureObject.SetActive(false);
    }
}
