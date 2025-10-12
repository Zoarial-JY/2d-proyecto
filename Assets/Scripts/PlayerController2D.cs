using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 7f;
    public float acceleration = 12f;
    public float deceleration = 14f;
    public float velPower = 0.9f;

    [Header("Salto")]
    public float jumpForce = 14f;
    public int extraAirJumps = 0;
    public float coyoteTime = 0.10f;
    public float jumpBufferTime = 0.12f;
    public float fallGravityMultiplier = 2f;
    public float lowJumpMultiplier = 2.5f;

    [Header("Disparo")] // 👈 NUEVO
    public GameObject bulletPrefab;   // asigna el prefab Bullet
    public Transform firePoint;       // asigna el FirePoint
    public KeyCode shootKey = KeyCode.K;
    public float bulletSpeed = 12f;
    public float fireRate = 0.3f;     // tiempo entre disparos
    private float nextShootTime = 0f;

    [Header("Detección de suelo")]
    public LayerMask groundLayer;

    [Header("Controles")]
    public KeyCode jumpKey = KeyCode.UpArrow;
    public KeyCode altJumpKey = KeyCode.W;
    

    Rigidbody2D rb;
    Collider2D col;
    Animator animator;

    float inputX;
    bool isFacingRight = true;

    float coyoteCounter;
    float jumpBufferCounter;
    int airJumpsLeft;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        airJumpsLeft = extraAirJumps;
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        // --- Animaciones ---
        bool isRunning = Mathf.Abs(inputX) > 0.1f && IsGrounded();
        animator.SetBool("Running", isRunning);

        bool isJumping = !IsGrounded();
        animator.SetBool("Jumping", isJumping);

        // --- Movimiento / Salto ---
        HandleJump();
        FlipSprite();

        // --- NUEVO: Disparo ---
        HandleShooting();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    // ----------------------------
    // 🧠 FUNCIONES DE MOVIMIENTO
    // ----------------------------

    void HandleMovement()
    {
        float targetSpeed = inputX * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
        rb.AddForce(movement * Vector2.right);

        // Gravedad mejorada
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = fallGravityMultiplier;
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(jumpKey))
            rb.gravityScale = lowJumpMultiplier;
        else
            rb.gravityScale = 1f;
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(jumpKey) || Input.GetKeyDown(altJumpKey))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
            airJumpsLeft = extraAirJumps;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f)
        {
            if (coyoteCounter > 0f)
            {
                DoJump();
            }
            else if (airJumpsLeft > 0)
            {
                DoJump();
                airJumpsLeft--;
            }
            jumpBufferCounter = 0f;
        }
    }

    void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteCounter = 0f;

        animator.SetBool("Running", false);
        animator.SetBool("Jumping", true);
    }

    void FlipSprite()
    {
        if (inputX > 0 && !isFacingRight) Flip();
        else if (inputX < 0 && isFacingRight) Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    bool IsGrounded()
    {
        return col.IsTouchingLayers(groundLayer);
    }

    // ----------------------------
    // 💥 FUNCIONES DE DISPARO
    // ----------------------------

    void HandleShooting()
    {
        if (Input.GetKeyDown(shootKey) && Time.time >= nextShootTime)
        {
            nextShootTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Dirección según el Player (mirando a derecha o izquierda)
        float dirX = Mathf.Sign(transform.localScale.x);
        Vector2 dir = new Vector2(dirX, 0f);

        // Instancia la bala en FirePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Si el prefab tiene BulletScript, le pasamos la dirección
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(dir);
        }
        else
        {
        // Si no, aplicamos la velocidad directamente (respaldo)
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            rbBullet.gravityScale = 0;
            rbBullet.linearVelocity = dir * bulletSpeed;
        }
}

        // Opcional: destruir bala después de un tiempo
        Destroy(bullet, 3f);
    }
}
