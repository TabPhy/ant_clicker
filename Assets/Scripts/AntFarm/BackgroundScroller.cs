using UnityEngine;

/// <summary>
/// Loops a sprite (clouds, falling dirt particles, floating food crumbs)
/// horizontally or vertically across the ant farm background.
/// Attach to any background element you want to scroll infinitely.
///
/// Usage examples:
///   - Clouds drifting right across the sky layer
///   - Dirt dust particles drifting down
///   - Leaves blowing past at surface stages
/// </summary>
public class BackgroundScroller : MonoBehaviour
{
    public enum ScrollAxis { Horizontal, Vertical }

    [Header("Scroll Settings")]
    public ScrollAxis axis       = ScrollAxis.Horizontal;
    public float      speed      = 20f;        // units per second
    public float      wrapAtMax  = 500f;       // X (or Y) position to wrap back
    public float      wrapToMin  = -500f;      // position after wrap

    [Header("Randomise on Start")]
    [Tooltip("Randomly offset start position so multiple scrollers don't sync up")]
    public bool randomiseStartPosition = true;

    private void Start()
    {
        if (randomiseStartPosition)
        {
            Vector3 pos = transform.position;
            if (axis == ScrollAxis.Horizontal)
                pos.x = Random.Range(wrapToMin, wrapAtMax);
            else
                pos.y = Random.Range(wrapToMin, wrapAtMax);
            transform.position = pos;
        }
    }

    private void Update()
    {
        Vector3 pos = transform.position;

        if (axis == ScrollAxis.Horizontal)
        {
            pos.x += speed * Time.deltaTime;
            if (pos.x > wrapAtMax) pos.x = wrapToMin;
            if (pos.x < wrapToMin) pos.x = wrapAtMax;
        }
        else
        {
            pos.y += speed * Time.deltaTime;
            if (pos.y > wrapAtMax) pos.y = wrapToMin;
            if (pos.y < wrapToMin) pos.y = wrapAtMax;
        }

        transform.position = pos;
    }
}
