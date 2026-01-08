using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Behaviour : MonoBehaviour
{

    [SerializeField] private Vector3 destination;
    [SerializeField] private Vector3 min, max;
    [SerializeField] private GameObject player;

    [SerializeField] private int childrenIndex;
    [SerializeField] private Transform path;

    [SerializeField] private float playerDetectionDistance;
    [SerializeField] private bool playerDetected;

    private Coroutine runningPatroll;


    public void Start()
    {
        //destination = RandomDestination();
        //GetComponent<NavMeshAgent>().SetDestination(destination);

        //StartCoroutine("Follow");

        runningPatroll = StartCoroutine("Patroll");
        //StartCoroutine("DistanceDetection");
        
    }

    public void Update()
    {

        //if (playerDetected)
        //{
        //    StopCoroutine("Patroll");
        //    runningPatroll = null;
        //}
        //else if (runningPatroll == null)
        //{
            
        //    StartCoroutine("Patroll");
            
        //}

    }


    private Vector3 RandomDestination()
    {
        return new Vector3(Random.Range(min.x, max.x), 0, Random.Range(min.z, max.z));
    }


    #region Always Detect

    IEnumerator Follow()
    {
        while (true)
        {
            destination = player.transform.position;
            GetComponent<NavMeshAgent>().SetDestination(destination);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(1);
        }
    }

    #endregion

    #region Patroll Movement

    IEnumerator Patroll()
    {
        destination = path.GetChild(childrenIndex).position;
        GetComponent<NavMeshAgent>().SetDestination(destination);
        
        while (true)
        {
            
            if(Vector3.Distance(transform.position, destination) < 0.5f)
            {

                childrenIndex++;
                childrenIndex = childrenIndex % path.childCount;

                destination = path.GetChild(childrenIndex).position;
                GetComponent<NavMeshAgent>().SetDestination(destination);
                
            }
            yield return new WaitForSeconds(1);
        }
    }



    #endregion


    #region Distance Detection

    IEnumerator DistanceDetection()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < playerDetectionDistance)
            {
                if (runningPatroll != null)
                {
                    StopCoroutine("Patroll");
                    runningPatroll = null;
                }
                playerDetected = true;
                destination = player.transform.position;
                GetComponent<NavMeshAgent>().SetDestination(destination);
            }
            else
            {
                playerDetected = false;
                if(runningPatroll == null)
                    runningPatroll = StartCoroutine("Patroll");
            }

            yield return new WaitForSeconds(1);
        }
    }

    #endregion


    #region Collider Detection

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (runningPatroll != null)
            {
                StopCoroutine("Patroll");
                runningPatroll = null;
            }
            playerDetected = true;
            StartCoroutine("Follow");
        }
       
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopCoroutine("Follow");
            playerDetected = false;
            if (runningPatroll == null)
                runningPatroll = StartCoroutine("Patroll");
        }

    }

    #endregion

}


