using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Switch : MonoBehaviour
{
    #region Variables

    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _stairs;
    private bool _isWaveActive = false;
    private bool _isGameOver = false;
    private const string _playerTag = "Player";
    private const string _switchTrigger = "Switch";

    #endregion

    #region Start, OnDisable

    private void Start()
    {
        if (TryGetComponent<Animator>(out Animator animator))
        {
            _animator = animator;
        }
        else
            Debug.LogWarning("The Animator on the Switch is NULL");

        GameManager.onGameOver += GameManager_onGameOver;
        GameManager.onGameWin += GameManager_onGameWin;
        GameManager.onWaveStart += GameManager_onWaveStart;
        GameManager.onWaveComplete += GameManager_onWaveComplete;
    }

    private void OnDisable()
    {
        GameManager.onGameOver -= GameManager_onGameOver;
        GameManager.onGameWin -= GameManager_onGameWin;
        GameManager.onWaveStart -= GameManager_onWaveStart;
        GameManager.onWaveComplete -= GameManager_onWaveComplete;
    }

    #endregion

    #region Event Methods

    private void GameManager_onWaveComplete()
    {
        _isWaveActive = false;
    }

    private void GameManager_onWaveStart()
    {
        _isWaveActive = true;
    }

    private void GameManager_onGameWin()
    {
        _isGameOver = true;
    }

    private void GameManager_onGameOver()
    {
        _isGameOver = true;
    }

    #endregion

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            //play switch animation
            _animator.SetTrigger(_switchTrigger);

            if (_stairs != null)
                _stairs.SetActive(true); //set the stairs active

            //start next wave if current wave is inactive, and game is not over
            if (_isWaveActive == false && _isGameOver == false)
                GameManager.Instance.WaveStart();
        }
    }

    #endregion
}
