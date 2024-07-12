using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerCore : MonoBehaviour
{
    #region Variables

    [SerializeField] private int _currentHealth;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private Collider _collider;
    [SerializeField] private GameObject _reactorOBJ;
    [SerializeField] private GameObject _explosionVFX;
    [SerializeField] private GameObject _bigExplosionVFX;
    [SerializeField] private GameObject _smoke1;
    [SerializeField] private GameObject _smoke2;
    private bool _isDead = false;
    private const string _handTag = "Hand";

    #endregion

    #region Start, OnDisable

    private void Start()
    {
        _currentHealth = _maxHealth;

        UIManager.Instance.UpdateHealthUI(_currentHealth, _maxHealth);

        _collider = GetComponent<Collider>();

        GameManager.onWaveStart += GameManager_onWaveStart;
    }

    private void OnDisable()
    {
        GameManager.onWaveStart -= GameManager_onWaveStart;
    }

    #endregion

    #region Event Methods

    private void GameManager_onWaveStart()
    {
        _currentHealth = _maxHealth;

        UIManager.Instance.UpdateHealthUI(_currentHealth, _maxHealth);
    }

    #endregion

    #region Damage, Death

    private void Damage()
    {
        _currentHealth -= 10;

        UIManager.Instance.UpdateHealthUI(_currentHealth, _maxHealth);

        CameraManager.Instance.ShakeCamera(1f, 40f);

        AudioManager.Instance.PlayReactorHitSound();

        if (_currentHealth <= _maxHealth / 2)
        {
            _smoke1.SetActive(true);
        }

        if (_currentHealth <= _maxHealth / 4)
        {
            _smoke2.SetActive(true);
        }

        if (_currentHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        if (_isDead == false)
        {
            _isDead = true;

            _collider.enabled = false;

            _currentHealth = 0;

            //set explosion active
            _bigExplosionVFX.SetActive(true);
            _explosionVFX.SetActive(true);

            _reactorOBJ.SetActive(false);

            GameManager.Instance.GameOver();

            UIManager.Instance.SetReactorTextActive();
        }
    }

    #endregion

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_handTag))
        {
            PoolManager.Instance.RequestImpactSparks(other.transform.position);
            PoolManager.Instance.RequestBarrierHitFX(other.transform.position);
            PoolManager.Instance.RequestElectricalBarrierHitFX(other.transform.position);
            Damage();
        }
    }

    #endregion
}
