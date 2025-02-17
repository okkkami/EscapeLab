using UnityEngine;
using UnityEngine.SceneManagement;

public class Porta : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;  // Nombre de la escena destino
    [SerializeField] private string doorName;     // Nombre �nico de esta puerta
    private EnemyManager enemyManager; // Referencia al EnemyManager

    private Animator animator;
    private bool isLocked = true; // Estado inicial de la puerta
    private bool isOpen = false; // Estado de la puerta (abierta o cerrada)

    private void Start()
    {
        animator = GetComponent<Animator>();
        enemyManager = FindObjectOfType<EnemyManager>(); // Obtener la referencia al EnemyManager
    }

    // M�todo para desbloquear la puerta
    public void UnlockDoor()
    {
        if (isLocked)
        {
            isLocked = false;

            // Verificar si todos los enemigos han sido derrotados usando el m�todo del EnemyManager
            if (enemyManager != null && enemyManager.AreAllEnemiesDefeated())
            {
                OpenDoor();
            }
        }
    }

    private void OpenDoor()
    {
        if (!isOpen && animator != null)
        {
            animator.SetTrigger("OpenDoor"); // Aseg�rate de que este nombre coincida exactamente
            isOpen = true; // Cambia el estado a abierto
        }
    }

   

    // M�todo para verificar si la puerta est� bloqueada
    public bool IsLocked()
    {
        return isLocked;
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
                   
                    CambiarEscena(); // Cambiar de escena
                }
                else
                {
                    Debug.Log("La puerta del tesoro est� bloqueada. Necesitas una llave.");
                }
            }
            else
            {
                // Verificar si todos los enemigos han sido derrotados
                if (enemyManager != null && enemyManager.AreAllEnemiesDefeated())
                {
                    UnlockDoor(); // Desbloquear la puerta
                    CambiarEscena(); // Cambiar de escena
                }
                else
                {
                    Debug.Log("La puerta est� bloqueada. Derrota a todos los enemigos primero.");
                }
            }
        }
    }

    // M�todo para cambiar de escena
    private void CambiarEscena()
    {
        PlayerPrefs.SetString("LastDoor", doorName);
        SceneManager.LoadScene(sceneToLoad);
    }
}