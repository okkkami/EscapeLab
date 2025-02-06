using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Velocidad del jugador
    private Animator animator;
    private bool isGameOver = false; // Flag para saber si el juego ha terminado
    public GameObject gameOverText;  // Referencia al texto de Game Over
    public GameObject mainMenuButton;  // Referencia al botón de "Main Menu"

    [Header("Fire Point")]
    [SerializeField] private Transform firePoint; // Único punto de disparo

    [Header("Shooting")]
    public GameObject bulletPrefab;  // Prefab de la bala
    public float bulletSpeed = 10f;  // Velocidad de la bala
    public float fireRate = 0.3f;    // Tiempo entre disparos
    private float nextFireTime = 0f; // Control de tiempo entre disparos

    private Vector2 lastMoveDirection = Vector2.right; // Última dirección de movimiento

    private void Start()
    {
        // Asegúrate de que los objetos del UI no se muestren al inicio
        gameOverText.SetActive(false);
        mainMenuButton.SetActive(false);

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator no encontrado. Asigna el componente Animator al GameObject.");
        }

        // Recuperar la velocidad desde PlayerPrefs
        moveSpeed = PlayerPrefs.GetFloat("PlayerSpeed", 5f); // Si no está guardada, se establece a 5 por defecto
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

        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;

        Vector3 move = new Vector3(moveX, moveY, 0f).normalized * moveSpeed * Time.deltaTime;
        transform.Translate(move);

        // Guardar la última dirección de movimiento (evita disparar en dirección incorrecta)
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
            rb.velocity = direction * bulletSpeed; // La bala se mueve en la dirección seleccionada
        }

        nextFireTime = Time.time + fireRate; // Configurar el tiempo para el próximo disparo
    }

    void UpdateAnimation(float moveX, float moveY)
    {
        animator.SetBool("isMoving", moveX != 0 || moveY != 0);
        animator.SetBool("isMovingUp", moveY > 0);
        animator.SetBool("isMovingDown", moveY < 0);
        animator.SetBool("isMovingLeft", moveX < 0);
        animator.SetBool("isMovingRight", moveX > 0);
    }

    public void BoostSpeed()
    {
        moveSpeed *= 2; // Duplicar la velocidad actual del jugador
        PlayerPrefs.SetFloat("PlayerSpeed", moveSpeed); // Guardar la nueva velocidad
        PlayerPrefs.Save(); // Asegurar que se guarda correctamente
        Debug.Log("Velocidad aumentada: " + moveSpeed);
    }

    public void DisablePlayerControls()
    {
        isGameOver = true;  // Cambiar el flag a verdadero para bloquear entradas
        gameOverText.SetActive(true);  // Mostrar el texto de Game Over
        mainMenuButton.SetActive(true);  // Mostrar el botón de "Main Menu"
        Time.timeScale = 0f;  // Detener todo el tiempo del juego
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;  // Asegurarse de que el tiempo se reinicie antes de cambiar de escena
        PlayerPrefs.SetFloat("PlayerSpeed", 5f); // Restablecer la velocidad del jugador a 5
        PlayerPrefs.Save(); // Guardar el valor restablecido
        SceneManager.LoadScene("MainMenu");  // Cambiar a la escena del menú principal
    }
}

