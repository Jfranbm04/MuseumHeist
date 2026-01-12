using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Behaviour : MonoBehaviour
{
    public enum State { Patrolling, Chasing }
    public State currentState;

    [Header("Configuración de Visión")]
    public float sightRange = 10f;
    public float fieldOfViewAngle = 90f;
    public LayerMask obstacleLayer;

    [Header("Configuración de Ruta")]
    public Transform pathParent;
    public Transform player;
    public float guardSpeed = 3f;
    public float stopChaseRange = 15f;

    private List<Transform> waypoints = new List<Transform>();
    private List<int> pendingIndices = new List<int>(); // Lista de índices que faltan por visitar
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = guardSpeed;

        // Llenamos la lista original de waypoints
        foreach (Transform child in pathParent) waypoints.Add(child);

        currentState = State.Patrolling;

        // Iniciamos la primera patrulla aleatoria
        ResetPendingIndices();
        MoveToNextRandomWaypoint();
    }

    void Update()
    {
        if (currentState == State.Patrolling)
        {
            PatrolLogic();
            if (CanSeePlayer())
            {
                currentState = State.Chasing;
            }
        }
        else if (currentState == State.Chasing)
        {
            ChaseLogic();
        }
    }

    void ResetPendingIndices()
    {
        pendingIndices.Clear();
        for (int i = 0; i < waypoints.Count; i++)
        {
            pendingIndices.Add(i);
        }
    }

    void MoveToNextRandomWaypoint()
    {
        if (waypoints.Count == 0) return;

        // Si ya visitamos todos, reiniciamos la lista para un nuevo ciclo
        if (pendingIndices.Count == 0)
        {
            ResetPendingIndices();
        }

        // Elegimos un índice al azar de los que quedan
        int randomIndexInList = Random.Range(0, pendingIndices.Count);
        int waypointIndex = pendingIndices[randomIndexInList];

        // Le decimos al agente que vaya allí
        agent.SetDestination(waypoints[waypointIndex].position);

        // Eliminamos ese índice para no repetirlo en este ciclo
        pendingIndices.RemoveAt(randomIndexInList);
    }

    void PatrolLogic()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            MoveToNextRandomWaypoint();
    }


    bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < sightRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToPlayer) < fieldOfViewAngle / 2)
            {
                Vector3 startPos = transform.position + Vector3.up * 1.5f + transform.forward * 0.5f;
                Vector3 targetPos = player.position + Vector3.up * 1.5f;
                RaycastHit hit;
                if (Physics.Linecast(startPos, targetPos, out hit))
                {
                    if (hit.transform.CompareTag("Player")) return true;
                }
                else return true;
            }
        }
        return false;
    }

    void ChaseLogic()
    {
        agent.SetDestination(player.position);
        if (Vector3.Distance(transform.position, player.position) > stopChaseRange)
        {
            currentState = State.Patrolling;
            MoveToNextRandomWaypoint();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Vector3 leftBoundary = Quaternion.AngleAxis(-fieldOfViewAngle / 2, Vector3.up) * transform.forward;
        Vector3 rightBoundary = Quaternion.AngleAxis(fieldOfViewAngle / 2, Vector3.up) * transform.forward;
        Gizmos.DrawRay(transform.position, leftBoundary * sightRange);
        Gizmos.DrawRay(transform.position, rightBoundary * sightRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.PlayerCaught();
        }
    }
}