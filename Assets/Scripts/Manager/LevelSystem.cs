using UnityEngine;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerCoins playerCoins;   // Arrastra aquí el Player
    public TMP_Text levelText;        // Texto "Nivel: X"
    public TMP_Text coinText;         // Texto "Monedas: X/3"

    [Header("Reglas de progreso")]
    public int level = 1;             // Nivel inicial (se sobreescribe al cargar)
    public int coinsPerLevel = 3;

    private GameDataManager data;     // referencia al gestor de datos
    private bool isInitialized = false; // ← NUEVO: hasta que cargue Firestore


    void Start()
    {
        if (playerCoins == null) playerCoins = FindObjectOfType<PlayerCoins>();
        data = GameDataManager.Instance ?? FindObjectOfType<GameDataManager>();

        if (playerCoins == null)
        {
            Debug.LogError("❌ LevelSystem: No se encontró PlayerCoins.");
            return;
        }

        // Suscripción a cambios locales de monedas
        playerCoins.OnCoinsChanged += HandleCoinsChanged;

        // Si hay GameDataManager, cargar desde Firestore
        if (data != null)
        {
            data.OnLoaded += ApplyLoadedState; // suscribirse para recibir los datos
            data.LoadData();                    // dispara la carga
        }
        else
        {
            // Sin Firestore: usa local y permite guardar localmente
            isInitialized = true;
            UpdateUI();
            HandleCoinsChanged(playerCoins.coins);
        }
    }

    void OnDestroy()
    {
        if (playerCoins != null) playerCoins.OnCoinsChanged -= HandleCoinsChanged;
        if (data != null) data.OnLoaded -= ApplyLoadedState;
    }

    // Aplicar lo que viene de Firestore al inicio
    void ApplyLoadedState(int loadedCoins, int loadedLevel)
    {
        level = Mathf.Max(1, loadedLevel);
        playerCoins.coins = Mathf.Max(0, loadedCoins);

        Debug.Log($"☁️ Estado aplicado desde Firestore: level={level}, coins={playerCoins.coins}");
        UpdateUI();
        isInitialized = true;  
    }

    void HandleCoinsChanged(int currentCoins)
    {
        if (!isInitialized) return;  // ← NUEVO: evita guardar al iniciar

        int newLevel = level;
        int coinsTemp = currentCoins;

        // Subir niveles consumiendo monedas (3 por nivel, o coinsPerLevel)
        while (coinsTemp >= coinsPerLevel)
        {
            coinsTemp -= coinsPerLevel;
            newLevel++;
            Debug.Log($"🏆 ¡Subiste a nivel {newLevel}!");
        }

        // Aplicar los valores procesados (resto de monedas tras consumo)
        bool levelChanged = (newLevel != level);
        bool coinsChanged = (coinsTemp != playerCoins.coins);

        if (levelChanged || coinsChanged)
        {
            level = newLevel;
            playerCoins.coins = coinsTemp;
        }

        UpdateUI();

        // 🔹 GUARDAR SIEMPRE EN FIRESTORE TRAS CADA PICK-UP (y tras subir nivel)
        if (data != null)
        {
            data.SaveData(playerCoins.coins, level); // <-- NUEVO: guardar siempre
            Debug.Log($"☁️ Guardado en Firestore (cada cambio): coins={playerCoins.coins}, level={level}");
        }
    }


    void UpdateUI()
    {
        if (levelText != null) levelText.text = $"Nivel: {level}";
        if (coinText != null)  coinText.text  = $"Monedas: {playerCoins.coins}/{coinsPerLevel}";
    }

    public void ResetProgress()
    {
        Debug.Log("🔁 Reiniciando progreso local y remoto...");

        // 1️⃣ Detener temporalmente la reacción a eventos
        isInitialized = false; // bloquea guardados durante el reset
        if (playerCoins != null)
            playerCoins.OnCoinsChanged -= HandleCoinsChanged;

        // 2️⃣ Reiniciar valores en memoria
        level = 1;
        if (playerCoins != null)
            playerCoins.coins = 0;

        UpdateUI();

        // 3️⃣ Guardar el nuevo estado limpio en la base de datos
        if (data != null)
            data.SaveData(0, 1);

        // 4️⃣ Volver a habilitar sincronización y eventos
        if (playerCoins != null)
            playerCoins.OnCoinsChanged += HandleCoinsChanged;
        isInitialized = true;

        Debug.Log("✅ Progreso reiniciado correctamente: coins=0, level=1.");
    }


}
