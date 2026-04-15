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

    // Public state
    public int wormID;
    public bool IsAlive = true;

    // Movement tracking - these are PUBLIC so UI can read them
    private float _distanceMoved = 0f;
    public float DistanceRemaining => maxMoveDistance - _distanceMoved;
    public float DistancePercent => _distanceMoved / maxMoveDistance;

    // Private references (cached for performance)
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private Camera _mainCam;

    // State
    public bool _isGrounded = true;

    public bool IsTurn => WormManager.Instance.IsMyTurn(wormID);

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _mainCam = Camera.main;
    }

    void Update()
    {
        if (!IsTurn || !IsAlive)
            return;

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

        if (horizInput != 0)
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
        // Can only shoot when not moving
        if (Input.GetAxis("Horizontal") != 0)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
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

            // Hide gun and end turn
            EnableGun(false);
            TimerController.Instance.ResetTime();

            if (IsTurn)
                WormManager.Instance.NextWorm();
        }
    }

    private void RotateGun()
    {
        Vector3 mouseWorld = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mouseWorld - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _currGun.rotation = Quaternion.Euler(0, 0, angle + 180f);
    }

    private void EnableGun(bool enable)
    {
        _currGun.gameObject.SetActive(enable);
    }

    // Called by WormManager when this worm's turn starts
    public void StartTurn()
    {
        _distanceMoved = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bullet"))
        {
            // Deal 25 damage (was 10 - increased for faster gameplay)
            _wormHealth.ChangeHealth(-25);

            if (IsTurn)
                WormManager.Instance.NextWorm();
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
