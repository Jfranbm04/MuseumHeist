using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 5f;
    private NavMeshAgent agent;
    private Camera cam;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = playerSpeed;
        cam = Camera.main;
    }

    void Update()
    {
        // Movimiento con Click
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    // Recoger objetos al chocar
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jewel"))
        {
            Debug.Log("¡Joya robada: " + other.name + "!");

            if (GameManager.instance != null)
            {
                GameManager.instance.CollectGem();
            }

            Destroy(other.gameObject); 
        }
    }
}