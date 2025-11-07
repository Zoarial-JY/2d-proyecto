using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void Jugar()
    {
        // Obtiene el índice actual de escena
        int next = SceneManager.GetActiveScene().buildIndex + 1;

        // Verifica si hay una escena siguiente válida en Build Settings
        if (next < SceneManager.sceneCountInBuildSettings)
        {
            Time.timeScale = 1f; // por si estaba pausado
            string scenePath = SceneUtility.GetScenePathByBuildIndex(next);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"🎮 Cargando siguiente escena: {sceneName}");
            SceneManager.LoadScene(next);
        }
        else
        {
            // 🔁 Fallback: intenta cargar la escena por nombre manualmente
            string fallbackScene = "SampleScene"; // cambia el nombre si tu escena tiene otro
            Debug.LogWarning($"⚠️ No hay escena siguiente configurada. Intentando cargar '{fallbackScene}'...");

            try
            {
                SceneManager.LoadScene(fallbackScene);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error: no se pudo cargar '{fallbackScene}'. Asegúrate de agregarla en Build Settings.\n{e.Message}");
            }
        }
    }

    public void Salir()
    {
        Debug.Log("👋 Cerrando el juego...");
        Application.Quit();

#if UNITY_EDITOR
        // Esto evita cierres inesperados cuando estás probando en el Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
