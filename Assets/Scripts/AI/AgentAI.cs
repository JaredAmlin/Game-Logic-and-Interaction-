using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
public class AgentAI : MonoBehaviour, IDamageable
{
    #region Variables

    #region Serialized

    [Tooltip("Set the Starting Health of the Enemy")]
    [SerializeField] private int _startingHealth;
    [SerializeField] private int _currentHealth;
    private bool _isDead = false;

    public int Health
    {
        get
        {
            return _currentHealth;
        }
    }

    [Tooltip("Set the Points Awareded to the Player")]
    [SerializeField] private int _points;

    [Tooltip("Set the Movement Speed of the Enemy")]
    [SerializeField] private float _baseSpeed;

    [Tooltip("Set the Fall Speed of the Enemy when jumping")]
    [SerializeField] private float _jumpSpeed = 3f;

    [Tooltip("Used to temporarily slow the enemy while stunned")]
    [SerializeField] private float _hitSpeed;

    [Tooltip("Random time to wait while in Cover state")]
    [SerializeField] private float _minCoverWait = 1f;
    [SerializeField] private float _maxCoverWait = 5f;

    [Tooltip("Time to stop agent for Jumping Down animation")]
    [SerializeField] private float _jumpDelay = 2f;

    private enum _AiState { attack, running, cover, death, gameOver }
    [Tooltip("State Machine")]
    [SerializeField] private _AiState _currentState;

    //used for damaging the Power Core Reactor
    [Tooltip("Assign Collider from Mixamo Rig Left Hand Here")]
    [SerializeField] private Collider _handCollider;

    [Tooltip("Assign Death Clip to Play here")]
    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioSource _deathSource;

    #endregion

    #region Private

    //collider on parent object
    private Collider _collider;
    //handle to nav mesh agent
    private NavMeshAgent _agent;

    //store waypoints in List
    private List<Transform> _waypoints;
    //waypoint object instances
    private GameObject[] _waypointOBJ;
    //waypoint to move towards
    private int _currentWaypoint;

    //do not stop agent when jumping
    private bool _isJumping = false;

    //handle to Animator
    private Animator _animator;

    //used to stop the enemy agent for the duration of a stun animation
    private float _clipLength;

    //transform for power core reactor to attack
    private Transform _powerCore;

    //assign hit ID's to hit name variables
    private int _headHit = 0, _bodyHit = 1, _armHit = 2, _legHit = 3, _barrelHit = 4;

    //string names
    private const string _waypointTag = "Waypoint";
    private const string _isCover = "isCover";
    private const string _isAttacking = "isAttacking";
    private const string _takeCover = "TakeCover";
    private const string _respawn = "Respawn";
    private const string _stopAgent = "StopAgent";
    private const string _startAgent = "StartAgent";
    private const string _waitRoutine = "WaitForAnimation";
    private const string _attackRoutine = "AttackRoutine";
    private const string _sadIdle = "SadIdle";
    private const string _robotDance = "RobotDance";
    private const string _victoryDance = "Victory";
    private const string _hurdle = "Hurdle";
    private const string _disableHandCollider = "DisableHandCollider";

    //used to randomize death animation trigger name
    private string[] _deathAnimationNames;

    #endregion

    #endregion

    #region Awake, OnEnable, OnDisable

    private void Awake()
    {
        Initialization();
    }

    private void OnEnable()
    {
        _currentHealth = _startingHealth;

        _isDead = false;

        _collider.enabled = true;

        _handCollider.enabled = false;

        _animator.SetBool(_isCover, false);

        _animator.SetBool(_isAttacking, false);

        GameManager.onGameOver += GameManager_onGameOver;
        GameManager.onGameWin += GameManager_onGameWin;

        //set running state to default state
        _currentState = _AiState.running;

        //set state machine behavior
        StateCheck();
    }

    

    private void OnDisable()
    {
        GameManager.onGameOver -= GameManager_onGameOver;
        GameManager.onGameWin -= GameManager_onGameWin;
    }

    #endregion

    #region Initialization

    private void Initialization()
    {
        //initialize death animation names
        _deathAnimationNames = new string[] { _sadIdle, _robotDance, _victoryDance };

        //get collider on parent
        if (TryGetComponent<Collider>(out Collider collider))
        {
            _collider = collider;
        }
        else
            Debug.LogWarning("The Collider on the Enemy Parent is NULL");

        //get animator component
        if (TryGetComponent<Animator>(out Animator animator))
        {
            _animator = animator;
        }
        else
            Debug.LogWarning("The Animator on the Enemy Agent is NULL");
        
        //get Nav Mesh Agent
        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            _agent = agent;
        }
        else
            Debug.LogWarning("The Navmesh Agent on the Enemy is NULL");

