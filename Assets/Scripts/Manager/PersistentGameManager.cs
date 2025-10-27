using UnityEngine;

public class PersistentGameManager : MonoBehaviour
{
    private static PersistentGameManager _instance;
    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
