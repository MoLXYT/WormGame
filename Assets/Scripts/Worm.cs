using UnityEngine;

public class Worm : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _bulletPrefab;
    [SerializeField] private Transform _currGun;
    [SerializeField] private WormHealth _wormHealth;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float maxMoveDistance = 5f;

    [Header("Jump Settings")]
    public Vector2 jumpForce = new Vector2(0, 8f);

    [Header("Weapon Settings")]
    public float missileForce = 10f;

    // Public state - assigned by WormManager
    [HideInInspector] public int wormID = -1;
    public bool IsAlive = true;

    // Movement tracking
    private float _distanceMoved = 0f;
    public float DistanceRemaining => maxMoveDistance - _distanceMoved;
    public float DistancePercent => maxMoveDistance > 0 ? _distanceMoved / maxMoveDistance : 0;

    // Private references (cached for performance)
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private Camera _mainCam;

    // State
    public bool _isGrounded = true;
    private bool _hasShot = false;

    /// <summary>
    /// Returns true only if:
    /// - WormManager exists
    /// - Game is not over
    /// - Not transitioning between turns
    /// - It's actually this worm's turn
    /// - This worm is alive
    /// </summary>
    public bool IsTurn
    {
        get
        {
            // Safety checks
            if (WormManager.Instance == null)
                return false;

            if (!IsAlive)
                return false;

            // This does all the checking (game over, transitioning, correct ID)
            return WormManager.Instance.IsMyTurn(wormID);
        }
    }

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _mainCam = Camera.main;

        // Hide gun at start (will show when it's our turn)
        if (_currGun != null)
            _currGun.gameObject.SetActive(false);
    }

    void Update()
    {
        // CRITICAL: Only do anything if it's our turn
        if (!IsTurn)
        {
            // Make sure gun is hidden when not our turn
            if (_currGun != null && _currGun.gameObject.activeSelf)
                _currGun.gameObject.SetActive(false);
            return;
        }

        // It's our turn - handle input
        RotateGun();
        HandleMovement();
        HandleJump();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float horizInput = Input.GetAxis("Horizontal");

        // Can't move if no distance remaining
        if (_distanceMoved >= maxMoveDistance)
        {
            // Still allow gun to show when stopped
            if (horizInput == 0)
                EnableGun(true);
            return;
        }

        if (Mathf.Abs(horizInput) > 0.01f)
        {
            EnableGun(false);

            // Calculate how much we want to move
            float moveAmount = Mathf.Abs(horizInput) * Time.deltaTime * walkSpeed;

            // Clamp to remaining distance
            float remainingDistance = maxMoveDistance - _distanceMoved;
            moveAmount = Mathf.Min(moveAmount, remainingDistance);

            // Apply movement
            float direction = Mathf.Sign(horizInput);
            transform.position += Vector3.right * direction * moveAmount;

            // Track distance
            _distanceMoved += moveAmount;

            // Flip sprite based on direction
            _spriteRenderer.flipX = horizInput > 0;
        }
        else
        {
            EnableGun(true);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
            _isGrounded = false;
        }
    }

    private void HandleShooting()
    {
        // Prevent shooting multiple times per turn
        if (_hasShot)
            return;

        // Can only shoot when not moving
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _hasShot = true;

            // Create bullet
            Rigidbody2D bullet = Instantiate(
                _bulletPrefab,
                _currGun.position - _currGun.right,
                _currGun.rotation
            );

            // Launch bullet
            bullet.AddForce(-_currGun.right * missileForce, ForceMode2D.Impulse);

            // Destroy after 5 seconds
            Destroy(bullet.gameObject, 5f);

            // Hide gun
            EnableGun(false);

            Debug.Log($"{gameObject.name} fired!");

            // End turn after a short delay (let bullet travel)
            Invoke(nameof(EndTurnAfterShot), 0.5f);
        }
    }

    private void EndTurnAfterShot()
    {
        if (WormManager.Instance != null)
        {
            WormManager.Instance.NextWorm();
        }
    }

    private void RotateGun()
    {
        if (_currGun == null || _mainCam == null)
            return;

        Vector3 mouseWorld = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mouseWorld - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _currGun.rotation = Quaternion.Euler(0, 0, angle + 180f);
    }

    private void EnableGun(bool enable)
    {
        if (_currGun != null)
            _currGun.gameObject.SetActive(enable);
    }

    /// <summary>
    /// Called by WormManager when this worm's turn starts.
    /// </summary>
    public void StartTurn()
    {
        _distanceMoved = 0f;
        _hasShot = false;
        EnableGun(true);

        Debug.Log($"{gameObject.name}: My turn started! Distance reset to 0.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsAlive)
            return;

        if (collision.CompareTag("bullet"))
        {
            // Deal damage (25 = dies in 4 hits)
            _wormHealth.ChangeHealth(-25);
            Debug.Log($"{gameObject.name} was hit! Health changed by -25");
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") && !_isGrounded)
        {
            _isGrounded = true;
        }
    }
}
