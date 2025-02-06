using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;  // M�xima salud del jugador
    private int currentHealth;  // Salud actual del jugador

    public GameObject[] cheeseIcons;  // Los iconos de los quesos en la UI
    private PlayerController playerController;

    void Start()
    {
        currentHealth = maxHealth;  // La salud inicial es la m�xima
        playerController = GetComponent<PlayerController>(); // Obtenemos la referencia del PlayerController
        UpdateCheeseUI();
    }

    // M�todo para recibir da�o
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Reducir la salud actual

        if (currentHealth < 0) currentHealth = 0;  // Asegurarse de que no baje de 0

        UpdateCheeseUI();  // Actualizar la UI de quesos

        if (currentHealth == 0)
        {
            Die(); // Si la salud es 0, se llama al m�todo Die
        }
    }

    // M�todo para actualizar la UI cuando el jugador recibe da�o
    void UpdateCheeseUI()
    {
        // Recorrer todos los iconos de queso y activar/desactivar seg�n la salud
        for (int i = 0; i < cheeseIcons.Length; i++)
        {
            // Si la salud es mayor que el �ndice, el queso sigue visible
            if (i < currentHealth)
            {
                cheeseIcons[i].SetActive(true);
            }
            else
            {
                cheeseIcons[i].SetActive(false);  // Desactivar el queso cuando el jugador recibe da�o
            }
        }
    }

    // M�todo para que el jugador reciba da�o y muera
    public void Die()
    {
        playerController.DisablePlayerControls();  // Llamar al m�todo que desactiva el control desde PlayerController
    }
}
