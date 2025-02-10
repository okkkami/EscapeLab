using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;

    private void Update()
    {
        Collider2D playerCollider = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Collider2D>();

        if (playerCollider != null)
        {
            if (GetComponent<Collider2D>().IsTouching(playerCollider))
            {
                Debug.Log("¡El jugador está tocando la moneda!");
                GameManager.instance.AddCoins(coinValue);
                Destroy(gameObject);
            }
        }
    }
}
