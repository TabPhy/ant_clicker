using UnityEngine;
using System.Collections;

/// <summary>
/// Visually pulses (bobs up/down) a structure or ant sprite to give the farm a "alive" feel.
/// Great for the Queen, the Moon Rocket on its pad, flag poles, etc.
///
/// Attach directly to any sprite GameObject.
/// </summary>
public class IdleAnimator : MonoBehaviour
{
    public enum AnimationType { Bob, Rotate, Breathe, Wiggle }

    [Header("Animation")]
    public AnimationType type      = AnimationType.Bob;
    public float         speed     = 1.2f;
    public float         magnitude = 4f;   // pixels (Bob/Wiggle) or degrees (Rotate) or scale delta (Breathe)

    [Header("Random Phase Offset")]
    [Tooltip("Randomise start phase so multiple objects don't move in sync")]
    public bool randomPhase = true;

    private Vector3 _origin;
    private float   _phase;

    private void Start()
    {
        _origin = transform.localPosition;
        _phase  = randomPhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    private void Update()
    {
        float t = Time.time * speed + _phase;

        switch (type)
        {
            case AnimationType.Bob:
                transform.localPosition = _origin + Vector3.up * Mathf.Sin(t) * magnitude;
                break;

            case AnimationType.Rotate:
                transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(t) * magnitude);
                break;

            case AnimationType.Breathe:
                float s = 1f + Mathf.Sin(t) * magnitude * 0.01f;
                transform.localScale = Vector3.one * s;
                break;

            case AnimationType.Wiggle:
                transform.localPosition = _origin
                    + Vector3.right * Mathf.Sin(t * 2.3f) * magnitude * 0.5f
                    + Vector3.up    * Mathf.Sin(t)         * magnitude * 0.3f;
                break;
        }
    }
}
