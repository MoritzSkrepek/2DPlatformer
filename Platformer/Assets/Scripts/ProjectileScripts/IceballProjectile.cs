using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceballProjectile : BaseProjectile
{
    protected override void OnEnemyEffect(EnemyHealth enemy)
    {
        enemy.TakeDamage(base.damage);
        Debug.Log("Slowing enemy");
    }

    protected override void OnEnvirenmentHitEffect()
    {
        Debug.Log("Freezing area");
        FreezeArea();
    }

    // TODO: Implement freezing area effect so that ground is frozen / slippy
    private void FreezeArea()
    {
        return;
    }
}
