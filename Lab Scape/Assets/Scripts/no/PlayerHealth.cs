using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;  // Máxima salud del jugador
    private int currentHealth;  // Salud actual del jugador

    public GameObject[] cheeseIcons;  // Los iconos de los quesos en la UI
    private PlayerController playerController;

    void Start()
    {
        currentHealth = maxHealth;  // La salud inicial es la máxima
        playerController = GetComponent<PlayerController>(); // Obtenemos la referencia del PlayerController
        UpdateCheeseUI();
    }

    // Método para recibir daño
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Reducir la salud actual

        if (currentHealth < 0) currentHealth = 0;  // Asegurarse de que no baje de 0

        UpdateCheeseUI();  // Actualizar la UI de quesos

        if (currentHealth == 0)
        {
            Die(); // Si la salud es 0, se llama al método Die
        }
    }

    // Método para actualizar la UI cuando el jugador recibe daño
    void UpdateCheeseUI()
    {
        // Recorrer todos los iconos de queso y activar/desactivar según la salud
        for (int i = 0; i < cheeseIcons.Length; i++)
        {
            // Si la salud es mayor que el índice, el queso sigue visible
            if (i < currentHealth)
            {
                cheeseIcons[i].SetActive(true);
            }
            else
            {
                cheeseIcons[i].SetActive(false);  // Desactivar el queso cuando el jugador recibe daño
            }
        }
    }

    // Método para que el jugador reciba daño y muera
    public void Die()
    {
        playerController.DisablePlayerControls();  // Llamar al método que desactiva el control desde PlayerController
    }
}
