using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button optionsButton;
    public Button exitButton;
    public AudioSource hoverSound;

    void Start()
    {
        // Asigna los efectos a los botones
        AssignButtonEffects(level1Button);
        //AssignButtonEffects(level2Button);
        AssignButtonEffects(optionsButton);
        AssignButtonEffects(exitButton);

        // Asigna eventos de clic
        level1Button.onClick.AddListener(PlayGame);
        optionsButton.onClick.AddListener(OpenOptions);
        exitButton.onClick.AddListener(ExitGame);
    }

    void AssignButtonEffects(Button button)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // Evento al pasar el cursor (hover)
        EventTrigger.Entry entryHover = new EventTrigger.Entry();
        entryHover.eventID = EventTriggerType.PointerEnter;
        entryHover.callback.AddListener((data) => { OnHover(); });
        trigger.triggers.Add(entryHover);
    }

    void OnHover()
    {
        // Reproduce el sonido
        if (hoverSound) hoverSound.Play();
    }

    void PlayGame()
    {
        Debug.Log("Play Game");

        // Asegurarse de que la velocidad se restablezca a 5 antes de cargar la escena
        PlayerPrefs.SetFloat("PlayerSpeed", 5f);  // Establecer la velocidad a 5
        PlayerPrefs.DeleteAll(); // Limpiar todos los PlayerPrefs para restablecer el estado de los enemigos
        PlayerPrefs.Save();  // Guardar la configuración

        // Reiniciar el estado de la llave
        PlayerPrefs.SetInt("HasKey", 0); // El jugador no tiene la llave al comenzar una nueva partida
        PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios

        // Restablecer la salud del jugador
        PlayerPrefs.SetInt("PlayerHealth", 6); // Restablecer la salud a su valor máximo
        PlayerPrefs.SetInt("HasKey", 0); // Restablecer el estado de la llave
        PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios



        SceneManager.LoadScene("hab_inicial");
    }


    void OpenOptions()
    {
        Debug.Log("Open Options");
        SceneManager.LoadScene("Options");
    }

    void ExitGame()
    {
        Debug.Log("Exit Game");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}

