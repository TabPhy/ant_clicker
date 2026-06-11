using UnityEngine;

/// <summary>
/// Defines the walking behaviour of one ant agent on screen.
/// Each ant picks a random target point along a tunnel path,
/// walks to it, optionally plays a "carrying food" animation, then picks a new target.
///
/// Attach to your Ant prefab (a simple SpriteRenderer GameObject).
/// </summary>
public class AntAgent : MonoBehaviour
{
    // ── Configuration (set by AntSpawner on spawn) ────────────────
    [HideInInspector] public float walkSpeed      = 40f;
    [HideInInspector] public float activityLevel  = 0.5f;   // 0–1 affects idle pause duration
    [HideInInspector] public bool  isCarryingFood = false;

    [Header("Sprites")]
    public Sprite spriteIdle;
    public Sprite spriteWalking;
    public Sprite spriteCarrying;

    [Header("Bounds — set to match your ant farm width/height")]
    public float minX = -200f;
    public float maxX =  200f;
    public float minY = -300f;
    public float maxY = -20f;

    // ── Runtime ───────────────────────────────────────────────────
    private SpriteRenderer _sr;
    private Vector3        _target;
    private bool           _idle        = false;
    private float          _idleTimer   = 0f;
    private float          _idleDuration = 0f;

    private enum State { Walking, Idle, CarryingFood }
    private State _state = State.Walking;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        PickNewTarget();
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Walking:
            case State.CarryingFood:
                HandleWalking();
                break;
        }
    }

    // ── Walking ───────────────────────────────────────────────────
    private void HandleWalking()
    {
        Vector3 dir      = (_target - transform.position).normalized;
        transform.position += dir * walkSpeed * Time.deltaTime;

        // Flip sprite to face direction of travel
        if (dir.x != 0)
            _sr.flipX = dir.x < 0;

        UpdateSprite();

        // Arrived?
        if (Vector3.Distance(transform.position, _target) < 2f)
        {
            // Occasionally pause
            bool shouldIdle = Random.value < (0.4f * (1f - activityLevel) + 0.05f);
            if (shouldIdle)
                EnterIdle();
            else
                PickNewTarget();
        }
    }

    // ── Idle ──────────────────────────────────────────────────────
    private void EnterIdle()
    {
        _state        = State.Idle;
        _idleDuration = Random.Range(0.5f, 3f * (1f - activityLevel) + 0.5f);
        _idleTimer    = 0f;
        UpdateSprite();
    }

    private void HandleIdle()
    {
        _idleTimer += Time.deltaTime;
        if (_idleTimer >= _idleDuration)
        {
            // Randomly decide to carry food
            isCarryingFood = Random.value < 0.3f * activityLevel;
            _state         = isCarryingFood ? State.CarryingFood : State.Walking;
            PickNewTarget();
        }
    }

    // ── Targeting ─────────────────────────────────────────────────
    private void PickNewTarget()
    {
        // Ants prefer to walk horizontally (tunnel behaviour)
        // Small Y variance keeps them on a "tunnel row"
        float targetX = Random.Range(minX, maxX);
        float targetY = transform.position.y + Random.Range(-10f, 10f);
        targetY       = Mathf.Clamp(targetY, minY, maxY);

        _target = new Vector3(targetX, targetY, transform.position.z);
        _state  = isCarryingFood ? State.CarryingFood : State.Walking;
    }

    // ── Sprite ───────────────────────────────────────────────────
    private void UpdateSprite()
    {
        if (_sr == null) return;
        _sr.sprite = _state switch
        {
            State.Idle         => spriteIdle    != null ? spriteIdle    : spriteWalking,
            State.CarryingFood => spriteCarrying != null ? spriteCarrying : spriteWalking,
            _                  => spriteWalking,
        };
    }

    // ── Public API ────────────────────────────────────────────────
    public void SetBounds(float x0, float x1, float y0, float y1)
    {
        minX = x0; maxX = x1;
        minY = y0; maxY = y1;
    }

    /// <summary>Snap ant to a random position within bounds (used on spawn).</summary>
    public void RandomisePosition()
    {
        transform.position = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            transform.position.z
        );
        PickNewTarget();
    }
}
