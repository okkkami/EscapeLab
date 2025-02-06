using UnityEngine;

public class ButterPickup : MonoBehaviour
{
    public float speedBoost = 1.0f; // Cantidad de aumento de velocidad

    public float floatStrength = 0.5f; // Fuerza del movimiento flotante
    public float floatSpeed = 2f; // Velocidad del movimiento flotante

    private Vector3 startPos; // Posición inicial del objeto

    private void Start()
    {
        startPos = transform.position; // Guardar la posición inicial al comenzar
    }

    private void Update()
    {
        // Movimiento de flotación hacia arriba y hacia abajo
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = new Vector3(startPos.x, startPos.y + newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>(); 
            if (player != null)
            {
                player.BoostSpeed(speedBoost);  // Llamar a BoostSpeed para aumentar la velocidad
                Destroy(gameObject); // Destruir la mantequilla después de recogerla
            }
        }
    }
}




