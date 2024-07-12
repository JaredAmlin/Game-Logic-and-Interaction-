using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour, IHittable
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _hitClip;

    public void HitDamage(int damageAmount, Vector3 spawnPosition)
    {
        PoolManager.Instance.RequestBarrierHitFX(spawnPosition);

        //play hit sound
        AudioManager.Instance.PlayBarrierHitSound(_audioSource, _hitClip);
    }
}
