using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Aseg�rate de incluir esto para trabajar con UI

public class Porta : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;  // Nombre de la escena destino
    [SerializeField] private string doorName;     // Nombre �nico de esta puerta
    private EnemyManager enemyManager; // Referencia al EnemyManager
    public GameObject blockedDoorImage; // Referencia a la imagen de puerta bloqueada

    private void Start()
    {
        // Obtener la referencia al EnemyManager en la escena
        enemyManager = FindObjectOfType<EnemyManager>();

        // Aseg�rate de que la imagen de puerta bloqueada est� desactivada al inicio
        if (blockedDoorImage != null)
        {
            blockedDoorImage.SetActive(true);
        }
    }

    private void Update()
    {
        // Verificar si hay enemigos en la sala actual
        if (enemyManager != null && enemyManager.AreAllEnemiesDefeated())
        {
            HideBlockedDoorImage(); // Ocultar la imagen si no hay enemigos
        }
        else
        {
            ShowBlockedDoorImage(); // Mostrar la imagen si hay enemigos
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>(); // Obtener la referencia al jugador
            Debug.Log("Jugador ha entrado en la puerta.");

            // Verificar si la puerta es DoorInicial
            if (doorName == "doorInicialR")
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
                    Debug.Log("La puerta del tesoro est� bloqueada. Necesitas una llave.");
                    ShowBlockedDoorImage(); // Mostrar imagen de puerta bloqueada
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
                    Debug.Log("La puerta est� bloqueada. Derrota a todos los enemigos primero.");
                    ShowBlockedDoorImage(); // Mostrar imagen de puerta bloqueada
                }
            }
        }
    }

    // M�todo para cambiar de escena
    private void CambiarEscena()
    {
        Debug.Log("Cambiando de escena.");
        PlayerPrefs.SetString("LastDoor", doorName);
        SceneManager.LoadScene(sceneToLoad);
    }

    // M�todo para mostrar la imagen de puerta bloqueada
    private void ShowBlockedDoorImage()
    {
        if (blockedDoorImage != null)
        {
            blockedDoorImage.SetActive(false); // Activar la imagen
        }
    }

    // M�todo para ocultar la imagen de puerta bloqueada
    public void HideBlockedDoorImage()
    {
        if (blockedDoorImage != null)
        {
            blockedDoorImage.SetActive(false); // Desactivar la imagen
        }
    }
}