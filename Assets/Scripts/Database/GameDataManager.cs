using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    private FirebaseFirestore db;
    private const string COLLECTION = "players";
    private const string DOC_ID = "player1";

    // Evento opcional para avisar que se cargó desde la nube
    public event Action<int, int> OnLoaded; // (coins, level)

    void Awake()
    {
        // Singleton + persistencia
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        db = FirebaseFirestore.DefaultInstance;
    }

    /// <summary>
    /// Carga coins y level desde Firestore.
    /// </summary>
    public void LoadData()
    {
        db.Collection(COLLECTION).Document(DOC_ID).GetSnapshotAsync()
            .ContinueWithOnMainThread(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogError("❌ Error al cargar Firestore: " + t.Exception);
                    OnLoaded?.Invoke(0, 1); // valores por defecto
                    return;
                }

                var snap = t.Result;
                if (snap.Exists)
                {
                    int coins = snap.ContainsField("coins") ? snap.GetValue<int>("coins") : 0;
                    int level = snap.ContainsField("level") ? snap.GetValue<int>("level") : 1;

                    Debug.Log($"🔄 Cargado Firestore -> coins={coins}, level={level}");
                    OnLoaded?.Invoke(coins, level);
                }
                else
                {
                    Debug.Log("⚠️ No había datos, creando documento inicial (coins=0, level=1).");
                    SaveData(0, 1); // crea inicial
                    OnLoaded?.Invoke(0, 1);
                }
            });
    }

    /// <summary>
    /// Guarda coins y level en Firestore.
    /// </summary>
    public void SaveData(int coins, int level)
    {
        var data = new { coins, level };

        db.Collection(COLLECTION).Document(DOC_ID).SetAsync(data)
            .ContinueWithOnMainThread(t =>
            {
                if (t.IsCompleted)
                    Debug.Log($"✅ Guardado en Firestore -> coins={coins}, level={level}");
                else if (t.IsFaulted)
                    Debug.LogError("❌ Error al guardar: " + t.Exception);
            });
    }
}