        //initialize waypoint list
        _waypoints = new List<Transform>();

        _waypoints = SpawnManager.Instance.GetWaypoints();

        /*
        //find waypoints by tag
        _waypointOBJ = GameObject.FindGameObjectsWithTag(_waypointTag);

        if (_waypointOBJ != null)
        {
            foreach (GameObject waypoint in _waypointOBJ)
            {
                //add waypoint transforms to list
                _waypoints.Add(waypoint.transform);
            }
        }
        else
            Debug.LogWarning("The Enemy did not find any Waypoints");
        */

        PowerCore powerCore = FindObjectOfType<PowerCore>();

        if (powerCore != null)
        {
            _powerCore = powerCore.transform;
        }
        else
            Debug.LogWarning("The Enemy did not find the Power Core Reactor");
    }

    #endregion

    #region State Machine

    private void StateCheck()
    {
        switch (_currentState)
        {
            case _AiState.attack:
                Attack();
                break;
            case _AiState.running:
                Running();
                break;
            case _AiState.cover:
                Cover();
                break;
            case _AiState.death:
                Death();
                break;
            case _AiState.gameOver:
                GameOver();
                break;
            default:
                Debug.LogWarning("There is no enemy state for this case");
                break;
        }
    }

    #endregion

    #region Event Methods

    private void GameManager_onGameOver()
    {
        _currentState = _AiState.gameOver;

        StateCheck();
    }

    private void GameManager_onGameWin()
    {
        _currentState = _AiState.gameOver;

        StateCheck();
    }

    #endregion

    #region Private Methods

    #region Enemy States

    private void Running()
    {
        _agent.speed = _baseSpeed;
        _agent.isStopped = false;

        //set agent destination
        Destination();
    }

    private void Attack()
    {
        _collider.enabled = false;
        _agent.isStopped = true;

        _animator.SetBool(_isAttacking, true);

        StartCoroutine(_attackRoutine);
    }

    private void Cover()
    {
        _agent.isStopped = true;
        _animator.SetBool(_isCover, true);
        float randomTime = Random.Range(_minCoverWait, _maxCoverWait);
        Invoke(_takeCover, randomTime);
    }

    private void Death()
    {
        _collider.enabled = false;

        _deathSource.PlayOneShot(_deathClip);

        GameManager.Instance.AddPoints(_points);

        SpawnManager.Instance.RemoveEnemyCount();

        Invoke(_respawn, 5f);
    }

    private void GameOver()
    {
        _isDead = true;

        _collider.enabled = false;

        //game over behavior
        _agent.isStopped = true;

        //play game over animation
        int randomAnimation = Random.Range(0, _deathAnimationNames.Length);

        _animator.SetTrigger(_deathAnimationNames[randomAnimation]);
    }

    #endregion

    #region Invoked Methods

    //invoked by Cover Method
    private void TakeCover()
    {
        if (_isDead == false)
        {
            _animator.SetBool(_isCover, false);
            _currentState = _AiState.running;

            StateCheck();
        }
    }

    //invoked by Death Method
    private void Respawn()
    {
        _currentWaypoint = 0;
        this.gameObject.SetActive(false);
        Vector3 spawnPosition = SpawnManager.Instance.GetSpawnPosition();
        this.gameObject.transform.position = spawnPosition;
    }

    //invoked by Damage Method if leg hit
    private void StopAgent()
    {
        _agent.isStopped = true;
    }

    private void StartAgent()
    {
        _agent.isStopped = false;
    }

    #endregion

    #region Navigation

    private void Destination()
    {
        if (_waypoints[_currentWaypoint] != null)
        {
            //move agent to current waypoint
            _agent.destination = _waypoints[_currentWaypoint].position;
        }
        else
            Debug.LogWarning("The current waypoint on the enemy agent is NULL");
    }

    //used to randomly skip certain waypoints
    private void WayPointSplit()
    {
        int waypointSplit = Random.Range(0, 2);

        if (waypointSplit == 0)
        {
            _currentWaypoint++;
        }
        else
        {
            _currentWaypoint += 2;
        }

        if (_currentWaypoint > 10)
        {
            _currentWaypoint = 10;
        }

        Destination();
    }

    #endregion

    private void SetClip(float clipLength)
    {
        //stores length of hit animation clip to stop the enemy agent for the duration
        _clipLength = clipLength;
    }

    private float GetClip()
    {
        //returns length of hit animation clip to stop the enemy agent for the duration
        return _clipLength;
    }

    #endregion

    #region Public Methods

    #region Damage

    public void Damage(int damageAmount, string triggerName, string deathTriggerName, float clipLength, int hitID)
    {
        StopCoroutine(_waitRoutine);

        _currentHealth -= damageAmount;

        //trigger damage UI if exploding barrel hit
        if (hitID == _barrelHit)
        {
            GameObject newDamageDisplay = PoolManager.Instance.RequestDamageVFX(this.transform.position);

            IDisplay display = newDamageDisplay.GetComponent<IDisplay>();

            if (display != null)
            {
                display.DisplayText(damageAmount.ToString());
            }
        }

        //plays a hit animation
        if (_currentHealth > 0)
        {
            _animator.SetTrigger(triggerName);

            //stop the agent when hit with a head, body or exploding barrel hit
            if (hitID == _headHit | hitID == _bodyHit | hitID == _barrelHit)
            {
                //don't stop the agent while jumping
                if (_isJumping == false)
                    _agent.isStopped = true;
            }
            else if (hitID == _armHit)
            {
                if (_isJumping == false)
                    //temporarily slow the agent if hit in the arm
                    _agent.speed = _hitSpeed;
            }

            SetClip(clipLength);

            StartCoroutine(_waitRoutine);
        }
        else //plays a death animation
        {
            if (!_isDead)
            {
                _isDead = true;

                //stops a potential wait routine
                StopCoroutine(_waitRoutine);

                //stop the agent after root motion leg death animation has completed
                if (hitID == _legHit)
                {
                    Invoke(_stopAgent, clipLength);
                }
                else
                    _agent.isStopped = true;

                _animator.SetTrigger(deathTriggerName);

                _currentState = _AiState.death;

                GameManager.Instance.AddDeathHits(hitID);

                StateCheck();
            }
        }
    }

    #endregion

    //triggered through attack animation event
    public void EnableHandCollider()
    {
        _handCollider.enabled = true;

        Invoke(_disableHandCollider, 0.2f);
    }

    //invoked by Enable Hand Collider Method
    public void DisableHandCollider()
    {
        _handCollider.enabled = false;
    }

    #endregion

    #region Coroutines

    //started by string name
    private IEnumerator AttackRoutine()
    {
        while (_currentState == _AiState.attack)
        {
            _agent.transform.LookAt(_powerCore);
            yield return null;
        }
    }

    //started by string name
    private IEnumerator WaitForAnimation()
    {
        //returns the clip length of the hit animation being played
        //used to stop the enemy for it's duration when hit
        float waitTime = GetClip();

        yield return new WaitForSeconds(waitTime);

        _agent.speed = _baseSpeed;

        StateCheck();
    }

    #endregion

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
        //check for waypoint tag
        if (other.CompareTag(_waypointTag))
        {
            //check if cover waypoint 0,2,5,7
            if (_currentWaypoint == 0 | _currentWaypoint == 2 | _currentWaypoint == 5 | _currentWaypoint == 7)
            {
                //if so call cover state
                _currentState = _AiState.cover;

                //need to increment current waypoint
                _currentWaypoint++;

                StateCheck();
            }
            else
            {
                //check for waypoint to jump down from
                if (_currentWaypoint == 3)
                {
                    _animator.SetTrigger(_hurdle);
                    _agent.isStopped = true;
                    _isJumping = true;
                    _agent.speed = _jumpSpeed;
                    Invoke(_startAgent, _jumpDelay);
                }
                else if (_currentWaypoint == 4)
                {
                    _isJumping = false;
                    _agent.speed = _baseSpeed;
                }

                //check if at the final waypoint before incrementing current waypoint
                if (_currentWaypoint != _waypoints.Count - 1)
                {
                    if (_currentWaypoint > 3)
                    {
                        WayPointSplit();
                    }
                    else
                    {
                        _currentWaypoint++;

                        Destination();
                    }
                }
                else
                {
                    //attack power core
                    _currentState = _AiState.attack;

                    StateCheck();
                }
            }
        }
    }

    #endregion
}
