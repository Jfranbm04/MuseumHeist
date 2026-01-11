using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Behaviour : MonoBehaviour
{
    public enum State { Patrolling, Chasing }
    public State currentState;

    [Header("Configuración")]
    public Transform pathParent;
    public Transform player;
    public float sightRange = 5f; // Distancia para empezar a seguirte
    public float stopChaseRange = 5f; // Distancia para dejar de seguirte

    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Cargar puntos de ruta
        foreach (Transform child in pathParent) waypoints.Add(child);

        currentState = State.Patrolling;
        MoveToNextWaypoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (currentState == State.Patrolling)
        {
            // Lógica de patrulla
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                MoveToNextWaypoint();

            // Si el jugador entra en rango, perseguir
            if (distanceToPlayer < sightRange)
            {
                currentState = State.Chasing;
            }
        }
        else if (currentState == State.Chasing)
        {
            // Perseguir al jugador
            agent.SetDestination(player.position);

            // Si el jugador se aleja demasiado, volver a patrullar
            if (distanceToPlayer > stopChaseRange)
            {
                currentState = State.Patrolling;
                MoveToNextWaypoint();
            }
        }
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Count == 0) return;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
    }
}