using UnityEngine;
using System.Collections;

/// <summary>
/// Handles click feedback on the ant farm:
///   1. "+N ants" floating text that rises and fades.
///   2. A quick "squish" scale punch on the click target.
///   3. Tells AntSpawner to burst a few extra ants near the tap.
///
/// Attach to the clickable ant farm collider/button GameObject.
/// Requires a 2D collider (or a UI Button — see alternative setup below).
/// </summary>
public class AntFarmClickHandler : MonoBehaviour
{
    [Header("References")]
    public AntSpawner           antSpawner;
    public FloatingTextSpawner  floatingTextSpawner;

    [Header("Click Punch")]
    [Tooltip("Transform to punch-scale on click (the ant farm sprite)")]
    public Transform punchTarget;
    public float     punchScale    = 1.15f;
    public float     punchDuration = 0.1f;

    private Vector3 _originalScale;

    private void Start()
    {
        if (punchTarget != null)
            _originalScale = punchTarget.localScale;
    }

    // ── Called by a UI Button's OnClick OR by OnMouseDown ────────
    public void OnClick()
    {
        // 1. Add currency (routed through GameManager so clicks are tracked)
        GameManager.Instance.OnAntFarmClicked();

        // 2. Floating text
        Vector3 worldPos = GetClickWorldPosition();
        double  gained   = CurrencyManager.Instance.AntsPerClick;
        floatingTextSpawner?.Spawn("+" + BigNumberFormatter.FormatShort(gained), worldPos);

        // 3. Burst ants near click
        antSpawner?.OnFarmClicked(worldPos);

        // 4. Scale punch
        if (punchTarget != null)
        {
            StopAllCoroutines();
            StartCoroutine(PunchScale());
        }
    }

    private Vector3 GetClickWorldPosition()
    {
        // Works for both screen-space UI and world-space clicks
        if (Camera.main != null)
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return transform.position;
    }

    private IEnumerator PunchScale()
    {
        float   elapsed   = 0f;
        Vector3 bigScale  = _originalScale * punchScale;

        // Scale up
        while (elapsed < punchDuration)
        {
            elapsed               += Time.deltaTime;
            punchTarget.localScale = Vector3.Lerp(_originalScale, bigScale, elapsed / punchDuration);
            yield return null;
        }

        elapsed = 0f;
        // Scale back
        while (elapsed < punchDuration)
        {
            elapsed               += Time.deltaTime;
            punchTarget.localScale = Vector3.Lerp(bigScale, _originalScale, elapsed / punchDuration);
            yield return null;
        }

        punchTarget.localScale = _originalScale;
    }

    // ── Alternative: Unity Physics2D click detection ──────────────
    private void OnMouseDown()
    {
        OnClick();
    }
}
