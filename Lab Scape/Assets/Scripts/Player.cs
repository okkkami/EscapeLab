using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Aseg�rate de incluir esto para trabajar con UI
using TMPro;

public class Player : MonoBehaviour
{
    // Variables de salud
    public int maxHealth = 6;  // M�xima salud del jugador
    private int currentHealth;  // Salud actual del jugador
    public GameObject[] cheeseIcons;  // Los iconos de los quesos en la UI

    // Variables de control del jugador
    public float initialMoveSpeed = 5f;  // Velocidad inicial del jugador
    private float currentMoveSpeed;  // Velocidad actual del jugador
    private Animator animator;

    private bool isGameOver = false; // Flag para saber si el juego ha terminado
    public GameObject gameOverText;  // Referencia al texto de Game Over
    public Button mainMenuButton;  // Cambia a Button en lugar de GameObject
    public TextMeshProUGUI totalScoreText; // Referencia al texto que mostrar� la puntuaci�n total

    private bool hasKey; // Indica si el jugador tiene la llave

    [Header("Fire Point")]
    [SerializeField] private Transform firePoint; // �nico punto de disparo

    [Header("Shooting")]
    public GameObject bulletPrefab;  // Prefab de la bala
    public float bulletSpeed = 10f;  // Velocidad de la bala
    public float fireRate = 0.3f;    // Tiempo entre disparos
    private float nextFireTime = 0f; // Control de tiempo entre disparos

    private Vector2 lastMoveDirection = Vector2.right; // �ltima direcci�n de movimiento

    // Referencia al AudioSource
    private AudioSource audioSource; // Componente de AudioSource
    public AudioClip damageSound; // Clip de sonido para da�o
    public AudioClip shootSound; // Clip de sonido para disparo



    private void Start()
    {
        // Recuperar la salud desde PlayerPrefs
        currentHealth = PlayerPrefs.GetInt("PlayerHealth", maxHealth); // Si no est� guardada, se establece a maxHealth

        // Recuperar la velocidad desde PlayerPrefs
        currentMoveSpeed = PlayerPrefs.GetFloat("PlayerSpeed", initialMoveSpeed); // Si no est� guardada, se establece a 5 por defecto
        animator = GetComponent<Animator>();

        // Aseg�rate de que los objetos del UI no se muestren al inicio
        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(false); // Aseg�rate de que el bot�n est� desactivado al inicio
            mainMenuButton.onClick.AddListener(LoadMainMenu); // Asigna el m�todo al bot�n
        }

        if (gameOverText != null)
        {
            gameOverText.SetActive(false); // Aseg�rate de que el texto de Game Over est� desactivado al inicio
        }

        if (totalScoreText != null)
        {
            totalScoreText.gameObject.SetActive(false); // Aseg�rate de que el texto de puntuaci�n total est� desactivado al inicio
        }

        UpdateCheeseUI(); // Aseg�rate de que este m�todo est� definido
        // Inicializar el estado de la llave
        hasKey = PlayerPrefs.GetInt("HasKey", 0) == 1; // 0 significa que no tiene la llave
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource

        // Cargar la �ltima puerta cruzada
        string lastDoor = PlayerPrefs.GetString("LastDoor", "");
        MoveToSpawnPoint(lastDoor);
    }

    private void MoveToSpawnPoint(string lastDoor)
    {
        // Encuentra el punto de aparici�n correspondiente y mueve al jugador
        GameObject spawnPoint = null;

        switch (lastDoor)
        {
            case "doorR":
                spawnPoint = GameObject.Find("spawnLeft"); // Encuentra el punto de aparici�n a la izquierda
                break;
            case "doorL":
                spawnPoint = GameObject.Find("spawnRight"); // Encuentra el punto de aparici�n a la derecha
                break;
            case "doorU":
                spawnPoint = GameObject.Find("spawnDown"); // Encuentra el punto de aparici�n abajo
                break;
            case "doorD":
                spawnPoint = GameObject.Find("spawnUp"); // Encuentra el punto de aparici�n arriba
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
            // Mueve al jugador a la posici�n del punto de aparici�n
            transform.position = spawnPoint.transform.position;
        }
    }
    private void Update()
    {
        if (isGameOver) return; // Si el juego ha terminado, no procesamos m�s entradas
        Move();
        HandleShooting();
    }

    void Move()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;

        Vector3 move = new Vector3(moveX, moveY, 0f).normalized * currentMoveSpeed * Time.deltaTime;
        transform.Translate(move);

        // Guardar la �ltima direcci�n de movimiento (evita disparar en direcci�n incorrecta)
        if (moveX != 0 || moveY != 0)
        {
            lastMoveDirection = new Vector2(moveX, moveY).normalized;
        }

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
            rb.velocity = direction * bulletSpeed; // La bala se mueve en la direcci�n seleccionada
        }

        nextFireTime = Time.time + fireRate; // Configurar el tiempo para el pr�ximo disparo

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

        // Reproducir el sonido de da�o
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound); // Reproducir el sonido de da�o
        }

        // Guardar la salud actual en PlayerPrefs
        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        PlayerPrefs.Save();

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
            cheeseIcons[i].SetActive(i < currentHealth);
        }
    }

    // M�todo para que el jugador reciba da�o y muera
    public void Die()
    {
        DisablePlayerControls();  // Llamar al m�todo que desactiva el control
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
        mainMenuButton.gameObject.SetActive(true);  // Mostrar el bot�n de "Main Menu"
        totalScoreText.gameObject.SetActive(true);
        Time.timeScale = 0f;  // Detener todo el tiempo del juego

        // Mostrar la puntuaci�n total
        int totalScore = PlayerPrefs.GetInt("Score", 0); // Cargar la puntuaci�n total desde PlayerPrefs
        totalScoreText.text = "Puntos Totales: " + totalScore; // Actualizar el texto de puntuaci�n total
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;  // Asegurarse de que el tiempo se reinicie antes de cambiar de escena
        PlayerPrefs.SetFloat("PlayerSpeed", 5f); // Restablecer la velocidad del jugador a 5
        PlayerPrefs.SetInt("EnemiesDefeated", 0); // Restablecer el estado de los enemigos
        PlayerPrefs.Save(); // Guardar el valor restablecido
        SceneManager.LoadScene("MainMenu");  // Cambiar a la escena del men� principal

    }
    public void ObtenerLlave()
    {
        hasKey = true; // Indica que el jugador ahora tiene la llave
        PlayerPrefs.SetInt("HasKey", 1); // Guardar que el jugador tiene la llave
        PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios
        Debug.Log("El jugador ha obtenido la llave.");
    }

    public bool HasKey()
    {
        return hasKey; // M�todo para verificar si el jugador tiene la llave
    }
    // M�todo para restablecer la salud
    public void ResetHealth()
    {
        currentHealth = maxHealth; // Restablecer la salud al m�ximo
        PlayerPrefs.SetInt("PlayerHealth", currentHealth); // Guardar la salud en PlayerPrefs
        PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios
        UpdateCheeseUI(); // Actualizar la UI de quesos
    }
}