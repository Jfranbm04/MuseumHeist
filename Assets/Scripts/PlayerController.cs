using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Camera cam;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
        // Asegúrate de poner el Tag "Jewel" a tus joyas
        if (other.CompareTag("Jewel"))
        {
            Debug.Log("¡Joya robada: " + other.name + "!");
            Destroy(other.gameObject); // Elimina el objeto de la escena
            // Aquí podrías sumar puntos a un ScoreManager
        }
    }
}