using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioClip))]
public class ExplodingBarrel : MonoBehaviour, IHittable
{
    #region Variables

    [SerializeField] private Collider _collider;
    [SerializeField] private GameObject _cleanBarrel;
    [SerializeField] private GameObject[] _explodedBarrel;
    [SerializeField] private Rigidbody[] _rigidbodies;
    [SerializeField] private Collider _explosionCollider;
    [SerializeField] private GameObject _explosionVFX;
    [SerializeField] private GameObject _bigExplosionVFX;
    [SerializeField] private AudioSource _explosionSource1;
    [SerializeField] private AudioSource _explosionSource2;
    [SerializeField] private AudioClip _explosionSFX1;
    [SerializeField] private AudioClip _explosionSFX2;
    [SerializeField] private int _damageAmount = 800;
    [SerializeField] private int _hitID = 4;
    private const string _enemyTag = "Enemy";
    private const string _bodyHitName = "BodyHitFront";
    private const string _bodyDeathName = "BodyDeathFront";

    private Quaternion _startingRotation = Quaternion.Euler(90f, 0f, 0f);
    private Vector3 _startingPosition;

    #endregion

    #region Start, OnDisable

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider>();

        _rigidbodies = this.gameObject.GetComponentsInChildren<Rigidbody>();

        GameManager.onWaveStart += GameManager_onWaveStart;

        foreach (GameObject fragment in _explodedBarrel)
        {
            _startingPosition = fragment.transform.position;
            fragment.SetActive(false);
        }
    }

    private void OnDisable()
    {
        GameManager.onWaveStart -= GameManager_onWaveStart;
    }

    #endregion

    #region Event Methods

    private void GameManager_onWaveStart()
    {
        _collider.enabled = true;

        _cleanBarrel.SetActive(true);

        //respawn barrel
        foreach (GameObject fragment in _explodedBarrel)
        {
            fragment.SetActive(false);
            fragment.transform.position = _startingPosition;
            fragment.transform.localRotation = _startingRotation;
        }

        foreach(Rigidbody rb in _rigidbodies)
        {
            rb.velocity = Vector3.zero;
        }
    }

    #endregion

    #region Damage

    public void HitDamage(int damageAmount, Vector3 spawnPosition)
    {
        _collider.enabled = false;

        _bigExplosionVFX.SetActive(true);
        _explosionVFX.SetActive(true);

        AudioManager.Instance.PlayExplodingBarrelSound(_explosionSource1, _explosionSource2, _explosionSFX1, _explosionSFX2);

        //hide clean barrel
        _cleanBarrel.SetActive(false);

        GameManager.Instance.AddBarrelHits();

        //enable exploded pieces
        foreach(GameObject fragment in _explodedBarrel)
        {
            fragment.SetActive(true);
        }

        //add force to fragments
        foreach (Rigidbody rb in _rigidbodies)
        {
            Vector3 _offset = new Vector3(0.2f, -0.2f, 0.2f);
            rb.AddExplosionForce(500f, this.transform.position + _offset, 5f);
        }

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 10f);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.gameObject.CompareTag(_enemyTag))
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();

                if (damageable != null)
                    damageable.Damage(_damageAmount, _bodyHitName, _bodyDeathName, 2f, _hitID);
            }
        }
    }

    #endregion
}
