using UnityEngine;
using UnityEngine.AI; 

public class GemSpawner : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject gemPrefab;
    public int numberOfGems = 10; // Cantidad a generar
    public Vector2 mapSize = new Vector2(20, 20); // Tamaño aproximado del mapa (X, Z)
    public float heightOffset = 0.5f;

    

    void Start()
    {
        SpawnGems();
    }

    void SpawnGems()
    {
        for (int i = 0; i < numberOfGems; i++)
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh();

            Vector3 spawnPosition = new Vector3(randomPoint.x, 0f, randomPoint.z);

            Instantiate(gemPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPointOnNavMesh()
    {
        // 1. Elegimos una posición aleatoria dentro de un cubo imaginario
        float randomX = Random.Range(-mapSize.x / 2, mapSize.x / 2);
        float randomZ = Random.Range(-mapSize.y / 2, mapSize.y / 2);
        Vector3 randomPos = new Vector3(randomX, 0, randomZ) + transform.position;

        // 2. Le preguntamos al NavMesh: "¿Cuál es el punto válido más cercano a esto?"
        NavMeshHit hit;
        // El 2.0f es el radio de búsqueda. Si cae dentro de un muro, busca suelo a 2 metros.
        if (NavMesh.SamplePosition(randomPos, out hit, 4.0f, NavMesh.AllAreas))
        {
            return hit.position; 
        }
        return randomPos;
    }

    // Dibujamos el área en el editor para que sepas dónde van a aparecer
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(mapSize.x, 1, mapSize.y));
    }
}