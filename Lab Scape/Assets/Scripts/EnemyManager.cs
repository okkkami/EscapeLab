using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo
    public int enemyCount = 5; // Número de enemigos a generar
    public GameObject keyPrefab; // Prefab de la llave
    public Transform keySpawnPoint; // Punto donde aparecerá la llave

    private List<Enemy> enemies = new List<Enemy>(); // Lista de enemigos activos
    private int defeatedEnemies; // Enemigos derrotados

    private void Start()
    {
        defeatedEnemies = 0; // Inicializar contadores

        // Verificar si ya se han derrotado enemigos
        if (PlayerPrefs.GetInt("EnemiesDefeated", 0) == 0)
        {
            SpawnEnemies(); // Solo generar enemigos si no han sido derrotados
        }
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyObject = Instantiate(enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            enemies.Add(enemy);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Genera una posición aleatoria dentro de un rango específico
        float x = Random.Range(-8f, 8f);
        float y = Random.Range(-4f, 4f);
        return new Vector3(x, y, 0);
    }

    public void EnemyDied(Enemy enemy)
    {
        enemies.Remove(enemy);
        defeatedEnemies++; // Incrementar el contador de enemigos derrotados

        // Guardar el estado de los enemigos en PlayerPrefs
        if (enemies.Count == 0)
        {
            PlayerPrefs.SetInt("EnemiesDefeated", 1); // Marcar que todos los enemigos han sido derrotados
            PlayerPrefs.Save(); // Guardar los cambios
            AppearKey(); // Aparecer la llave al derrotar al último enemigo
        }
    }

    private void AppearKey()
    {
        // Instanciar la llave en el punto de aparición
        Instantiate(keyPrefab, keySpawnPoint.position, Quaternion.identity);
    }

    public bool AreAllEnemiesDefeated()
    {
        return enemies.Count == 0; // Retorna true si no hay enemigos activos
    }

    public void ResetEnemies()
    {
        PlayerPrefs.SetInt("EnemiesDefeated", 0); // Restablecer el estado en PlayerPrefs
        defeatedEnemies = 0; // Reiniciar el contador de enemigos derrotados
        enemies.Clear(); // Limpiar la lista de enemigos
        SpawnEnemies(); // Regenerar enemigos si es necesario
    }
}