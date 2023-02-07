using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    //handle to robot Ai OBJ
    [SerializeField] private GameObject _robotOBJ;

    //list of robots to pool 
    [SerializeField] private List<GameObject> _robotPool;

    //cache Robot string name to variable
    private const string _robot = "Robot";

    //assumed max robots to instantiate
    [SerializeField] private int _maxRobots = 20;

    //parent to hold instantiated enemies
    [SerializeField] private GameObject _enemyContainer;

    // Start is called before the first frame update
    void Start()
    {
        SpawnManager.Instance.TestInput();

        NullChecks();

        _robotPool = CreateRobots(_maxRobots);
    }

    public void TestInput()
    {
        Debug.Log("The Pool Manager is working.");
    }

    public override void Initialization()
    {
        base.Initialization();

        Debug.Log("Pool Manager Initialized.");
    }

    private void NullChecks()
    {
        if (_robotOBJ == null)
            _robotOBJ = Resources.Load("Robot") as GameObject;
    }

    private List<GameObject> CreateRobots(int maxRobots)
    {
        for (int i = 0; i < maxRobots; i++)
        {
            Vector3 spawnPosition = SpawnManager.Instance.GetSpawnPosition();

            GameObject robot = Instantiate(_robotOBJ, spawnPosition, Quaternion.identity, _enemyContainer.transform);
            robot.SetActive(false);

            _robotPool.Add(robot);
        }

        return _robotPool;
    }

    public GameObject RequestEnemy()
    {
        foreach (GameObject robot in _robotPool)
        {
            if (robot.activeInHierarchy == false)
            {
                //robot is available
                //robot.transform.position = SpawnManager.Instance.GetSpawnPosition();
                robot.SetActive(true);
                return robot;
            }
        }

        Vector3 spawnPosition = SpawnManager.Instance.GetSpawnPosition();
        GameObject additionalRobot = Instantiate(_robotOBJ, spawnPosition, Quaternion.identity, _enemyContainer.transform);
        _robotPool.Add(additionalRobot);
        return additionalRobot;
    }
}
