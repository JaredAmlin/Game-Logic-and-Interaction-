using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAI : MonoBehaviour
{
    //store waypoints in List
    [SerializeField] private List<Transform> _waypoints;

    [SerializeField] private GameObject[] _waypointOBJ;

    //store waypoint to move towards
    [SerializeField] private int _currentWaypoint;

    //store Waypoint Tag
    private const string _WaypointTag = "Waypoint";

    //handle to nav mesh agent
    private NavMeshAgent _agent;

    //handle to Animator
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Initialization()
    {
        //get animator component
        _animator = GetComponent<Animator>();

        //get Nav Mesh Agent
        _agent = GetComponent<NavMeshAgent>();

        //find waypoints by tag
        _waypointOBJ = GameObject.FindGameObjectsWithTag(_WaypointTag);

        NullChecks();
    }

    private void NullChecks()
    {
        //null check animator
        if (_animator == null)
            Debug.LogError("The Animator on the Agent is NULL.");

        if (_agent == null)
            Debug.LogError("The Nav Mesh Agent is NULL.");

        if (_waypointOBJ != null)
        {
            foreach (GameObject waypoint in _waypointOBJ)
            {
                //add waypoint transforms to list
                _waypoints.Add(waypoint.transform);
            }
        }
        else Debug.LogError("No Waypoints were found");
    }

    private void Movement()
    {
        if (_waypoints[_currentWaypoint] != null)
        {
            //move agent to current waypoint
            _agent.destination = _waypoints[_currentWaypoint].position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //check for waypoint tag
        if (other.CompareTag(_WaypointTag))
        {
            //check if at the final waypoint before incrementing current waypoint
            if (_currentWaypoint != _waypoints.Count - 1)
            {
                _currentWaypoint++;
            }
            else
            {
                Debug.Log("Game Over!");
                _currentWaypoint = 0;
                this.gameObject.SetActive(false);
                Vector3 spawnPosition = SpawnManager.Instance.GetSpawnPosition();
                this.gameObject.transform.position = spawnPosition;
            }
        }
    }
}
