using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Spawns pooled floating "+N ants" labels that rise and fade.
/// Attach to an empty GameObject in your Canvas (Screen Space – Camera or World Space).
///
/// Setup:
///   - Create a TextMeshPro prefab (no background, centred anchor, ~28pt bold).
///   - Assign it to floatingTextPrefab.
///   - Assign the Canvas Transform to canvasParent.
///   - Set poolSize to ~20.
/// </summary>
public class FloatingTextSpawner : MonoBehaviour
{
    [Header("Prefab & Pool")]
    public GameObject floatingTextPrefab;
    public Transform  canvasParent;
    public int        poolSize = 20;

    [Header("Animation")]
    public float riseDistance = 80f;    // pixels to float upward
    public float duration     = 0.9f;   // seconds until fully faded
    public float spreadX      = 30f;    // random horizontal jitter

    private Queue<FloatingTextInstance> _pool = new Queue<FloatingTextInstance>();

    // ── Lifecycle ─────────────────────────────────────────────────
    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var go   = Instantiate(floatingTextPrefab, canvasParent);
            var inst = new FloatingTextInstance(go, go.GetComponent<TextMeshProUGUI>());
            go.SetActive(false);
            _pool.Enqueue(inst);
        }
    }

    // ── Public API ────────────────────────────────────────────────
    /// <summary>
    /// Spawn floating text at a world position.
    /// Converts world → screen → canvas automatically.
    /// </summary>
    public void Spawn(string text, Vector3 worldPos, Color? colour = null)
    {
        if (_pool.Count == 0) return;

        var inst = _pool.Dequeue();
        inst.Label.text  = text;
        inst.Label.color = colour ?? Color.white;

        // World → screen → canvas-local
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        screenPos.x += Random.Range(-spreadX, spreadX);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasParent as RectTransform,
            screenPos,
            null,
            out Vector2 localPos
        );

        var rt = inst.Go.GetComponent<RectTransform>();
        rt.anchoredPosition = localPos;

        inst.Go.SetActive(true);
        StartCoroutine(AnimateAndReturn(inst));
    }

    private IEnumerator AnimateAndReturn(FloatingTextInstance inst)
    {
        var    rt        = inst.Go.GetComponent<RectTransform>();
        float  elapsed   = 0f;
        Vector2 startPos = rt.anchoredPosition;
        Color   startCol = inst.Label.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t  = elapsed / duration;

            rt.anchoredPosition = startPos + Vector2.up * (riseDistance * t);
            inst.Label.color    = new Color(startCol.r, startCol.g, startCol.b, 1f - t);
            yield return null;
        }

        inst.Go.SetActive(false);
        _pool.Enqueue(inst);
    }

    // ── Inner type ────────────────────────────────────────────────
    private class FloatingTextInstance
    {
        public GameObject      Go    { get; }
        public TextMeshProUGUI Label { get; }
        public FloatingTextInstance(GameObject go, TextMeshProUGUI label)
        { Go = go; Label = label; }
    }
}
