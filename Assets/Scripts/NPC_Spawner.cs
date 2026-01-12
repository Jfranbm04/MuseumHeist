using UnityEngine;
using UnityEngine.AI;

public class NPC_Spawner : MonoBehaviour
{
    public GameObject npcPrefab; 
    public Transform player;      
    public Transform pathParent;  // Arrastra el objeto con los Waypoints
    public Vector2 mapSize = new Vector2(20, 20);

    void Start()
    {
        // 1. Leemos cuántos guardias hay que crear (por defecto 2 si algo falla)
        int numGuards = PlayerPrefs.GetInt("GuardsCount", 2);

        // 2. Los creamos
        for (int i = 0; i < numGuards; i++)
        {
            SpawnGuard();
        }
    }

    void SpawnGuard()
    {
        Vector3 randomPos = GetRandomPoint();
        GameObject newGuard = Instantiate(npcPrefab, randomPos, Quaternion.identity);

        // 3. IMPORTANTE: Configuramos las referencias del script del guardia automáticamente
        NPC_Behaviour behaviour = newGuard.GetComponent<NPC_Behaviour>();
        if (behaviour != null)
        {
            behaviour.player = player;
            behaviour.pathParent = pathParent;
        }
    }

    Vector3 GetRandomPoint()
    {
        float rx = Random.Range(-mapSize.x / 2, mapSize.x / 2);
        float rz = Random.Range(-mapSize.y / 2, mapSize.y / 2);
        Vector3 randomPos = new Vector3(rx, 0, rz) + transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 5.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return randomPos;
    }
}