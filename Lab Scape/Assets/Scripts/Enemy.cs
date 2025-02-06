using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 3; // Salud máxima del enemigo
    private int currentHealth; // Salud actual del enemigo
    public float moveSpeed = 2f; // Velocidad de movimiento del enemigo
    private Transform player; // Referencia al transform del jugador

    public bool IsDead { get; private set; } = false; // Estado del enemigo
    private EnemyManager enemyManager;
    private Rigidbody2D rb; // Referencia al Rigidbody2D del enemigo

    public float bounceForce = 5f; // Fuerza de rebote al colisionar con el jugador

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

                // Calcular la dirección de retroceso
                Vector2 bounceDirection = (transform.position - other.transform.position).normalized;

                // Mover el enemigo un pequeño paso atrás
                float stepBackDistance = 0.5f; // Ajusta esta distancia según sea necesario
                transform.position += (Vector3)bounceDirection * stepBackDistance; // Mover el enemigo hacia atrás
            }
        }
    }

    // Método para recibir daño
    public void TakeDamage(int damage)
    {
        if (IsDead) return; // No recibir daño si ya está muerto

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
        Destroy(gameObject); // Destruir el objeto enemigo
    }
}