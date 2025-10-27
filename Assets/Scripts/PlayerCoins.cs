using UnityEngine;
using System;

public class PlayerCoins : MonoBehaviour
{
    public int coins = 0;
    public event Action<int> OnCoinsChanged;

    void Start() => Notify();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<CoinPickup>() != null)
        {
            coins++;
            Debug.Log("💰 Moneda recogida. Total: " + coins);
            Notify();
            Destroy(other.gameObject);
        }
    }

    void Notify() => OnCoinsChanged?.Invoke(coins);
}
