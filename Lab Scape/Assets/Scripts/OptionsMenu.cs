using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public Button Music;
    public Button Controls;
    public Button Website;
    public Button Return;
    public AudioSource hoverSound;

    void Start()
    {
        // Asigna los efectos a los botones
        AssignButtonEffects(Music);
        //AssignButtonEffects(level2Button);
        AssignButtonEffects(Website);
        AssignButtonEffects(Return);
        AssignButtonEffects(Controls);

        // Asigna eventos de clic
        Music.onClick.AddListener(OpenMusic);
        Website.onClick.AddListener(OpenWebsite);
        Return.onClick.AddListener(OpenMainMenu);
        Controls.onClick.AddListener(OpenControls);
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

    void OpenMusic()
    {
         



        SceneManager.LoadScene("Music");
    }

    void OpenControls()
    {




        SceneManager.LoadScene("Controls");
    }



    void OpenWebsite()
    {
        
        SceneManager.LoadScene("Website");
    }

    void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");

    }
}

