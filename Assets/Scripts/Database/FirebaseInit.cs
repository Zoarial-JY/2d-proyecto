using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    void Start()
    {
        // Verifica e instala dependencias necesarias de Firebase antes de usarlo
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                Debug.Log("✅ Firebase listo.");
                // Aquí ya puedes usar Firestore/Auth/etc. si quisieras.
            }
            else
            {
                Debug.LogError($"❌ No se pudo inicializar Firebase: {status}. " +
                               "Revisa el google-services.json y vuelve a resolver dependencias.");
            }
        });
    }
}
