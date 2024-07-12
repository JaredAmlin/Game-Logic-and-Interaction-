using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using GameDevHQ.FileBase.Plugins.FPS_Character_Controller;

public class GameManager : MonoSingleton<GameManager>
{
    #region Variables

    [SerializeField] private int _points;
    [SerializeField] private int _headHits, _bodyHits, _armHits, _legHits;
    [SerializeField] private int _headShots, _bodyDeaths, _armDeaths, _legDeaths, _barrelDeaths;
    [SerializeField] private int _enemyHits;
    [SerializeField] private int _barrierHits;
    [SerializeField] private int _barrelHits;
    [SerializeField] private int _shotsFired;
    [SerializeField] private float _accuracy;

    [SerializeField] private float _timeRemaining = 30f;
    [SerializeField] private float _waveTime = 30f;

    [SerializeField] private bool _isWaveActive = false;
    private bool _isGameOver = false;

    private const string _calculateAccuracy = "CalculateAccuracy";
    private const string _waveRoutine = "WaveRoutine";
    [SerializeField] private int _wave;

    [SerializeField] private PlayableDirector _tutorialDirector;
    [SerializeField] private PlayableDirector _mainMenuDirector;
    [SerializeField] private PlayableDirector _gameWinDirector;
    [SerializeField] private FPS_Controller _playerController;

    public static event Action onGameOver;

    public static event Action onTimerComplete;

    public static event Action onWaveComplete;

    public static event Action onWaveStart;

    public static event Action onGameWin;

    public static event Action onGameStart;

    #endregion

    #region Public Methods

    #region Stat Tracking

    public void AddEnemyHits(int hitID)
    {
        _enemyHits++;

        if (hitID == 0)
        {
            _headHits++;
        }
        else if (hitID == 1)
        {
            _bodyHits++;
        }
        else if (hitID == 2)
        {
            _armHits++;
        }
        else if (hitID == 3)
        {
            _legHits++;
        }
    }

    public void AddDeathHits(int hitID)
    {
        if (hitID == 0)
        {
            _headShots++;
        }
        else if (hitID == 1)
        {
            _bodyDeaths++;
        }
        else if (hitID == 2)
        {
            _armDeaths++;
        }
        else if (hitID == 3)
        {
            _legDeaths++;
        }
        else if (hitID == 4)
        {
            _barrelDeaths++;
        }
    }

    public void AddBarrierHits()
    {
        _barrierHits++;
    }

    public void AddBarrelHits()
    {
        _barrelHits++;
    }

    public void AddShotsFired()
    {
        _shotsFired++;

        Invoke(_calculateAccuracy, 0.2f);
    }

    #endregion

    #region Events

    public void GameOver()
    {
        _isWaveActive = false;
        _isGameOver = true;
        onGameOver?.Invoke();
    }

    public void WaveStart()
    {
        _wave++;
        _isWaveActive = true;
        _waveTime += 10f;
        _timeRemaining = _waveTime;
        onWaveStart?.Invoke();
        StartCoroutine(_waveRoutine);
    }

    public void WaveComplete()
    {
        onWaveComplete?.Invoke();
    }

    public void StartGame()
    {
        _tutorialDirector.Play();
        UIManager.Instance.HideMainMenuPanel();
    }

    public void WinGame()
    {
        if (_isGameOver == false)
        {
            _isWaveActive = false;
            _isGameOver = true;
            _gameWinDirector.Play();
            onGameWin?.Invoke();
        }
    }

    public void OnGameStart()
    {
        onGameStart?.Invoke();
    }

    #endregion

    #region Return Methods

    public int GetCurrentWave()
    {
        return _wave;
    }

    public int Wave()
    {
        return _wave + 1;
    }

    public (int, int, int, int, int, int, int, int, int, int, int, int, float, int) GetStats() // tuple return type
    {        
        return (_shotsFired, _headShots, _bodyDeaths, _armDeaths, _legDeaths, _headHits, _bodyHits, _armHits, _legHits, _barrierHits, _barrelHits, _barrelDeaths, _accuracy, _wave);
    }

    #endregion

    public void AddPoints(int points)
    {
        _points += points;

        //display points update on via UI Manager
        UIManager.Instance.UpdateScore(_points);
    }

    public void SkipTutorial()
    {
        PlayState tutorialState = _tutorialDirector.state;

        if (tutorialState == PlayState.Playing)
            _tutorialDirector.time = _tutorialDirector.duration;

        PlayState menuState = _mainMenuDirector.state;

        if (menuState == PlayState.Playing)
            _mainMenuDirector.time = _mainMenuDirector.duration;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Private Methods

    private void CalculateAccuracy()
    {
        int totalHits = _enemyHits + _barrierHits + _barrelHits;

        _accuracy = ((float)totalHits / (float)_shotsFired) * 100;
    }

    #endregion

    #region Coroutines

    private IEnumerator WaveRoutine()
    {
        while(_isWaveActive)
        {
            _timeRemaining -= Time.deltaTime;

            if(_timeRemaining <= 0f)
            {
                _isWaveActive = false;

                _timeRemaining = 0f;
            }

            UIManager.Instance.UpdateTimeRemaining(_timeRemaining);

            yield return null;
        }

        onTimerComplete?.Invoke();
    }

    #endregion 
}
