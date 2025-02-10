using UnityEngine;
using TMPro; // Para usar TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int coinCount = 0; // Cantidad de monedas
    public TextMeshProUGUI coinText; // Referencia al texto en la UI

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        UpdateCoinUI();
    }

    public void RemoveCoins(int amount)
    {
        coinCount = Mathf.Max(0, coinCount - amount); // Evita que el número sea negativo
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coinCount.ToString();
        }
    }
}

