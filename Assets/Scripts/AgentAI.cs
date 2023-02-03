using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAI : MonoBehaviour
{
    //store waypoints in List
    [SerializeField] private List<Transform> _waypoints;

    //store waypoint to move towards
    [SerializeField] private int _currentWaypoint;

    //store Waypoint Tag
    private const string _WaypointTag = "Waypoint";

    //handle to nav mesh agent
    private NavMeshAgent _agent;

    //handle to Animator
    private Animator _animator;

    private void OnEnable()
    {
        Initialization();
    }

    // Start is called before the first frame update
    void Start()
    {
        //set current waypoint to 1
        _currentWaypoint++;
    }

    // Update is called once per frame
    void Update()
    {
        //move agent to current waypoint
        _agent.destination = _waypoints[_currentWaypoint].position;
    }

    private void Initialization()
    {
        //get animator component
        _animator = GetComponent<Animator>();

        //get Nav Mesh Agent
        _agent = GetComponent<NavMeshAgent>();

        NullChecks();
    }

    private void NullChecks()
    {
        //null check animator
        if (_animator == null)
            Debug.LogError("The Animator on the Agent is NULL.");

        if (_agent == null)
            Debug.LogError("The Nav Mesh Agent is NULL.");

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
            else Debug.Log("Game Over!");
        }
    }
}
