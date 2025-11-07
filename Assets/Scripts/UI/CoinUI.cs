using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [Header("Referencia al texto de monedas")]
    [SerializeField] private TMP_Text coinsText;

    /// <summary>
    /// Actualiza el texto del contador de monedas en pantalla.
    /// </summary>
    public void SetCoins(int value)
    {
        if (coinsText != null)
        {
            coinsText.text = $"Monedas: {value}";
        }
        else
        {
            Debug.LogWarning("[CoinUI] No hay texto asignado para mostrar las monedas.");
        }
    }
}
