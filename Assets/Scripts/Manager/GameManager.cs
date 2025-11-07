// GameManager.cs  (objeto persistente)
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Vinculaciones en escena")]
    public PauseMenu pauseMenu; // se reencuentra solo en cada escena
    public CoinUI coinUI;       // opcional: tu HUD de monedas

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m) => Rebind();

    public void Rebind()
    {
        // Encuentra referencias de la escena actual (no inactivos)
        pauseMenu = FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Exclude);
        coinUI    = FindAnyObjectByType<CoinUI>(FindObjectsInactive.Exclude);

        Debug.Log($"[GM] Rebind -> pauseMenu={(pauseMenu!=null)}, coinUI={(coinUI!=null)}");
    }
}
