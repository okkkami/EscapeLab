using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 3; // Salud m�xima del enemigo
    private int currentHealth; // Salud actual del enemigo
    public float moveSpeed = 2f; // Velocidad de movimiento del enemigo
    private Transform player; // Referencia al transform del jugador

    public bool IsDead { get; private set; } = false; // Estado del enemigo
    private EnemyManager enemyManager;
    private Rigidbody2D rb; // Referencia al Rigidbody2D del enemigo

    public float bounceForce = 5f; // Fuerza de rebote al colisionar con el jugador

    // Referencia al prefab de moneda
    public GameObject coinPrefab; // Asigna el prefab de moneda en el inspector

    private void Start()
    {
        currentHealth = maxHealth; // Inicializar la salud actual
        player = GameObject.FindGameObjectWithTag("Player").transform; // Buscar el jugador
        enemyManager = FindObjectOfType<EnemyManager>();
        rb = GetComponent<Rigidbody2D>(); // Obtener el componente Rigidbody2D
    }

    private void Update()
    {
        if (player != null && !IsDead)
        {
            // Moverse hacia el jugador
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(1); // Quitar 1 de vida al jugador

                // Calcular la direcci�n de retroceso
                Vector2 bounceDirection = (transform.position - other.transform.position).normalized;

                // Mover el enemigo un peque�o paso atr�s
                float stepBackDistance = 0.5f; // Ajusta esta distancia seg�n sea necesario
                transform.position += (Vector3)bounceDirection * stepBackDistance; // Mover el enemigo hacia atr�s
            }
        }
    }

    // M�todo para recibir da�o
    public void TakeDamage(int damage)
    {
        if (IsDead) return; // No recibir da�o si ya est� muerto

        currentHealth -= damage; // Reducir la salud actual

        if (currentHealth <= 0)
        {
            Die(); // Si la salud es 0 o menos, el enemigo muere
        }
    }

    private void Die()
    {
        if (enemyManager != null)
        {
            enemyManager.EnemyDied(this); // Notificar al EnemyManager que este enemigo ha muerto
        }

        // Instanciar la moneda en la posici�n del enemigo
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); // Destruir el objeto enemigo
    }
}