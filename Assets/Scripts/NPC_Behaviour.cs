using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Behaviour : MonoBehaviour
{
    public enum State { Patrolling, Chasing }
    public State currentState;


    [Header("Configuración de Visión")]
    public float sightRange = 10f;       // Distancia máxima
    public float fieldOfViewAngle = 90f; // Cono de visión (90 grados delante)
    public LayerMask obstacleLayer;     // Capa "Obstacle"

    [Header("Configuración de Ruta")]
    public Transform pathParent;
    public Transform player;
    public float guardSpeed = 3f;
    public float stopChaseRange = 15f;

    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = guardSpeed;
        foreach (Transform child in pathParent) waypoints.Add(child);
        currentState = State.Patrolling;
        MoveToNextWaypoint();
    }

    void Update()
    {
        if (currentState == State.Patrolling)
        {
            PatrolLogic();
            // Comprobamos visión en cada frame
            if (CanSeePlayer())
            {
                Debug.Log("¡TE VEO!"); // Mensaje en consola para confirmar
                currentState = State.Chasing;
            }
        }
        else if (currentState == State.Chasing)
        {
            ChaseLogic();
        }
    }

    bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. Distancia
        if (distanceToPlayer < sightRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // 2. Ángulo (¿Estás delante de él?)
            if (Vector3.Angle(transform.forward, directionToPlayer) < fieldOfViewAngle / 2)
            {
                // Definimos origen y destino del rayo (a la altura de los ojos, aprox 1.5m)
                // IMPORTANTE: Movemos el origen un poco adelante (+ transform.forward * 0.5f) 
                // para que el rayo no choque con el propio collider del guardia.
                Vector3 startPos = transform.position + Vector3.up * 1.5f + transform.forward * 0.5f;
                Vector3 targetPos = player.position + Vector3.up * 1.5f;

                RaycastHit hit;

                // Lanzamos el rayo contra TODO (sin filtro de capas por ahora para evitar errores)
                if (Physics.Linecast(startPos, targetPos, out hit))
                {
                    // Si lo PRIMERO que toca el rayo es el Player...
                    if (hit.transform.CompareTag("Player"))
                    {
                        Debug.DrawLine(startPos, targetPos, Color.green); // Línea VERDE: Te veo
                        return true;
                    }
                    else
                    {
                        Debug.DrawLine(startPos, hit.point, Color.red); // Línea ROJA: Bloqueado por pared
                        // Opcional: saber qué bloquea la visión
                        // Debug.Log("Visión bloqueada por: " + hit.transform.name);
                        return false;
                    }
                }
                else
                {
                    // Si no choca con nada (raro en Linecast si apuntamos al player), asumimos que te ve
                    Debug.DrawLine(startPos, targetPos, Color.green);
                    return true;
                }
            }
        }
        return false;
    }

    void PatrolLogic()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            MoveToNextWaypoint();
    }

    void ChaseLogic()
    {
        agent.SetDestination(player.position);
        if (Vector3.Distance(transform.position, player.position) > stopChaseRange)
        {
            currentState = State.Patrolling;
            MoveToNextWaypoint();
        }
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Count == 0) return;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
    }

    // Dibujado visual para que veas el rango en el editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Dibuja el ángulo de visión
        Vector3 leftBoundary = Quaternion.AngleAxis(-fieldOfViewAngle / 2, Vector3.up) * transform.forward;
        Vector3 rightBoundary = Quaternion.AngleAxis(fieldOfViewAngle / 2, Vector3.up) * transform.forward;
        Gizmos.DrawRay(transform.position, leftBoundary * sightRange);
        Gizmos.DrawRay(transform.position, rightBoundary * sightRange);
    }
}