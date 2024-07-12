using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class ForceBarrier : MonoBehaviour, IHittable
{
    #region Variables

    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _hitClip;
    [SerializeField] private Collider _collider;
    private MeshRenderer _renderer;
    private const string _respawnName = "Respawn";

    #endregion

    #region Start

    private void Start()
    {
        _collider = GetComponent<Collider>();

        _renderer = GetComponent<MeshRenderer>();
    }

    #endregion

    #region Hittable

    public void HitDamage(int damageAmount, Vector3 spawnPosition)
    {
        _collider.enabled = false;

        _renderer.enabled = false;

        PoolManager.Instance.RequestElectricalBarrierHitFX(spawnPosition);

        //play hit sound
        AudioManager.Instance.PlayBarrierHitSound(_audioSource, _hitClip);

        GameManager.Instance.AddBarrierHits();

        Invoke(_respawnName, 5f);
    }

    #endregion

    #region Private Methods

    private void Respawn()
    {
        _collider.enabled = true;

        _renderer.enabled = true;
    }

    #endregion
}
