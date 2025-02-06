using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 3; // Salud m�xima del enemigo
    private int currentHealth; // Salud actual del enemigo
    public float moveSpeed = 2f; // Velocidad de movimiento del enemigo
    private Transform player; // Referencia al transform del jugador

    public bool IsDead { get; private set; } = false; // Estado del enemigo
    private EnemyManager enemyManager;

    private void Start()
    {
        currentHealth = maxHealth; // Inicializar la salud actual
        player = GameObject.FindGameObjectWithTag("Player").transform; // Buscar el jugador
        enemyManager = FindObjectOfType<EnemyManager>();
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
        Destroy(gameObject); // Destruir el objeto enemigo
    }

}