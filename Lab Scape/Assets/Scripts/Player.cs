using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Asegúrate de incluir esto para trabajar con UI
using TMPro;

public class Player : MonoBehaviour
{
    // Variables de salud
    public int maxHealth = 6;  // Máxima salud del jugador
    private int currentHealth;  // Salud actual del jugador
    public GameObject[] cheeseIcons;  // Los iconos de los quesos en la UI

    // Variables de control del jugador
    public float initialMoveSpeed = 5f;  // Velocidad inicial del jugador
    private float currentMoveSpeed;  // Velocidad actual del jugador
    private Animator animator;

    private bool isGameOver = false; // Flag para saber si el juego ha terminado
    public GameObject gameOverText;  // Referencia al texto de Game Over
    public Button mainMenuButton;  // Cambia a Button en lugar de GameObject
    public TextMeshProUGUI totalScoreText; // Referencia al texto que mostrará la puntuación total

    private bool hasKey; // Indica si el jugador tiene la llave

    [Header("Fire Point")]
    [SerializeField] private Transform firePoint; // Único punto de disparo

    [Header("Shooting")]
    public GameObject bulletPrefab;  // Prefab de la bala
    public float bulletSpeed = 10f;  // Velocidad de la bala
    public float fireRate = 0.3f;    // Tiempo entre disparos
    private float nextFireTime = 0f; // Control de tiempo entre disparos

    private Vector2 lastMoveDirection = Vector2.right; // Última dirección de movimiento

    // Referencia al AudioSource
    private AudioSource audioSource; // Componente de AudioSource
    public AudioClip damageSound; // Clip de sonido para daño
    public AudioClip shootSound; // Clip de sonido para disparo

    public int coinCount; // Contador de monedas
    public TextMeshProUGUI MonedaTXT; // Referencia al texto de la UI



    private void Start()
    {

        

        // Recuperar la salud desde PlayerPrefs
        currentHealth = PlayerPrefs.GetInt("PlayerHealth", maxHealth); // Si no está guardada, se establece a maxHealth

        // Recuperar la velocidad desde PlayerPrefs
        currentMoveSpeed = PlayerPrefs.GetFloat("PlayerSpeed", initialMoveSpeed); // Si no está guardada, se establece a 5 por defecto
        animator = GetComponent<Animator>();

        // Asegúrate de que los objetos del UI no se muestren al inicio
        // Asignar el evento onClick al botón
        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(false); // Asegúrate de que el botón esté desactivado al inicio
            mainMenuButton.onClick.AddListener(LoadMainMenu); // Asigna el método al botón
        }

        if (gameOverText != null)
        {
            gameOverText.SetActive(false); // Asegúrate de que el texto de Game Over esté desactivado al inicio
        }

        if (totalScoreText != null)
        {
            totalScoreText.gameObject.SetActive(false); // Asegúrate de que el texto de puntuación total esté desactivado al inicio
        }

        UpdateCheeseUI(); // Asegúrate de que este método esté definido
        // Inicializar el estado de la llave
        hasKey = PlayerPrefs.GetInt("HasKey", 0) == 1; // 0 significa que no tiene la llave
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource

        // Cargar la última puerta cruzada
        string lastDoor = PlayerPrefs.GetString("LastDoor", "");
        MoveToSpawnPoint(lastDoor);
        mainMenuButton.gameObject.SetActive(false); // Asegúrate de que el botón esté desactivado al inicio
        mainMenuButton.onClick.AddListener(LoadMainMenu); // Asigna el método al botón

