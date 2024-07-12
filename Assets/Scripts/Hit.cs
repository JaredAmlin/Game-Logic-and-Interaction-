using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationClip))]
public class Hit : MonoBehaviour, IHittable
{
    #region Variables

    [SerializeField] private int _hitMultiplier;

    [SerializeField] private string _hitTrigger;

    [SerializeField] private string _deathTrigger;

    [SerializeField] private AnimationClip _animationClip;

    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private int _hitID;

    private float _clipLength;

    private IDamageable enemyDamageable;

    #endregion

    #region Start

    private void Start()
    {
        enemyDamageable = GetComponentInParent<IDamageable>();

        _clipLength = _animationClip.length;
    }

    #endregion

    #region Hittable

    public void HitDamage(int damageAmount, Vector3 spawnPosition)
    {
        int damage = damageAmount * _hitMultiplier;

        PoolManager.Instance.RequestImpactSparks(spawnPosition);

        GameObject newDamageDisplay = PoolManager.Instance.RequestDamageVFX(spawnPosition);

        IDisplay display = newDamageDisplay.GetComponent<IDisplay>();

        if (display != null)
        {
            display.DisplayText(damage.ToString());
        }

        enemyDamageable.Damage(damage, _hitTrigger, _deathTrigger, _clipLength, _hitID);

        GameManager.Instance.AddEnemyHits(_hitID);

        _audioSource.Play();
    }

    #endregion
}
