using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; // Referencia al componente Text de la UI
    private int score; // Puntuación actual

    private void Start()
    {
        // Cargar la puntuación desde PlayerPrefs
        score = PlayerPrefs.GetInt("Score", 0); // Cargar la puntuación, 0 si no existe
        UpdateScoreText(); // Actualizar el texto de la puntuación
    }

    public void AddScore(int points)
    {
        score += points; // Sumar puntos
        UpdateScoreText(); // Actualizar el texto de la puntuación

        // Guardar la puntuación en PlayerPrefs
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save(); // Asegurarse de que se guarden los cambios
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Puntos: " + score; // Actualizar el texto en la UI
    }
}