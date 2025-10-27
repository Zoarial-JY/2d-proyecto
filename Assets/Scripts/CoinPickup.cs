using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // asegúrate de ponerle Tag = Player al jugador
        {
            Debug.Log("💰 El Player recogió una moneda");
            Destroy(gameObject); // opcional, para que desaparezca
        }
    }
}
