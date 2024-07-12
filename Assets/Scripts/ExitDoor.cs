using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    #region Variables

    private bool _hasTriggered = false;
    private bool _isGameOver = false;
    private const string _playerTag = "Player";

    #endregion

    #region Start, OnDisable

    private void Start()
    {
        GameManager.onGameOver += GameManager_onGameOver;
    }

    private void OnDisable()
    {
        GameManager.onGameOver -= GameManager_onGameOver;
    }

    #endregion

    #region Event Methods

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
            if (_hasTriggered == false && _isGameOver == false)
            {
                _hasTriggered = true;

                //play win game director
                GameManager.Instance.WinGame();
            }
        }
    }

    #endregion
}
