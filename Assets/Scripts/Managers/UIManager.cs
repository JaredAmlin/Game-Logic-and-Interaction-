using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    #region Variables

    [SerializeField] private TMP_Text _pointsText;
    [SerializeField] private TMP_Text _enemyCountText;
    [SerializeField] private TMP_Text _timerText;

    //stats texts
    [SerializeField] private TMP_Text _shotsFiredText;
    [SerializeField] private TMP_Text _accuracyText;
    [SerializeField] private TMP_Text _aiDestroyedText, _aiHitText, _wavesClearedText;
    [SerializeField] private TMP_Text _headShotsText, _bodyKillsText, _armKillsText, _legKillsText, _explodingBarrelKillsText;
    [SerializeField] private TMP_Text _headHitsText, _bodyHitsText, _armHitsText, _legHitsText, _explodingBarrelHitsText, _barrierHitsText;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _reloadingText;

    //rector health
    [SerializeField] private Image _healthFillImage;

    //alpha fade
    [SerializeField] private GameObject _alpha;

    //ammo and reload
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private Image _ammoFillImage;
    [SerializeField] private Image _reloadFillImage;

    //game over
    [SerializeField] private TMP_Text _reactorGameOverText;
    [SerializeField] private TMP_Text _outOfAmmoGameOverText;

    //panels
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private GameObject _gameDataPanel;
    [SerializeField] private GameObject _objectivePanel;
    [SerializeField] private GameObject _wavePanel;
    [SerializeField] private GameObject _statsPanel;
    [SerializeField] private GameObject _gameWinPanel;
    [SerializeField] private GameObject _enemiesPanel;

    private bool _isGameOver = false;
    private bool _isGameWin = false;
 
    private const string _pointsString = "Points: ";
    private const string _enemyCountString = "Enemy Count: ";
    private const string _timeRemainingString = "Time Remaining: ";

    #endregion

    #region Start, OnDisable

    private void Start()
    {
        _pointsText.text = _pointsString + 0;
        _enemyCountText.text = _enemyCountString + 0;
        _timerText.text = _timeRemainingString + 00.00;

        _reloadFillImage.fillAmount = 1f;

        GameManager.onWaveStart += GameManager_onWaveStart;
        GameManager.onWaveComplete += GameManager_onWaveComplete;
        GameManager.onGameOver += GameManager_onGameOver;
        GameManager.onGameWin += GameManager_onGameWin;
        GameManager.onGameStart += GameManager_onGameStart;
    }

    private void OnDisable()
    {
        GameManager.onWaveStart -= GameManager_onWaveStart;
        GameManager.onWaveComplete -= GameManager_onWaveComplete;
        GameManager.onGameOver -= GameManager_onGameOver;
        GameManager.onGameWin -= GameManager_onGameWin;
        GameManager.onGameStart -= GameManager_onGameStart;
    }

    #endregion

    #region Event Methods

    private void GameManager_onGameWin()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _isGameWin = true;

        _gameWinPanel.gameObject.SetActive(true);

        CalculateStats();
    }

    private void GameManager_onGameOver()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _isGameOver = true;

        _gameOverPanel.gameObject.SetActive(true);

        CalculateStats();
    }

    private void GameManager_onWaveStart()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _wavePanel.gameObject.SetActive(false);
    }

    private void GameManager_onWaveComplete()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        int wave = GameManager.Instance.Wave();

        _waveText.text = "Wave " + wave;

        _wavePanel.SetActive(true);
    }

    private void GameManager_onGameStart()
    {
        _alpha.gameObject.SetActive(false);
        _gameDataPanel.gameObject.SetActive(true);
        _wavePanel.gameObject.SetActive(true);

        int wave = GameManager.Instance.Wave();

        _waveText.text = "Wave " + wave;

        _waveText.gameObject.SetActive(true);
    }

    #endregion

    #region Private Methods

    private void CalculateStats()
    {
        //assign values to stats
        var values = GameManager.Instance.GetStats();
        var shotsfired = values.Item1;
        var headShots = values.Item2;
        var bodyDeaths = values.Item3;
        var armDeaths = values.Item4;
        var legDeaths = values.Item5;
        var headHits = values.Item6;
        var bodyHits = values.Item7;
        var armHits = values.Item8;
        var legHits = values.Item9;
        var barrierHits = values.Item10;
        var barrelHits = values.Item11;
        var barrelDeaths = values.Item12;
        var accuracy = values.Item13;
        var wave = values.Item14;

        _aiDestroyedText.text = "Ai Destroyed: " + (headShots + bodyDeaths + armDeaths + legDeaths + barrelDeaths);
        _aiHitText.text = "Ai Hit: " + (headHits + bodyHits + armHits + legHits);
        _shotsFiredText.text = "Shots Fired: " + shotsfired;
        _headShotsText.text = "Head Shots: " + headShots;
        _bodyKillsText.text = "Body Kills: " + bodyDeaths;
        _armKillsText.text = "Arm Kills: " + armDeaths;
        _legKillsText.text = "Leg Kills: " + legDeaths;
        _headHitsText.text = "Head Hits: " + headHits;
        _bodyHitsText.text = "Body Hits: " + bodyHits;
        _armHitsText.text = "Arm Hits: " + armHits;
        _legHitsText.text = "Leg Hits: " + legHits;
        _barrierHitsText.text = "Barrier Hits: " + barrierHits;
        _explodingBarrelHitsText.text = "Exploding Barrel Hits: " + barrelHits;
        _explodingBarrelKillsText.text = "Exploding Barrel Kills: " + barrelDeaths;
        _accuracyText.text = "Accuracy: " + (int)accuracy + "%";
        _wavesClearedText.text = "Waves Completed: " + (wave - 1);
    }

    #endregion

    #region Public Methods
    public void UpdateEnemyCount(int enemyCount)
    {
        _enemyCountText.text = _enemyCountString + enemyCount;
    }

    public void UpdateTimeRemaining(float timeRemaining)
    {
        timeRemaining = Mathf.Round(timeRemaining * 100f) / 100f;

        _timerText.text = _timeRemainingString + timeRemaining;
    }

    /*
    public void StartGame()
    {
        _alpha.gameObject.SetActive(false);
        _gameDataPanel.gameObject.SetActive(true);
        _wavePanel.gameObject.SetActive(true);

        int wave = GameManager.Instance.Wave();

        _waveText.text = "Wave " + wave;

        _waveText.gameObject.SetActive(true);
    }
    */

    public void HideMainMenuPanel()
    {
        _mainMenuPanel.gameObject.SetActive(false);
    }

    public void StatsPanel()
    {
        if (_statsPanel.activeInHierarchy)
        {
            _statsPanel.SetActive(false);

            if (_isGameOver)
            {
                _gameOverPanel.SetActive(true);
            }
            else if (_isGameWin)
            {
                _gameWinPanel.SetActive(true);
            }
        }
        else
        {
            _gameOverPanel.SetActive(false);
            _gameWinPanel.SetActive(false);
            _statsPanel.SetActive(true);
        }
    }

    public void ControlsPanel()
    {
        if (_controlsPanel.activeInHierarchy)
        {
            _controlsPanel.SetActive(false);
        }
        else
            _controlsPanel.SetActive(true);
    }

    public void ObjectivePanel()
    {
        if (_objectivePanel.activeInHierarchy)
        {
            _objectivePanel.SetActive(false);
        }
        else
            _objectivePanel.SetActive(true);
    }

    public void EnemiesPanel()
    {
        if (_enemiesPanel.activeInHierarchy)
        {
            _enemiesPanel.SetActive(false);
        }
        else
            _enemiesPanel.SetActive(true);
    }

    public void SetReactorTextActive()
    {
        _reactorGameOverText.gameObject.SetActive(true);
    }

    public void SetOutOfAmmoTextActive()
    {
        _outOfAmmoGameOverText.gameObject.SetActive(true);
    }

    public void UpdateScore(int points)
    {
        _pointsText.text = _pointsString + points;
    }

    public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        float fillAmount = (float)currentAmmo / (float)maxAmmo;

        _ammoFillImage.fillAmount = fillAmount;
    }

    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        float fillAmount = (float)currentHealth / (float)maxHealth;

        _healthFillImage.fillAmount = fillAmount;
    }

    public void UpdateAmmoReserveUI(int ammoReserve)
    {
        _ammoText.text = ammoReserve.ToString();
    }

    public void ReloadCooldownUI(float cooldownTime)
    {
        _reloadingText.gameObject.SetActive(true);
        StartCoroutine(ReloadRoutine(cooldownTime));
    }

    #endregion

    #region Coroutines

    private IEnumerator ReloadRoutine(float cooldownTime)
    {
        float timeElapsed = 0f;

        while (timeElapsed < cooldownTime)
        {
            _reloadFillImage.fillAmount = Mathf.Lerp(0, 1, timeElapsed / cooldownTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _reloadingText.gameObject.SetActive(false);
    }

    #endregion
}
