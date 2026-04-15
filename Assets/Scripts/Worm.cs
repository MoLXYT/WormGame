using UnityEngine;

public class Worm : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _bulletPrefab;
    [SerializeField] private Transform _currGun;
    [SerializeField] private WormHealth _wormHealth;

    public float walkSpeed = 1f;
    public float maxRelativeVelocity = 6f;
    public float misileForce = 5f;

    public int wormID;

    public bool IsAlive = true;

    private SpriteRenderer _spriteRenderer;
    private Camera _maincam;

    public Vector2 _jumpHeight;

    public bool IsTurn { get { return WormManager.Instance.IsMyTurn(wormID); } }

    private Vector3 diff;
    public bool _isGrounded;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _maincam = Camera.main;
        _isGrounded = true;
    }

    void Update()
    {
        if (!IsTurn || !IsAlive)
            return;

        RotateGun();
        MovementAndShooting();
        Jump();
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            GetComponent<Rigidbody2D>().AddForce(_jumpHeight, ForceMode2D.Impulse);
            _isGrounded = false;
        }
    }

    private void RotateGun()
    {
        diff = _maincam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        _currGun.rotation = Quaternion.Euler(0, 0, rot_z + 180f);
    }

    private void MovementAndShooting()
    {
        float horizMovement = Input.GetAxis("Horizontal");

        if (horizMovement == 0)
        {
            EnableGun(true);

            if (Input.GetKey(KeyCode.Mouse0))
            {
                TimerController.Instance.ResetTime();
                EnableGun(false);

                Rigidbody2D bullet = Instantiate(
                    _bulletPrefab,
                    _currGun.position - _currGun.right,
                    _currGun.rotation
                );

                bullet.AddForce(-_currGun.right * misileForce, ForceMode2D.Impulse);
                Destroy(bullet.gameObject, 5);

                if (IsTurn)
                    WormManager.Instance.NextWorm();
            }
        }
        else
        {
            EnableGun(false);
            transform.position += Vector3.right * horizMovement * Time.deltaTime * walkSpeed;
            _spriteRenderer.flipX = horizMovement > 0;
        }
    }

    private void EnableGun(bool enable)
    {
        _currGun.gameObject.SetActive(enable);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bullet"))
        {
            _wormHealth.ChangeHealth(-10);

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