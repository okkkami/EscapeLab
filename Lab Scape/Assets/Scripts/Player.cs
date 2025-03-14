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
    public float initialMoveSpeed = 30f;  // Velocidad inicial del jugador
    private float currentMoveSpeed;  // Velocidad actual del jugador
    private Animator animator;

    private bool isGameOver = false; // Flag para saber si el juego ha terminado
    public GameObject gameOverText;  // Referencia al texto de Game Over
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

    public int coinCount; // Contador de monedas
    public TextMeshProUGUI MonedaTXT; // Referencia al texto de la UI

    private AudioSource footstepAudioSource; // AudioSource para pasos
    private AudioSource hurtAudioSource; // AudioSource para da�o
    private AudioSource spitAudioSource; // AudioSource para disparo



    private void Start()
    {
         // Recuperar la salud desde PlayerPrefs
        currentHealth = PlayerPrefs.GetInt("PlayerHealth", maxHealth); // Si no est� guardada, se establece a maxHealth

        // Recuperar la velocidad desde PlayerPrefs
        currentMoveSpeed = PlayerPrefs.GetFloat("PlayerSpeed", initialMoveSpeed); // Si no est� guardada, se establece a 5 por defecto
        animator = GetComponent<Animator>();

        // Aseg�rate de que los objetos del UI no se muestren al inicio
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

        //Sonidos
        footstepAudioSource = GetComponents<AudioSource>()[0]; // Asumiendo que el primer AudioSource es para pasos
        hurtAudioSource = GetComponents<AudioSource>()[1]; 
        spitAudioSource = GetComponents<AudioSource>()[2]; 

        // Cargar la �ltima puerta cruzada
        string lastDoor = PlayerPrefs.GetString("LastDoor", "");
        MoveToSpawnPoint(lastDoor);

        // Cargar el recuento de monedas guardado
        coinCount = PlayerPrefs.GetInt("CoinCount", 0);
        UpdateCoinCountUI();
    }

    // M�todo para reproducir el sonido de los pasos
    public void Step()
    {
        if (!footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Play(); // Reproduce el sonido de los pasos
        }
    }

    // M�todo para detener el sonido de los pasos
    public void StopFootstepSound()
    {
        footstepAudioSource.Stop(); // Detiene el sonido de los pasos
    }

    // M�todo para reproducir sonido da�o
    public void PlayHurtSound()
    {
        if (hurtAudioSource != null && !hurtAudioSource.isPlaying)
        {
            hurtAudioSource.Play(); // Reproduce otros sonidos
        }
    }


    public void PlaySpitSound()
    {
        if (spitAudioSource != null && !spitAudioSource.isPlaying)
        {
            spitAudioSource.Play(); 
        }
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

        // Guardar la �ltima direcci�n de movimiento
        if (moveX != 0 || moveY != 0)
        {
            lastMoveDirection = new Vector2(moveX, moveY).normalized;
        }

        // Actualiza la animaci�n
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
        PlaySpitSound();
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

        PlayHurtSound(); // Reproduce el sonido del da�o

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
        totalScoreText.gameObject.SetActive(true);
        Time.timeScale = 0f;  // Detener todo el tiempo del juego

        // Mostrar la puntuaci�n total
        int totalScore = PlayerPrefs.GetInt("Score", 0); // Cargar la puntuaci�n total desde PlayerPrefs
        totalScoreText.text = "Total score: " + totalScore; // Actualizar el texto de puntuaci�n total
    }

    public void ObtenerLlave()
    {
        hasKey = true; // Indica que el jugador ahora tiene la llave
        PlayerPrefs.SetInt("HasKey", 1); // Guardar que el jugador tiene la llave
        PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios
        Debug.Log("El jugador ha obtenido la llave.");

       
        Porta puerta = FindObjectOfType<Porta>(); // Encuentra la instancia de Porta
        if (puerta != null)
        {
            // puerta.HideBlockedDoorImageTesoro(); // Ocultar la imagen de la puerta bloqueada del tesoro
            puerta.UnlockDoor(); // Desbloquear la puerta
        }
    }

    public void OnIdle()
    {
        StopFootstepSound(); // Detiene el sonido de los pasos
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