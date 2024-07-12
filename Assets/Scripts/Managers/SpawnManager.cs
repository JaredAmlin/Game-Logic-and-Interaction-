using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    #region Variables

    [SerializeField] private Transform[] _spawnPoint;

    [SerializeField] private float _spawnRate = 2f;

    [SerializeField] private int _enemyCount = 0;

    [SerializeField] private int _wave;

    [SerializeField] private int _randomEnemy;

    [SerializeField] private int _availableEnemies;

    [SerializeField] private List<Transform> _waypoints;

    private WaitForSeconds _spawnWaitTime;

    private const string _spawnRoutine = "SpawnRoutine";

    private bool _isSpawning = false;

    private bool _isCheckingForEnemies = false;

    #endregion

    #region Start, OnDisable

    // Start is called before the first frame update
    void Start()
    {
        GameManager.onGameOver += GameManager_onGameOver;
        GameManager.onGameWin += GameManager_onGameWin;
        GameManager.onTimerComplete += GameManager_onTimerComplete;
        GameManager.onWaveComplete += GameManager_onWaveComplete;
        GameManager.onWaveStart += GameManager_onWaveStart;

        _spawnWaitTime = new WaitForSeconds(_spawnRate);
    }

    private void OnDisable()
    {
        GameManager.onGameOver -= GameManager_onGameOver; 
        GameManager.onGameWin -= GameManager_onGameWin;
        GameManager.onTimerComplete += GameManager_onTimerComplete;
        GameManager.onWaveComplete -= GameManager_onWaveComplete;
        GameManager.onWaveStart -= GameManager_onWaveStart;
    }

    #endregion

    #region Event Methods

    private void GameManager_onWaveStart()
    {
        _wave = GameManager.Instance.GetCurrentWave();

        if (_wave == 1)
        {
            _availableEnemies++;
        }
        else if (_wave == 5)
        {
            _availableEnemies++;
        }
        else if (_wave == 10)
        {
            _availableEnemies++;
        }
        
        //_availableEnemies = 3;

        _isSpawning = true;
        _isCheckingForEnemies = false;
        StartCoroutine(_spawnRoutine);
    }

    private void GameManager_onTimerComplete()
    {
        _isSpawning = false;

        _isCheckingForEnemies = true;
    }

    private void GameManager_onWaveComplete()
    {
        _isSpawning = false;
    }

    private void GameManager_onGameOver()
    {
        _isSpawning = false;
    }

    private void GameManager_onGameWin()
    {
        _isSpawning = false;
    }

    #endregion

    #region Public Methods

    public Vector3 GetSpawnPosition()
    {
        int randomSpawnPoint = (Random.Range(0, _spawnPoint.Length));

        return _spawnPoint[randomSpawnPoint].position;
    }

    public void RemoveEnemyCount()
    {
        _enemyCount--;

        UIManager.Instance.UpdateEnemyCount(_enemyCount);

        if (_isCheckingForEnemies)
        {
            if (_enemyCount <= 0)
            {
                //wave complete
                GameManager.Instance.WaveComplete();
            }
        }
    }

    public List<Transform> GetWaypoints()
    {
        return _waypoints;
    }

    #endregion

    #region Coroutines

    private IEnumerator SpawnRoutine()
    {
        while (_isSpawning)
        {
            _randomEnemy = Random.Range(0, _availableEnemies);

            if (_randomEnemy == 0)
            {
                PoolManager.Instance.RequestRobot();
            }
            else if (_randomEnemy == 1)
            {
                PoolManager.Instance.RequestBigRobot();
            }
            else
                PoolManager.Instance.RequestGundam();
            
            _enemyCount++;

            UIManager.Instance.UpdateEnemyCount(_enemyCount);

            yield return _spawnWaitTime;
        }
    }

    #endregion
}
