using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] private Transform[] _spawnPoint;

    [SerializeField] private float _spawnRate = 2f;

    private WaitForSeconds _spawnWaitTime;

    private const string _spawnRoutine = "SpawnRoutine";

    private bool _isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        PoolManager.Instance.TestInput();

        _spawnWaitTime = new WaitForSeconds(_spawnRate);

        StartCoroutine(_spawnRoutine);
    }

    public void TestInput()
    {
        Debug.Log("The Spawn Manager is working.");
    }

    public override void Initialization()
    {
        base.Initialization();

        Debug.Log("Spawn Manager Initialized.");
    }

    private IEnumerator SpawnRoutine()
    {
        while (!_isGameOver)
        {
            yield return _spawnWaitTime;
            //spawn enemies
            Debug.Log("Spawning Enemies");

            GameObject enemy = PoolManager.Instance.RequestEnemy();
            enemy.transform.position = GetSpawnPosition();
        }
    }

    public Vector3 GetSpawnPosition()
    {
        int randomSpawnPoint = (Random.Range(0, _spawnPoint.Length));

        return _spawnPoint[randomSpawnPoint].position;
    }
}
