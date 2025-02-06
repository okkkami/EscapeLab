using UnityEngine;
using UnityEngine.SceneManagement;

public class Porta : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;  // Nombre de la escena destino
    [SerializeField] private string doorName;     // Nombre único de esta puerta
    private EnemyManager enemyManager; // Referencia al EnemyManager

    private void Start()
    {
        // Obtener la referencia al EnemyManager en la escena
        enemyManager = FindObjectOfType<EnemyManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>(); // Obtener la referencia al jugador
            Debug.Log("Jugador ha entrado en la puerta.");

            // Verificar si la puerta es DoorInicial
            if (doorName == "DoorInicial")
            {
                // Siempre se puede pasar por DoorInicial
                CambiarEscena();
            }
            else if (doorName == "DoorTesoro")
            {
                // Verificar si el jugador tiene la llave
                if (player != null && player.HasKey())
                {
                    CambiarEscena();
                }
                else
                {
                    Debug.Log("La puerta del tesoro está bloqueada. Necesitas una llave.");
                }
            }
            else
            {
                // Verificar si todos los enemigos han sido derrotados
                if (enemyManager != null && enemyManager.AreAllEnemiesDefeated())
                {
                    CambiarEscena();
                }
                else
                {
                    Debug.Log("La puerta está bloqueada. Derrota a todos los enemigos primero.");
                }
            }
        }
    }

    // Método para cambiar de escena
    private void CambiarEscena()
    {
        Debug.Log("Cambiando de escena.");
        PlayerPrefs.SetString("LastDoor", doorName);
        SceneManager.LoadScene(sceneToLoad);
    }
}