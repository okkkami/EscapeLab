using UnityEngine;

public class Key : MonoBehaviour
{
    public float floatStrength = 0.5f; // Fuerza del movimiento flotante
    public float floatSpeed = 2f; // Velocidad del movimiento flotante
    private Vector3 startPos; // Posición inicial de la llave
    public AudioClip pickupSound; // Clip de audio para el sonido de recogida
    private AudioSource audioSource; // Componente AudioSource

    private void Start()
    {
        // Guardar la posición inicial de la llave
        startPos = transform.position;

        // Obtener el componente AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = pickupSound; // Asignar el clip de audio
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
            Debug.Log("Has recogido la llave.");
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.ObtenerLlave(); // Dar la llave al jugador
            }
            PlayerPrefs.SetInt("HasKey", 1); // Guardar que el jugador tiene la llave
            PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios

            // Reproducir el sonido de recogida
            audioSource.Play();

            // Destruir el objeto de la llave después de que el sonido se haya reproducido
            Destroy(gameObject, audioSource.clip.length); // Destruir después de que el sonido termine
        }
    }
}