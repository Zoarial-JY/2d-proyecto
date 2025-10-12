using UnityEngine;

public class CamaraScript : MonoBehaviour
{
    public GameObject Player; // arrastra aquí tu Player en el Inspector

    void Update()
    {
        // Guardamos la posición actual de la cámara
        Vector3 position = transform.position;

        // Hacemos que la cámara siga la posición X del jugador
        position.x = Player.transform.position.x;

        // (Opcional) Aqui sigue también en Y
        // position.y = Player.transform.position.y;

        // Aplicamos la nueva posición manteniendo el valor de Z
        transform.position = position;
    }
}
