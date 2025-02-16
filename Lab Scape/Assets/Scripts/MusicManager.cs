using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Instancia del Singleton
    public AudioSource audioSource; // Asigna el AudioSource en el Inspector
    public AudioClip initialMusic; // M�sica inicial
    public AudioClip newMusic; // Nueva m�sica
    private bool isMuted = false; // Estado de mute

    private void Awake()
    {
        // Verifica si ya existe una instancia de AudioManager
        if (Instance == null)
        {
            Instance = this; // Asigna la instancia
            DontDestroyOnLoad(gameObject); // No destruir este objeto al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Destruye el objeto si ya existe una instancia
        }
    }
    private void Start()
    {
        PlayInitialMusic(); // Reproduce la m�sica inicial al iniciar
    }
    private void Update()
    {
        // Verifica si se presiona la tecla M
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted; // Cambia el estado de mute
        audioSource.mute = isMuted; // Aplica el estado de mute al AudioSource
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume; // M�todo para establecer el volumen
    }

    public void PlayInitialMusic()
    {
        audioSource.clip = initialMusic; // Asigna la m�sica inicial
        audioSource.loop = true; // Reproduce en bucle
        audioSource.Play(); // Inicia la reproducci�n
    }

    public void ChangeMusic()
    {
        audioSource.Stop(); // Detiene la m�sica actual
        audioSource.clip = newMusic; // Cambia al nuevo clip de m�sica
        audioSource.loop = true; // Reproduce en bucle
        audioSource.Play(); // Inicia la nueva m�sica
    }
}