using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int Health { get; }

    void Damage(int damageAmount, string triggerName, string deathTriggerName, float clipLength, int hitID);
}
