using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; // Referencia al componente Text de la UI
    private int score; // Puntuaci�n actual

    private void Start()
    {
        // Cargar la puntuaci�n desde PlayerPrefs
        score = PlayerPrefs.GetInt("Score", 0); // Cargar la puntuaci�n, 0 si no existe
        UpdateScoreText(); // Actualizar el texto de la puntuaci�n
    }

    public void AddScore(int points)
    {
        score += points; // Sumar puntos
        UpdateScoreText(); // Actualizar el texto de la puntuaci�n

        // Guardar la puntuaci�n en PlayerPrefs
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Puntos: " + score; // Actualizar el texto en la UI
    }
}