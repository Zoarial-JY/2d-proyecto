using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("Propiedades de la bala")]
    public float speed = 12f;
    public float lifeTime = 3f;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.right; // por defecto

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // autodestruye después del tiempo indicado
        Invoke(nameof(Despawn), lifeTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void FixedUpdate()
    {
        // mover la bala en su dirección asignada
        rb.linearVelocity = direction * speed;
    }

    // Este método lo llama el Player para definir la dirección
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // voltear sprite si la bala tiene SpriteRenderer
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = (direction.x < 0);
    }

    // destruir al salir de cámara (opcional)
    void OnBecameInvisible()
    {
        Despawn();
    }

    void Despawn()
    {
        Destroy(gameObject);
    }
}
