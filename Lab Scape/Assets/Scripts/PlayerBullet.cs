using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 1; // Daño del proyectil

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy")) // Verificamos si ha impactado con el enemigo
        {
            Debug.Log("Impacto con enemigo!");
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Restamos vida al enemigo
            }

            // Eliminar el proyectil después del impacto
            Destroy(gameObject);
        }
    }
}