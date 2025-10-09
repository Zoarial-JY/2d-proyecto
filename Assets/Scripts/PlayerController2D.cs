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
    public int extraAirJumps = 0;        // pon 1 para doble salto
    public float coyoteTime = 0.10f;     // tiempo de perdón al borde
    public float jumpBufferTime = 0.12f; // buffer de pulsación previa
    public float fallGravityMultiplier = 2f;
    public float lowJumpMultiplier = 2.5f;

    [Header("Detección de suelo")]
    public LayerMask groundLayer;        // asigna la capa "Ground" en el Inspector

    [Header("Controles")]
    public KeyCode jumpKey = KeyCode.Space;

    // ---
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
        airJumpsLeft = extraAirJumps;
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        // 👉 Actualizamos parámetro "Running" del Animator
        bool isRunning = Mathf.Abs(inputX) > 0.1f && IsGrounded();
        animator.SetBool("Running", isRunning);

        // --- NUEVO: animación de salto (Jumping)
        bool isJumping = !IsGrounded();         // cuando el personaje no está tocando el suelo
        animator.SetBool("Jumping", isJumping); // activa o desactiva el parámetro Jumping

        // Buffer de salto
        if (Input.GetKeyDown(jumpKey))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // Coyote time + recarga saltos aéreos
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
            airJumpsLeft = extraAirJumps;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        // Ejecutar salto si hay buffer disponible
        if (jumpBufferCounter > 0f)
        {
            if (coyoteCounter > 0f)              // salto desde suelo (incluye coyote)
            {
                DoJump();
            }
            else if (airJumpsLeft > 0)           // saltos aéreos (doble salto, etc.)
            {
                DoJump();
                airJumpsLeft--;
            }
            jumpBufferCounter = 0f;
        }

        // Voltear sprite
        if (inputX > 0 && !isFacingRight) Flip();
        else if (inputX < 0 && isFacingRight) Flip();
    }

    void FixedUpdate()
    {
        // Aceleración/desaceleración suave
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

    bool IsGrounded()
    {
        // Usa el propio collider del Player para saber si toca la capa Ground
        return col.IsTouchingLayers(groundLayer);
    }

    void DoJump()
    {
        // reset y aplica impulso vertical consistente
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteCounter = 0f;

        animator.SetBool("Running", false); 
        animator.SetBool("Jumping", true);  // 👈 activa la animación de salto al presionar Space


    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }
}
