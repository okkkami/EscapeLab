using UnityEngine;

public class MonedaPickup : MonoBehaviour
{
    public float floatStrength = 0.5f; // Fuerza del movimiento flotante
    public float floatSpeed = 2f; // Velocidad del movimiento flotante
    private Vector3 startPos; // Posici�n inicial de la moneda
    public AudioClip pickupSound; // Clip de audio para el sonido de recogida
    private AudioSource audioSource; // Componente AudioSource

    private void Start()
    {
        // Guardar la posici�n inicial de la moneda
        startPos = transform.position;

        // Obtener el componente AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = pickupSound; // Asignar el clip de audio
        audioSource.playOnAwake = false; // Asegurarse de que no se reproduzca al iniciar
    }

    private void Update()
    {
        // Movimiento de flotaci�n hacia arriba y hacia abajo
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = new Vector3(startPos.x, startPos.y + newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.CollectCoin(); // Llama al m�todo para recolectar la moneda

                // Reproducir el sonido de recogida
                if (pickupSound != null) // Verificar si el clip de audio est� asignado
                {
                    audioSource.Play(); // Reproducir el sonido
                }

                Destroy(gameObject, audioSource.clip.length); // Destruir la moneda despu�s de que se reproduzca el sonido
            }
        }
    }
}