        // Cargar el recuento de monedas guardado
        coinCount = PlayerPrefs.GetInt("CoinCount", 0);
        UpdateCoinCountUI();

    

    }

    public void CollectCoin()
    {
        coinCount++; // Incrementar el contador de monedas
        UpdateCoinCountUI();
        SaveCoinCount(); // Guardar el recuento de monedas
    }

    private void UpdateCoinCountUI()
    {
        if (MonedaTXT != null)
        {
            MonedaTXT.text = coinCount.ToString(); // Actualizar el texto de la UI
        }
    }

    private void SaveCoinCount()
    {
        PlayerPrefs.SetInt("CoinCount", coinCount); // Guardar el recuento de monedas
        PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios
    }
    private void MoveToSpawnPoint(string lastDoor)
    {
        // Encuentra el punto de aparición correspondiente y mueve al jugador
        GameObject spawnPoint = null;

        switch (lastDoor)
        {
            case "doorR":
                spawnPoint = GameObject.Find("spawnLeft"); // Encuentra el punto de aparición a la izquierda
                break;
            case "doorL":
                spawnPoint = GameObject.Find("spawnRight"); // Encuentra el punto de aparición a la derecha
                break;
            case "doorU":
                spawnPoint = GameObject.Find("spawnDown"); // Encuentra el punto de aparición abajo
                break;
            case "doorD":
                spawnPoint = GameObject.Find("spawnUp"); // Encuentra el punto de aparición arriba
                break;
            case "DoorTesoroR":
                spawnPoint = GameObject.Find("spawnLeft"); // Salir por la puerta izquierda
                break;
            case "DoorTesoroL":
                spawnPoint = GameObject.Find("spawnRight"); // Salir por la puerta derecha
                break;
            case "doorInicialR":
                spawnPoint = GameObject.Find("spawnLeft");
                break;
            case "doorInicialL":
                spawnPoint = GameObject.Find("spawnRight");
                break;
        }

        if (spawnPoint != null)
        {
            // Mueve al jugador a la posición del punto de aparición
            transform.position = spawnPoint.transform.position;
        }
    }
    private void Update()
    {
        if (isGameOver) return; // Si el juego ha terminado, no procesamos más entradas
        Move();
        HandleShooting();
    }

    void Move()
    {
        float moveX = 0f;
        float moveY = 0f;

        // Captura la entrada del jugador
        if (Input.GetKey(KeyCode.A)) moveX = -5f;
        if (Input.GetKey(KeyCode.D)) moveX = 5f;
        if (Input.GetKey(KeyCode.W)) moveY = 5f;
        if (Input.GetKey(KeyCode.S)) moveY = -5f;

        // Crea un vector de movimiento normalizado
        Vector2 move = new Vector2(moveX, moveY).normalized * currentMoveSpeed * Time.deltaTime;

        // Obtiene el componente Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Mueve el jugador usando Rigidbody2D
            rb.MovePosition(rb.position + move);
        }

        // Guardar la última dirección de movimiento
        if (moveX != 0 || moveY != 0)
        {
            lastMoveDirection = new Vector2(moveX, moveY).normalized;
        }

        // Actualiza la animación
        UpdateAnimation(moveX, moveY);
    }

    void HandleShooting()
    {
        if (Time.time >= nextFireTime)
        {
            Vector2 shootDirection = Vector2.zero;

            if (Input.GetKey(KeyCode.UpArrow)) shootDirection = Vector2.up;
            if (Input.GetKey(KeyCode.DownArrow)) shootDirection = Vector2.down;
            if (Input.GetKey(KeyCode.LeftArrow)) shootDirection = Vector2.left;
            if (Input.GetKey(KeyCode.RightArrow)) shootDirection = Vector2.right;

            if (shootDirection != Vector2.zero)
            {
                Shoot(shootDirection);
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed; // La bala se mueve en la dirección seleccionada
        }

        nextFireTime = Time.time + fireRate; // Configurar el tiempo para el próximo disparo

        // Reproducir el sonido de disparo
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound); // Reproducir el sonido de disparo
        }
    }

    void UpdateAnimation(float moveX, float moveY)
    {
        animator.SetBool("isMoving", moveX != 0 || moveY != 0);
        animator.SetBool("isMovingUp", moveY > 0);
        animator.SetBool("isMovingDown", moveY < 0);
        animator.SetBool("isMovingLeft", moveX < 0);
        animator.SetBool("isMovingRight", moveX > 0);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Reducir la salud actual

        if (currentHealth < 0) currentHealth = 0;  // Asegurarse de que no baje de 0

        UpdateCheeseUI();  // Actualizar la UI de quesos

        // Reproducir el sonido de daño
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound); // Reproducir el sonido de daño
        }

        // Guardar la salud actual en PlayerPrefs
        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        PlayerPrefs.Save();

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
            cheeseIcons[i].SetActive(i < currentHealth);
        }
    }

    // Método para que el jugador reciba daño y muera
    public void Die()
    {
        DisablePlayerControls();  // Llamar al método que desactiva el control
        Destroy(gameObject); // Destruir el objeto del jugador
        
    }

    public void BoostSpeed(float amount)
    {
        currentMoveSpeed += amount; // Aumentar la velocidad actual del jugador
        PlayerPrefs.SetFloat("PlayerSpeed", currentMoveSpeed); // Guardar la nueva velocidad
        PlayerPrefs.Save(); // Asegurar que se guarda correctamente
        Debug.Log("Velocidad aumentada: " + currentMoveSpeed);
    }

    public void DisablePlayerControls()
    {
        isGameOver = true;  // Cambiar el flag a verdadero para bloquear entradas
        gameOverText.SetActive(true);  // Mostrar el texto de Game Over
        mainMenuButton.gameObject.SetActive(true);  // Mostrar el botón de "Main Menu"
        totalScoreText.gameObject.SetActive(true);
        Time.timeScale = 0f;  // Detener todo el tiempo del juego

        // Mostrar la puntuación total
        int totalScore = PlayerPrefs.GetInt("Score", 0); // Cargar la puntuación total desde PlayerPrefs
        totalScoreText.text = "Total score: " + totalScore; // Actualizar el texto de puntuación total
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;  // Asegurarse de que el tiempo se reinicie antes de cambiar de escena
        PlayerPrefs.SetFloat("PlayerSpeed", 5f); // Restablecer la velocidad del jugador a 5
        PlayerPrefs.SetInt("EnemiesDefeated", 0); // Restablecer el estado de los enemigos
        PlayerPrefs.Save(); // Guardar el valor restablecido
        SceneManager.LoadScene("MainMenu");
    }
    public void ObtenerLlave()
    {
        hasKey = true; // Indica que el jugador ahora tiene la llave
        PlayerPrefs.SetInt("HasKey", 1); // Guardar que el jugador tiene la llave
        PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios
        Debug.Log("El jugador ha obtenido la llave.");

        // Ocultar la imagen de la puerta bloqueada del tesoro
        Porta puerta = FindObjectOfType<Porta>(); // Encuentra la instancia de Porta
        if (puerta != null)
        {
            // puerta.HideBlockedDoorImageTesoro(); // Ocultar la imagen de la puerta bloqueada del tesoro
            puerta.UnlockDoor(); // Desbloquear la puerta
        }



    }

    public bool HasKey()
    {
        return hasKey; // Método para verificar si el jugador tiene la llave
    }
    // Método para restablecer la salud
    public void ResetHealth()
    {
        currentHealth = maxHealth; // Restablecer la salud al máximo
        PlayerPrefs.SetInt("PlayerHealth", currentHealth); // Guardar la salud en PlayerPrefs
        PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios
        UpdateCheeseUI(); // Actualizar la UI de quesos
    }
}