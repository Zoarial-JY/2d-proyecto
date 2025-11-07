using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pausaPanel;   // Asignar en el Inspector
    bool isPaused;

    void Awake()
    {
        if (pausaPanel == null)
        {
            var t = transform.Find("PausaPanel");
            if (t) pausaPanel = t.gameObject;
        }
        SetPaused(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause() => SetPaused(!isPaused);

    public void SetPaused(bool value)
    {
        isPaused = value;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pausaPanel) pausaPanel.SetActive(isPaused);
    }

    // Botón “Reanudar”
    public void Reanudar() => SetPaused(false);

    // Botón “Ir al Menú”
    public void IrAlMenu()
    {
        SetPaused(false);
        Time.timeScale = 1f;                // aseguramos tiempo normal
        SceneManager.LoadScene("MainMenu"); // nombre exacto de tu escena
    }
}
